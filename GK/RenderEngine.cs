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
        public Mesh Mesh { get; set; }
        public Transform GlobalTransform { get; set; }
        public Transform Projection { get; set; }
        public RenderWindow Window { get; set; }

        private float[,] ZBuffer { get; set; }
        private Color[,] Bitmap { get; set; }



        private Vec3 GetLineIntersectionWithPlane(Vec3 planePoint, Vec3 planeNormal, Vec3 lineStart, Vec3 lineEnd)
        {
            // to be sure it is normal
            planeNormal = planeNormal.Normal();

            float plane_d = -planeNormal.Dot(planePoint);
            float ad = lineStart.Dot(planeNormal);
            float bd = lineEnd.Dot(planeNormal);
            float t = (-plane_d - ad) / (bd - ad);
            Vec3 lineStartToEnd = lineEnd - lineStart;
            Vec3 lineToIntersect = lineStartToEnd * t;
            return lineStart + lineToIntersect;
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
            Vec3[] insidePoints = new Vec3[3]; int nInsidePointCount = 0;
            Vec3[] outsidePoints = new Vec3[3]; int nOutsidePointCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = dist(tri[0]);
            float d1 = dist(tri[1]);
            float d2 = dist(tri[2]);

            if (d0 >= 0) { insidePoints[nInsidePointCount++] = tri[0]; }
            else { outsidePoints[nOutsidePointCount++] = tri[0]; }
            if (d1 >= 0) { insidePoints[nInsidePointCount++] = tri[1]; }
            else { outsidePoints[nOutsidePointCount++] = tri[1]; }
            if (d2 >= 0) { insidePoints[nInsidePointCount++] = tri[2]; }
            else { outsidePoints[nOutsidePointCount++] = tri[2]; }

            if (nInsidePointCount == 0) return new List<Triangle>();
            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                Vec3 v0 = insidePoints[0];
                Vec3 v1 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[0]);
                Vec3 v2 = GetLineIntersectionWithPlane(planePoint, planeNormal, v0, outsidePoints[1]);

                Triangle t = new Triangle(v0, v1, v2, tri.Color);
                //swap so normalvectors sign stays the same
                if (tri.NormalVector.Z * t.NormalVector.Z < 0)
                {
                    Vec3 tmp = t[1];
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
                Vec3 v00 = insidePoints[0];
                Vec3 v01 = insidePoints[1];
                Vec3 v02 = GetLineIntersectionWithPlane(planePoint, planeNormal, v00, outsidePoints[0]);

                Vec3 v10 = insidePoints[1];
                Vec3 v11 = v02;
                Vec3 v12 = GetLineIntersectionWithPlane(planePoint, planeNormal, v10, outsidePoints[0]);

                Triangle t1 = new Triangle(v00, v01, v02, tri.Color);
                Triangle t2 = new Triangle(v10, v12, v11, tri.Color);
                //swap so normalvectors sign stays the same
                if (tri.NormalVector.Z * t1.NormalVector.Z < 0)
                {
                    Vec3 tmp = t1[1];
                    t1[1] = t1[2];
                    t1[2] = tmp;
                }
                if (tri.NormalVector.Z * t2.NormalVector.Z < 0)
                {
                    Vec3 tmp = t2[1];
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
            Mesh transformed = new Mesh();
            foreach (Triangle triangle in Mesh)
            {
                Vec3 v0 = GlobalTransform * triangle[0];
                Vec3 v1 = GlobalTransform * triangle[1];
                Vec3 v2 = GlobalTransform * triangle[2];
                transformed.Add(new Triangle(v0, v1, v2, triangle.Color));
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
            //

            Mesh projected = new Mesh();
            Transform intoViewMoveAndScale = Transform.Identity.Translate(new Vec3(1, 1, 0)).Scale(new Vec3(Window.Size.X / 2, Window.Size.Y / 2, 1));
            Transform final = intoViewMoveAndScale * Projection;
            foreach (Triangle triangle in clipped)
            {
                //only if visible
                if (triangle.NormalVector.Dot(triangle[0]) < 0)
                {
                    //project and move, and scale into view
                    Vec3 v0 = final * triangle[0];
                    Vec3 v1 = final * triangle[1];
                    Vec3 v2 = final * triangle[2];
                    projected.Add(new Triangle(v0, v1, v2, triangle.Color));
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
                    while(nNewTriangles > 0)
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
                                clippedTriangles = ClipAgainstPlane(new Vec3(0,Camera.Instance.Height-1,0), new Vec3(0, -1, 0), t);
                                break;
                            case 2:
                                clippedTriangles = ClipAgainstPlane(new Vec3(), new Vec3(1, 0, 0), t);
                                break;
                            case 3:
                                clippedTriangles = ClipAgainstPlane(new Vec3(Camera.Instance.Width-1,0,0), new Vec3(-1, 0, 0), t);
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
            //init frame
            ZBuffer = new float[Window.Size.X, Window.Size.Y];
            Bitmap = new Color[Window.Size.X, Window.Size.Y];


            foreach (Triangle triangle in clipped2D)
            {
                ////with fill
                //VertexArray vertexArray = new VertexArray(PrimitiveType.Triangles);
                //vertexArray.Append(new Vertex((Vector2f)triangle[0], triangle.Color));
                //vertexArray.Append(new Vertex((Vector2f)triangle[1], triangle.Color));
                //vertexArray.Append(new Vertex((Vector2f)triangle[2], triangle.Color));
                //target.Draw(vertexArray);
                ////with wireframe
                //VertexArray vertexArrayWire = new VertexArray(PrimitiveType.LineStrip);
                //vertexArrayWire.Append(new Vertex((Vector2f)triangle[0], Color.Magenta));
                //vertexArrayWire.Append(new Vertex((Vector2f)triangle[1], Color.Magenta));
                //vertexArrayWire.Append(new Vertex((Vector2f)triangle[2], Color.Magenta));
                //vertexArrayWire.Append(new Vertex((Vector2f)triangle[0], Color.Magenta));
                //target.Draw(vertexArrayWire);

                //test zbuffer wireframe
                DrawTriangle(triangle, PrimitiveType.LineStrip);
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
        private void DrawTriangle(Triangle triangle, PrimitiveType primitiveType)
        {
            if(primitiveType == PrimitiveType.Triangles)
            {
                
            }
            else if (primitiveType == PrimitiveType.LineStrip)
            {
                DrawLine(triangle[0], triangle.Color, triangle[1], triangle.Color);
                DrawLine(triangle[1], triangle.Color, triangle[2], triangle.Color);
                DrawLine(triangle[2], triangle.Color, triangle[0], triangle.Color);
            }
        }

        private void DrawPixel(Vec3 pixel, Color color)
        {
            int screenX = (int)pixel.X;
            int screenY = (int)pixel.Y;
            if (pixel.Z < ZBuffer[screenX, screenY])
            {
                ZBuffer[screenX, screenY] = pixel.Z;
                Bitmap[screenX, screenY] = color;
            }
        }
        private void DrawLine(Vec3 from, Color color1, Vec3 to, Color color2)
        {
            float m;
            if (to.X - from.X != 0) m = (to.Y - from.Y) / (to.X - from.X);
            else m = to.Y - from.Y;
            if (Math.Abs(m) >= 1)
            {
                Vec3 start;
                Vec3 end;
                Color startColor, endColor;
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
                    Color finalColor = Mix(startColor, endColor, traverseFraction);
                    DrawPixel(pixel,finalColor);
                    x += (to.X - from.X != 0) ? 1 / m : 0;
                }
            }
            else
            {
                Vec3 start;
                Vec3 end;
                Color startColor, endColor;
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
                    Color finalColor = Mix(startColor, endColor, traverseFraction);
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
