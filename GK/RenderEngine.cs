using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Drawable3D> Drawables3D { get; set; } = new List<Drawable3D>();
        public List<LightSource> LightSources { get; set; } = new List<LightSource>();
        public Transform MatInvCamera { get; set; }
        public Transform Projection { get; set; }
        public RenderWindow Window { get; set; }

        private float[,] ZBuffer { get; set; }
        private Color[,] Bitmap { get; set; }


        //private readonly float Ip = 1f;

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

        private List<Tri> ClipAgainstPlane(Vec3 planePoint, Vec3 planeNormal, Tri tri)
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

            if (nInsidePointCount == 0) return new List<Tri>();
            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                Vertex3 v0 = insidePoints[0];
                Vertex3 v1 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[0]);
                Vertex3 v2 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[1]);

                Tri t = new Tri(tri) { v0 = v0, v1 = v1, v2 = v2 };
                //swap so normalvectors sign stays the same
                if (tri.NormalVector.Z * t.NormalVector.Z < 0)
                {
                    Vertex3 tmp = t[1];
                    t[1] = t[2];
                    t[2] = tmp;
                }
                return new List<Tri>
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

                Tri t1 = new Tri(tri) { v0 = v00, v1 = v01, v2 = v02 };
                Tri t2 = new Tri(tri) { v0 = v10, v1 = v11, v2 = v12 };
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
                return new List<Tri>
                {
                    t1,
                    t2,
                };
            }
            if (nInsidePointCount == 3)
            {
                return new List<Tri>
                {
                    tri,
                };
            }
            return new List<Tri>();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //init frame
            ZBuffer = new float[Window.Size.X, Window.Size.Y];
            Bitmap = new Color[Window.Size.X, Window.Size.Y];

            Transform intoViewMoveAndScale = Transform.Identity.Translate(new Vec3(1, 1, 0)).Scale(new Vec3(Window.Size.X / 2, Window.Size.Y / 2, 1));
            Transform matView = intoViewMoveAndScale * Projection;

            //Vector2f l = new Vector2f();
            foreach (Drawable3D drawable in Drawables3D)
            {
                Mesh mesh = drawable.GetMesh();
                Transform matMesh = mesh.Transform;
                Mesh transformed = new Mesh();
                foreach (Tri triangle in mesh)
                {
                    transformed.Add(MatInvCamera * matMesh * triangle);
                }
                //clip in 3d against camera
                Mesh clipped = new Mesh();
                foreach (Tri triangle in transformed)
                {
                    List<Tri> clt = ClipAgainstPlane(new Vec3(0, 0, Camera.Instance.Near), new Vec3(0, 0, 1), triangle);
                    foreach (var item in clt)
                    {
                        clipped.Add(item);
                    }
                }


                Mesh projected = new Mesh();
                foreach (Tri triangle in clipped)
                {
                    //only if visible
                    if (triangle.NormalVector.Dot(triangle[0].Position) < 0)
                    {
                        Vec4Color shadedColor0 = new Vec4Color();
                        Vec4Color shadedColor1 = new Vec4Color();
                        Vec4Color shadedColor2 = new Vec4Color();
                        foreach (var lsrc in LightSources)
                        {
                            Vec3 lightPos = MatInvCamera * lsrc.Position;
                            // ILLUMINATION - Phong
                            Vec3 N = triangle.NormalVector;
                            float kd = triangle.kd;
                            float ks = triangle.ks;
                            float n = triangle.n;
                            //camera position (0,0,0) - vertex position (after camerainverese)
                            Vec3 V0 = (-triangle[0].Position).Normal();
                            Vec3 V1 = (-triangle[1].Position).Normal();
                            Vec3 V2 = (-triangle[2].Position).Normal();
                            Vec3 L0 = (lightPos - triangle[0].Position).Normal();
                            Vec3 L1 = (lightPos - triangle[1].Position).Normal();
                            Vec3 L2 = (lightPos - triangle[2].Position).Normal();
                            Vec3 R0 = (-L0 - 2 * N.Dot(-L0) * N).Normal();
                            Vec3 R1 = (-L1 - 2 * N.Dot(-L1) * N).Normal();
                            Vec3 R2 = (-L2 - 2 * N.Dot(-L2) * N).Normal();
                            float minus = 1;
                            if (V0.Dot(R0) < 0) minus = -1;
                            float I0 = lsrc.Intensity * (kd * N.Dot(L0) + ks * minus * (float)Math.Pow(V0.Dot(R0), n));
                            if (V1.Dot(R1) < 0) minus = -1;
                            float I1 = lsrc.Intensity * (kd * N.Dot(L1) + ks * minus * (float)Math.Pow(V1.Dot(R1), n));
                            if (V2.Dot(R2) < 0) minus = -1;
                            float I2 = lsrc.Intensity * (kd * N.Dot(L2) + ks * minus * (float)Math.Pow(V2.Dot(R2), n));
                            I0 = Math.Max(I0, 0.2f);
                            I1 = Math.Max(I1, 0.2f);
                            I2 = Math.Max(I2, 0.2f);

                            shadedColor0 += triangle[0].Color * I0;
                            shadedColor1 += triangle[1].Color * I1;
                            shadedColor2 += triangle[2].Color * I2;
                        }

                        //project and move, and scale into view
                        Vec3 v0 = matView * triangle[0].Position;
                        Vec3 v1 = matView * triangle[1].Position;
                        Vec3 v2 = matView * triangle[2].Position;
                        Vertex3 vert0 = new Vertex3(v0, shadedColor0);
                        Vertex3 vert1 = new Vertex3(v1, shadedColor1);
                        Vertex3 vert2 = new Vertex3(v2, shadedColor2);
                        projected.Add(new Tri(triangle) { v0 = vert0, v1 = vert1, v2 = vert2 });
                    }
                }
                //clip 2D to screen borders
                Mesh clipped2D = new Mesh();
                foreach (Tri triangle in projected)
                {
                    Queue<Tri> qTris = new Queue<Tri>();
                    qTris.Enqueue(triangle);
                    int nNewTriangles = 1;

                    for (int p = 0; p < 4; p++)
                    {
                        while (nNewTriangles > 0)
                        {
                            Tri t = qTris.Dequeue();
                            nNewTriangles--;

                            List<Tri> clippedTriangles = new List<Tri>();
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

                //draw triangles
                foreach (Tri triangle in clipped2D)
                {
                    if (Options.Instance.ShowWireframe)
                        //test zbuffer wireframe
                        DrawTriangle(triangle, PrimitiveType.LineStrip);
                    else
                        //test zbuffer fill
                        DrawTriangle(triangle, PrimitiveType.Triangles);

                }
                //draw light sources (4 pixels)
                foreach (LightSource light in LightSources)
                {
                    Vec3 lpos = MatInvCamera * light.Position;
                    if (lpos.Z > 0)
                    {
                        Vec3 lPosOnScreen = matView * lpos;
                        DrawPixel(lPosOnScreen, (Vec4Color)Color.White);
                        DrawPixel(lPosOnScreen+new Vec3(0,1), (Vec4Color)Color.White);
                        DrawPixel(lPosOnScreen+new Vec3(1,0), (Vec4Color)Color.White);
                        DrawPixel(lPosOnScreen+new Vec3(1,1), (Vec4Color)Color.White);
                    }
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
        }
        private void DrawTriangle(Tri triangle, PrimitiveType primitiveType)
        {
            if (primitiveType == PrimitiveType.Triangles)
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
                        DrawLine(from.X, from.Z, c1, to.X, to.Z, c2, scanlineY);
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
                        DrawLine(from.X,from.Z,c1,to.X,to.Z,c2,scanlineY);
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
                A.Position = new Vec3((int)A.Position.X, (int)A.Position.Y, A.Position.Z);
                B.Position = new Vec3((int)B.Position.X, (int)B.Position.Y, B.Position.Z);
                C.Position = new Vec3((int)C.Position.X, (int)C.Position.Y, C.Position.Z);

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
                DrawLine(triangle[0].Position, (Vec4Color)Color.White, triangle[1].Position, (Vec4Color)Color.White, false);
                DrawLine(triangle[1].Position, (Vec4Color)Color.White, triangle[2].Position, (Vec4Color)Color.White, false);
                DrawLine(triangle[2].Position, (Vec4Color)Color.White, triangle[0].Position, (Vec4Color)Color.White, false);
            }
        }

        private void DrawPixel(Vec3 pixel, Vec4Color color)
        {
            int screenX = (int)pixel.X;
            int screenY = (int)pixel.Y;
            if (screenX < 0 || screenX >= Bitmap.GetLength(0)) return;
            if (screenY < 0 || screenY >= Bitmap.GetLength(1)) return;
            if (pixel.Z <= ZBuffer[screenX, screenY])
            {
                //drawing
                ZBuffer[screenX, screenY] = pixel.Z;
                Bitmap[screenX, screenY] = (Color)color;
            }
        }

        private void DrawPixel(Vec2 pixel, float z, Vec4Color color)
        {
            DrawPixel(new Vec3(pixel.X, pixel.Y, z), color);
        }

        private void DrawLine(Vec3 from, Vec4Color color0, Vec3 to, Vec4Color color1, bool wu = true)
        {
            if (wu)
            {
                float x0 = from.X;
                float x1 = to.X;
                float y0 = from.Y;
                float y1 = to.Y;
                float z0 = from.Z;
                float z1 = to.Z;
                //help functions
                int ipart(float x) { return (int)x; }
                int round(float x) { return ipart(x + 0.5f); }
                float fpart(float x)
                {
                    if (x < 0) return 1 - (x - (float)Math.Floor(x));
                    return x - (float)Math.Floor(x);
                }
                float rfpart(float x)
                {
                    return 1 - fpart(x);
                }
                void swap(ref float o1, ref float o2)
                {
                    float tmp = o1;
                    o1 = o2;
                    o2 = tmp;
                }
                //
                bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
                if (steep)
                {
                    swap(ref x0, ref y0);
                    swap(ref x1, ref y1);
                }
                if (x0 > x1)
                {
                    swap(ref x0, ref x1);
                    swap(ref y0, ref y1);
                    swap(ref z0, ref z1);
                }

                float dx = x1 - x0;
                float dy = y1 - y0;
                float gradient = dx == 0 ? 1 : dy / dx;


                //start point
                int xEnd = round(x0);
                float yEnd = y0 + gradient * (xEnd - x0);
                float xGap = rfpart(x0 + 0.5f);
                int xPixel1 = xEnd;
                int yPixel1 = ipart(yEnd);

                //here
                Vec4Color c0 = color0;
                Vec4Color c1 = color0;
                c0.A *= rfpart(yEnd) * xGap;
                c1.A *= fpart(yEnd) * xGap;
                if (steep)
                {
                    DrawPixel(new Vec3(yPixel1, xPixel1, z0), c0);
                    DrawPixel(new Vec3(yPixel1 + 1, xPixel1, z0), c1);
                    //SetPixel(g, yPixel1, xPixel1, c1);
                    //SetPixel(g, yPixel1 + 1, xPixel1, c2);
                }
                else
                {
                    DrawPixel(new Vec3(xPixel1, yPixel1, z0), c0);
                    DrawPixel(new Vec3(xPixel1, yPixel1 + 1, z0), c1);
                    //SetPixel(g, xPixel1, yPixel1, c1);
                    //SetPixel(g, xPixel1, yPixel1 + 1, c2);
                }
                float intery = yEnd + gradient;

                //end point
                xEnd = round(x1);
                yEnd = y1 + gradient * (xEnd - x1);
                xGap = fpart(x1 + 0.5f);
                int xPixel2 = xEnd;
                int yPixel2 = ipart(yEnd);

                ///here
                c0 = color1;
                c1 = color1;
                c0.A *= rfpart(yEnd) * xGap;
                c1.A *= fpart(yEnd) * xGap;
                if (steep)
                {
                    DrawPixel(new Vec3(yPixel2, xPixel2, z1), c0);
                    DrawPixel(new Vec3(yPixel2 + 1, xPixel2, z1), c1);
                    //SetPixel(g, yPixel2, xPixel2, c1);
                    //SetPixel(g, yPixel2 + 1, xPixel2, c2);
                }
                else
                {
                    DrawPixel(new Vec3(xPixel2, yPixel2, z1), c0);
                    DrawPixel(new Vec3(xPixel2, yPixel2 + 1, z1), c1);
                    //SetPixel(g, xPixel2, yPixel2, c1);
                    //SetPixel(g, xPixel2, yPixel2 + 1, c2);
                }


                //between
                if (steep)
                {
                    for (int x = (xPixel1 + 1); x <= xPixel2 - 1; x++)
                    {
                        ///here
                        float t = (x - xPixel1 - 1) / (float)(xPixel2 - xPixel1 - 2);
                        Vec4Color c = (1 - t) * color0 + t * color1;
                        c0 = c;
                        c1 = c;
                        c0.A *= rfpart(intery) * xGap;
                        c1.A *= fpart(intery) * xGap;
                        //c1 = Color.FromArgb((int)(rfpart(intery) * 255), Color);
                        //c2 = Color.FromArgb((int)(fpart(intery) * 255), Color);
                        DrawPixel(new Vec3(ipart(intery), x, z1), c0);
                        DrawPixel(new Vec3(ipart(intery) + 1, x, z1), c1);
                        //SetPixel(g, ipart(intery), x, c1);
                        //SetPixel(g, ipart(intery) + 1, x, c2);
                        intery += gradient;
                    }
                }
                else
                {
                    for (int x = (xPixel1 + 1); x <= xPixel2 - 1; x++)
                    {
                        ///here
                        float t = (x - xPixel1 - 1) / (float)(xPixel2 - xPixel1 - 2);
                        Vec4Color c = (1 - t) * color0 + t * color1;
                        c0 = c;
                        c1 = c;
                        c0.A *= rfpart(intery) * xGap;
                        c1.A *= fpart(intery) * xGap;

                        //c1 = Color.FromArgb((int)(rfpart(intery) * 255), Color);
                        //c2 = Color.FromArgb((int)(fpart(intery) * 255), Color);
                        DrawPixel(new Vec3(x, ipart(intery), z1), c0);
                        DrawPixel(new Vec3(x, ipart(intery) + 1, z1), c1);

                        //SetPixel(g, x, ipart(intery), c1);
                        //SetPixel(g, x, ipart(intery) + 1, c2);
                        intery += gradient;
                    }
                }
            }
            else
            {
                float m;
                if (to.X - from.X != 0) m = (to.Y - from.Y) / (to.X - from.X);
                else m = to.Y - from.Y;
                if (Math.Abs(m) >= 1)
                {
                    Vec3 start;
                    Vec3 end;
                    Vec4Color startColor, endColor;
                    if (from.Y < to.Y)
                    {
                        start = from;
                        end = to;
                        startColor = color0;
                        endColor = color1;
                    }
                    else
                    {
                        start = to;
                        end = from;
                        startColor = color1;
                        endColor = color0;
                    }


                    float x = start.X;
                    for (float y = start.Y; y <= end.Y; y++)
                    {
                        float traverseFraction = (end.Y - y) / (end.Y - start.Y);
                        float z = start.Z + (end.Z - start.Z) * traverseFraction;

                        Vec3 pixel = new Vec3(x, y, z);
                        Vec4Color finalColor = startColor * traverseFraction + endColor * (1 - traverseFraction);
                        DrawPixel(pixel, finalColor);
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
                        startColor = color0;
                        endColor = color1;
                    }
                    else
                    {
                        start = to;
                        end = from;
                        startColor = color1;
                        endColor = color0;
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
        }
        private void DrawLine(float fromX, float fromZ, Vec4Color color0, float toX, float toZ, Vec4Color color1, int y)
        {
            void swapInts(ref int o1, ref int o2)
            {
                int tmp = o1;
                o1 = o2;
                o2 = tmp;
            }
            void swapFloats(ref float o1, ref float o2)
            {
                float tmp = o1;
                o1 = o2;
                o2 = tmp;
            }
            void swapColors(ref Vec4Color o1, ref Vec4Color o2)
            {
                Vec4Color tmp = o1;
                o1 = o2;
                o2 = tmp;
            }
            int rFromX = (int)Math.Round(fromX);
            int rToX = (int)Math.Round(toX);
            
            if(rFromX > rToX)
            {
                swapInts(ref rFromX, ref rToX);
                swapFloats(ref fromZ, ref toZ);
                swapColors(ref color0, ref color1);
            }
            for (int x = rFromX; x <= rToX; x++)
            {
                Vec2 v = new Vec2(x, y);
                float t = (rFromX == rToX) ? 0.5f : (x - rFromX) / (float)(rToX - rFromX);
                Vec4Color c = (1 - t) * color0 + t * color1;
                float z = (1 - t) * fromZ + t * toZ;
                DrawPixel(v, z, c);
            }
        }
    }
}
