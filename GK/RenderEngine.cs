using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform = GK.Math3D.Transform;

namespace GK
{
    class RenderEngine : Drawable
    {
        public static RenderEngine Instance { get; } = new RenderEngine();
        static RenderEngine()
        {
        }

        private RenderEngine()
        {
        }
        public List<Mesh> Meshes { get; set; }
        public Transform MatInvCamera { get; set; }
        public Transform Projection { get; set; }
        public RenderWindow Window { get; set; }

        private float[,] ZBuffer { get; set; }
        private Color[,] Bitmap { get; set; }


        private Vec3 LightSource;
        private readonly float kd = 1f;
        private readonly float Ip = 1f;
        private readonly float ks = 1f;
        private readonly float n = 10f;
        



        private Vertex3 GetLineIntersectionWithPlane(Vec3 planePoint, Vec3 planeNormal, Vertex3 lineStart, Vertex3 lineEnd)
        {
            // to be sure it is normal
            planeNormal = planeNormal.Normal();

            float plane_d = -planeNormal.Dot(planePoint);
            float ad = lineStart.Position.Dot(planeNormal);
            float bd = lineEnd.Position.Dot(planeNormal);
            float t = (-plane_d - ad) / (bd - ad);
            Vec3 lineStartToEnd = lineEnd.Position - lineStart.Position;
            Vec4Color colorStartToEnd = lineEnd.Color - lineStart.Color;
            Vec3 lineToIntersect = lineStartToEnd * t;
            Vec4Color colorToIntersect = colorStartToEnd * t;
            return new Vertex3(lineStart.Position + lineToIntersect, lineStart.Color + colorToIntersect);
        }

        private List<Triangle> ClipAgainstPlane(Vec3 planePoint, Vec3 planeNormal, Triangle tri)
        {
            planeNormal = planeNormal.Normal();

            // returns distance from point to plane
            float dist(Vec3 point)
            {
                return planeNormal.Dot(point) - planeNormal.Dot(planePoint);
            }

            // Create two temporary storage arrays to classify points either side of plane
            // If distance sign is positive, point lies on "inside" of plane
            Vertex3[] insidePoints = new Vertex3[3]; int nInsidePointCount = 0;
            Vertex3[] outsidePoints = new Vertex3[3]; int nOutsidePointCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = dist(tri[0].Position);
            float d1 = dist(tri[1].Position);
            float d2 = dist(tri[2].Position);

            if (d0 >= 0) { insidePoints[nInsidePointCount++] = tri[0]; }
            else { outsidePoints[nOutsidePointCount++] = tri[0]; }
            if (d1 >= 0) { insidePoints[nInsidePointCount++] = tri[1]; }
            else { outsidePoints[nOutsidePointCount++] = tri[1]; }
            if (d2 >= 0) { insidePoints[nInsidePointCount++] = tri[2]; }
            else { outsidePoints[nOutsidePointCount++] = tri[2]; }

            if (nInsidePointCount == 0) return new List<Triangle>();
            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                Vertex3 v0 = insidePoints[0];
                Vertex3 v1 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[0]);
                Vertex3 v2 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[1]);

                Triangle t = new Triangle(v0, v1, v2);
                //swap so normalvectors sign stays the same
                if (tri.NormalVector.Z * t.NormalVector.Z < 0)
                {
                    Vertex3 tmp = t[1];
                    t[1] = t[2];
                    t[2] = tmp;
                }
                return new List<Triangle>
                {
                    t,
                };
            }
            if (nInsidePointCount == 2 && nOutsidePointCount == 1)
            {
                Vertex3 v00 = insidePoints[0];
                Vertex3 v01 = insidePoints[1];
                Vertex3 v02 = GetLineIntersectionWithPlane(planePoint, planeNormal, v00, outsidePoints[0]);

                Vertex3 v10 = insidePoints[1];
                Vertex3 v11 = v02;
                Vertex3 v12 = GetLineIntersectionWithPlane(planePoint, planeNormal, v10, outsidePoints[0]);

                Triangle t1 = new Triangle(v00, v01, v02);
                Triangle t2 = new Triangle(v10, v12, v11);
                //swap so normalvectors sign stays the same
                if (tri.NormalVector.Z * t1.NormalVector.Z < 0)
                {
                    Vertex3 tmp = t1[1];
                    t1[1] = t1[2];
                    t1[2] = tmp;
                }
                if (tri.NormalVector.Z * t2.NormalVector.Z < 0)
                {
                    Vertex3 tmp = t2[1];
                    t2[1] = t2[2];
                    t2[2] = tmp;
                }
                return new List<Triangle>
                {
                    t1,
                    t2,
                };
            }
            if (nInsidePointCount == 3)
            {
                return new List<Triangle>
                {
                    tri,
                };
            }
            return new List<Triangle>();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //init frame
            ZBuffer = new float[Window.Size.X, Window.Size.Y];
            Bitmap = new Color[Window.Size.X, Window.Size.Y];

            Transform intoViewMoveAndScale = Transform.Identity.Translate(new Vec3(1, 1, 0)).Scale(new Vec3(Window.Size.X / 2, Window.Size.Y / 2, 1));
            Transform matView = intoViewMoveAndScale * Projection;
            //light
            Vec3 lightSource = MatInvCamera * new Vec3(-3, 3, -3);
            LightSource = lightSource;

            Vector2f l = new Vector2f();
            foreach (Mesh mesh in Meshes)
            {
                Mesh transformed = new Mesh();
                foreach (Triangle triangle in mesh)
                {
                    transformed.Add(MatInvCamera * triangle);
                }
                //clip in 3d against camera
                Mesh clipped = new Mesh();
                foreach (Triangle triangle in transformed)
                {
                    List<Triangle> clt = ClipAgainstPlane(new Vec3(0, 0, Camera.Instance.Near), new Vec3(0, 0, 1), triangle);
                    foreach (var item in clt)
                    {
                        clipped.Add(item);
                    }
                }
                

                Mesh projected = new Mesh();
                foreach (Triangle triangle in clipped)
                {
                    //only if visible
                    if (triangle.NormalVector.Dot(triangle[0].Position) < 0)
                    {
                        // ILLUMINATION - Phong
                        Vec3 N = triangle.NormalVector;
                        //camera position (0,0,0) - vertex position (after camerainverese)
                        Vec3 V0 = (- triangle[0].Position).Normal();
                        Vec3 V1 = (- triangle[1].Position).Normal();
                        Vec3 V2 = (- triangle[2].Position).Normal();
                        Vec3 L0 = (lightSource - triangle[0].Position).Normal();
                        Vec3 L1 = (lightSource - triangle[1].Position).Normal();
                        Vec3 L2 = (lightSource - triangle[2].Position).Normal();
                        Vec3 R0 = (-L0 - 2 * N.Dot(-L0) * N).Normal();
                        Vec3 R1 = (-L1 - 2 * N.Dot(-L1) * N).Normal();
                        Vec3 R2 = (-L2 - 2 * N.Dot(-L2) * N).Normal();
                        float I0 = Ip * (kd * N.Dot(L0) + ks * (float)Math.Pow(V0.Dot(R0), n));
                        float I1 = Ip * (kd * N.Dot(L1) + ks * (float)Math.Pow(V1.Dot(R1), n));
                        float I2 = Ip * (kd * N.Dot(L2) + ks * (float)Math.Pow(V2.Dot(R2), n));
                        //float I0 = Ip * (kd * N.Dot(L0) + ks * (float)Math.Pow(N.Dot(L0), n));
                        //float I1 = Ip * (kd * N.Dot(L1) + ks * (float)Math.Pow(N.Dot(L1), n));
                        //float I2 = Ip * (kd * N.Dot(L2) + ks * (float)Math.Pow(N.Dot(L2), n));
                        I0 = (float)Math.Max(I0, 0.2f);
                        I1 = (float)Math.Max(I1, 0.2f);
                        I2 = (float)Math.Max(I2, 0.2f);
                        Vec4Color shadedColor0 = triangle[0].Color * I0;
                        Vec4Color shadedColor1 = triangle[1].Color * I1;
                        Vec4Color shadedColor2 = triangle[2].Color * I2;

                        //project and move, and scale into view
                        Vec3 v0 = matView * triangle[0].Position;
                        Vec3 v1 = matView * triangle[1].Position;
                        Vec3 v2 = matView * triangle[2].Position;
                        Vertex3 vert0 = new Vertex3(v0, shadedColor0);
                        Vertex3 vert1 = new Vertex3(v1, shadedColor1);
                        Vertex3 vert2 = new Vertex3(v2, shadedColor2);
                        projected.Add(new Triangle(vert0, vert1, vert2));

                        //light source pos
                        Vec3 lv = matView * lightSource;
                        l = new Vector2f(lv.X, lv.Y);

                        //t.Color = shadedColor;
                        //projected.Add(new Triangle(v0, v1, v2, triangle.Color));
                        //projected.Add(t);
                    }
                }
                //clip 2D to screen borders
                Mesh clipped2D = new Mesh();
                foreach (Triangle triangle in projected)
                {
                    Queue<Triangle> qTris = new Queue<Triangle>();
                    qTris.Enqueue(triangle);
                    int nNewTriangles = 1;

                    for (int p = 0; p < 4; p++)
                    {
                        while (nNewTriangles > 0)
                        {
                            Triangle t = qTris.Dequeue();
                            nNewTriangles--;

                            List<Triangle> clippedTriangles = new List<Triangle>();
                            switch (p)
                            {
                                case 0:
                                    clippedTriangles = ClipAgainstPlane(new Vec3(), new Vec3(0, 1, 0), t);
                                    break;
                                case 1:
                                    clippedTriangles = ClipAgainstPlane(new Vec3(0, Camera.Instance.Height - 1, 0), new Vec3(0, -1, 0), t);
                                    break;
                                case 2:
                                    clippedTriangles = ClipAgainstPlane(new Vec3(), new Vec3(1, 0, 0), t);
                                    break;
                                case 3:
                                    clippedTriangles = ClipAgainstPlane(new Vec3(Camera.Instance.Width - 1, 0, 0), new Vec3(-1, 0, 0), t);
                                    break;

                            }
                            foreach (var item in clippedTriangles)
                            {
                                qTris.Enqueue(item);
                            }
                        }
                        nNewTriangles = qTris.Count;
                    }
                    foreach (var item in qTris)
                    {
                        clipped2D.Add(item);
                    }
                }


                //draw



                foreach (Triangle triangle in clipped2D)
                {
                    if (Options.Instance.DrawWireframe)
                        //test zbuffer wireframe
                        DrawTriangle(triangle, PrimitiveType.LineStrip);
                    else
                        //test zbuffer fill
                        DrawTriangle(triangle, PrimitiveType.Triangles);

                }
            }
            //draw bitmap to screen
            Image img = new Image(Bitmap);
            Texture tex = new Texture(img);
            Sprite s = new Sprite(tex);
            target.Draw(s, states);
            s.Dispose();
            tex.Dispose();
            img.Dispose();
            CircleShape c = new CircleShape(5) { Position = l };
            target.Draw(c);
        }
        private void DrawTriangle(Triangle triangle, PrimitiveType primitiveType)
        {
            if(primitiveType == PrimitiveType.Triangles)
            {
                void fillBottomFlatTriangle(Vertex3 v1, Vertex3 v2, Vertex3 v3)
                {
                    float invslope1 = (v2.Position.X - v1.Position.X) / (v2.Position.Y - v1.Position.Y);
                    float invslope2 = (v3.Position.X - v1.Position.X) / (v3.Position.Y - v1.Position.Y);

                    float curx1 = v1.Position.X;
                    float curx2 = v1.Position.X;

                    for (int scanlineY = (int)v1.Position.Y; scanlineY <= v2.Position.Y; scanlineY++)
                    {
                        float z1 = v1.Position.Z + ((float)(scanlineY - v1.Position.Y) / (float)(v2.Position.Y - v1.Position.Y)) * (v2.Position.Z - v1.Position.Z);
                        float z2 = v1.Position.Z + ((float)(scanlineY - v1.Position.Y) / (float)(v3.Position.Y - v1.Position.Y)) * (v3.Position.Z - v1.Position.Z);
                        Vec4Color c1 = v1.Color + ((float)(scanlineY - v1.Position.Y) / (float)(v2.Position.Y - v1.Position.Y)) * (v2.Color - v1.Color);
                        Vec4Color c2 = v1.Color + ((float)(scanlineY - v1.Position.Y) / (float)(v3.Position.Y - v1.Position.Y)) * (v3.Color - v1.Color);
                        Vec3 from = new Vec3((float)Math.Round(curx1), scanlineY, z1);
                        Vec3 to = new Vec3((float)Math.Round(curx2), scanlineY, z2);
                        DrawTriangleLine(from, c1, to, c2);
                        curx1 += invslope1;
                        curx2 += invslope2;
                    }
                }

                void fillTopFlatTriangle(Vertex3 v1, Vertex3 v2, Vertex3 v3)
                {
                    float invslope1 = (v3.Position.X - v1.Position.X) / (v3.Position.Y - v1.Position.Y);
                    float invslope2 = (v3.Position.X - v2.Position.X) / (v3.Position.Y - v2.Position.Y);

                    float curx1 = v3.Position.X;
                    float curx2 = v3.Position.X;

                    for (int scanlineY = (int)v3.Position.Y; scanlineY > v1.Position.Y; scanlineY--)
                    {
                        float z1 = v3.Position.Z + ((float)(scanlineY - v3.Position.Y) / (float)(v1.Position.Y - v3.Position.Y)) * (v1.Position.Z - v3.Position.Z);
                        float z2 = v3.Position.Z + ((float)(scanlineY - v3.Position.Y) / (float)(v2.Position.Y - v3.Position.Y)) * (v2.Position.Z - v3.Position.Z);
                        Vec3 from = new Vec3((float)Math.Round(curx1), scanlineY, z1);
                        Vec3 to = new Vec3((float)Math.Round(curx2), scanlineY, z2);
                        Vec4Color c1 = v3.Color + ((float)(scanlineY - v3.Position.Y) / (float)(v1.Position.Y - v3.Position.Y)) * (v1.Color - v3.Color);
                        Vec4Color c2 = v3.Color + ((float)(scanlineY - v3.Position.Y) / (float)(v2.Position.Y - v3.Position.Y)) * (v2.Color - v3.Color);
                        DrawTriangleLine(from, c1, to, c2);
                        curx1 -= invslope1;
                        curx2 -= invslope2;
                    }
                }


                List<Vertex3> sortedVecs = new List<Vertex3>
                {
                    triangle[0],
                    triangle[1],
                    triangle[2],
                };
                sortedVecs = sortedVecs.OrderBy(i => i.Position.Y).ToList();
                Vertex3 A = sortedVecs[0];
                Vertex3 B = sortedVecs[1];
                Vertex3 C = sortedVecs[2];
                //to ints
                A.Position = new Vec3((int)A.Position.X,(int)A.Position.Y, A.Position.Z);
                B.Position = new Vec3((int)B.Position.X,(int)B.Position.Y, B.Position.Z);
                C.Position = new Vec3((int)C.Position.X,(int)C.Position.Y, C.Position.Z);

                /* here we know that v1.y <= v2.y <= v3.y */
                /* check for trivial case of bottom-flat triangle */
                if (B.Position.Y == C.Position.Y)
                {
                    fillBottomFlatTriangle(A, B, C);
                }
                /* check for trivial case of top-flat triangle */
                else if (A.Position.Y == B.Position.Y)
                {
                    fillTopFlatTriangle(A, B, C);
                }
                else
                {
                    /* general case - split the triangle in a topflat and bottom-flat one */
                    float z = A.Position.Z + ((float)(B.Position.Y - A.Position.Y) / (float)(C.Position.Y - A.Position.Y)) * (C.Position.Z - A.Position.Z);
                    Vec4Color c = A.Color + ((float)(B.Position.Y - A.Position.Y) / (float)(C.Position.Y - A.Position.Y)) * (C.Color - A.Color);
                    Vec3 v4 = new Vec3(
                      (int)(A.Position.X + ((float)(B.Position.Y - A.Position.Y) / (float)(C.Position.Y - A.Position.Y)) * (C.Position.X - A.Position.X)),
                      B.Position.Y,
                      z
                      );
                    Vertex3 v = new Vertex3(v4, c);

                    fillBottomFlatTriangle(A, B, v);
                    fillTopFlatTriangle(B, v, C);
                }

                #region barycentric
                /* spanning vectors of edge (v1,v2) and (v1,v3) */
                //Vec2 vs1 = new Vec2(triangle[1].X - triangle[0].X, triangle[1].Y - triangle[0].Y);
                //Vec2 vs2 = new Vec2(triangle[2].X - triangle[0].X, triangle[2].Y - triangle[0].Y);

                //float minX, minY;
                //minX = minY = float.MaxValue;
                //float maxX, maxY;
                //maxX = maxY = float.MinValue;

                //minX = Math.Min(minX, triangle[0].X);
                //minX = Math.Min(minX, triangle[1].X);
                //minX = Math.Min(minX, triangle[2].X);
                //maxX = Math.Max(maxX, triangle[0].X);
                //maxX = Math.Max(maxX, triangle[1].X);
                //maxX = Math.Max(maxX, triangle[2].X);

                //minY = Math.Min(minY, triangle[0].Y);
                //minY = Math.Min(minY, triangle[1].Y);
                //minY = Math.Min(minY, triangle[2].Y);
                //maxY = Math.Max(maxY, triangle[0].Y);
                //maxY = Math.Max(maxY, triangle[1].Y);
                //maxY = Math.Max(maxY, triangle[2].Y);


                //for (int x = (int)minX; x <= maxX; x++)
                //{
                //    for (int y = (int)minY; y <= maxY; y++)
                //    {
                //        Vec2 q = new Vec2(x - triangle[0].X, y - triangle[0].Y);

                //        float s = q.Cross(vs2) / vs1.Cross(vs2);
                //        float t = vs1.Cross(q) / vs1.Cross(vs2);

                //        if ((s >= 0) && (t >= 0) && (s + t <= 1))
                //        { /* inside triangle */
                //            float z = s*triangle[0].Z + t * triangle[1].Z + (1-s-t) * triangle[2].Z;
                //            DrawPixel(new Vec3(x,y,z),triangle.Color);
                //        }
                //    }
                //}
                #endregion
            }
            else if (primitiveType == PrimitiveType.LineStrip)
            {
                //DrawLine(triangle[0], triangle.Color, triangle[1], triangle.Color);
                //DrawLine(triangle[1], triangle.Color, triangle[2], triangle.Color);
                //DrawLine(triangle[2], triangle.Color, triangle[0], triangle.Color);
                DrawTriangleLine(triangle[0].Position, (Vec4Color)Color.White, triangle[1].Position, (Vec4Color)Color.White);
                DrawTriangleLine(triangle[1].Position, (Vec4Color)Color.White, triangle[2].Position, (Vec4Color)Color.White);
                DrawTriangleLine(triangle[2].Position, (Vec4Color)Color.White, triangle[0].Position, (Vec4Color)Color.White);
            }
        }

        private void DrawPixel(Vec3 pixel, Vec4Color color)
        {
            int screenX = (int)pixel.X;
            int screenY = (int)pixel.Y;
            if (pixel.Z <= ZBuffer[screenX, screenY])
            {
                ZBuffer[screenX, screenY] = pixel.Z;
                Bitmap[screenX, screenY] = (Color)color;
            }
        }
        private void DrawTriangleLine(Vec3 from, Vec4Color color1, Vec3 to, Vec4Color color2)
        {
            float m;
            if (to.X - from.X != 0) m = (to.Y - from.Y) / (to.X - from.X);
            else m = to.Y - from.Y;
            if (Math.Abs(m) >= 1)
            {
                Vec3 start;
                Vec3 end;
                Vec4Color startColor, endColor;
                if(from.Y < to.Y)
                {
                    start = from;
                    end = to;
                    startColor = color1;
                    endColor = color2;
                }
                else
                {
                    start = to;
                    end = from;
                    startColor = color2;
                    endColor = color1;
                }


                float x = start.X;
                for (float y = start.Y; y <= end.Y; y++)
                {
                    float traverseFraction = (end.Y - y) / (end.Y - start.Y);
                    float z = start.Z + (end.Z - start.Z) * traverseFraction;
                    
                    Vec3 pixel = new Vec3(x, y, z);
                    Vec4Color finalColor = startColor * traverseFraction + endColor * (1 - traverseFraction);
                    DrawPixel(pixel,finalColor);
                    x += (to.X - from.X != 0) ? 1 / m : 0;
                }
            }
            else
            {
                Vec3 start;
                Vec3 end;
                Vec4Color startColor, endColor;
                if (from.X < to.X)
                {
                    start = from;
                    end = to;
                    startColor = color1;
                    endColor = color2;
                }
                else
                {
                    start = to;
                    end = from;
                    startColor = color2;
                    endColor = color1;
                }

                float y = start.Y;
                for (float x = start.X; x <= end.X; x++)
                {
                    float traverseFraction = (end.X - x) / (end.X - start.X);
                    float z = start.Z + (end.Z - start.Z) * traverseFraction;

                    Vec3 pixel = new Vec3(x, y, z);
                    Vec4Color finalColor = startColor * traverseFraction + endColor * (1 - traverseFraction);
                    DrawPixel(pixel, finalColor);
                    y += m;
                }
            }
        }
        private Color Mix(Color c1, Color c2, float fraction)
        {
            float r = c1.R * fraction + c2.R * (1 - fraction);
            float g = c1.G * fraction + c2.G * (1 - fraction);
            float b = c1.B * fraction + c2.B * (1 - fraction);
            float a = c1.A * fraction + c2.A * (1 - fraction);
            r = Math.Max(r, 0);
            g = Math.Max(g, 0);
            b = Math.Max(b, 0);
            a = Math.Max(a, 0);
            r = Math.Min(r, 255);
            g = Math.Min(g, 255);
            b = Math.Min(b, 255);
            a = Math.Min(a, 255);

            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
