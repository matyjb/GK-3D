using SFML.Graphics;
using SFML.System;
using GK.Drawables;
using GK.Interfaces;
using GK.Structs;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GK.DrawingView
{
    public sealed class Renderer : Drawable
    {
        public static Renderer Instance { get; } = new Renderer();
        static Renderer() { }
        private Renderer() { }

        public List<IDrawable3D> drawables = new List<IDrawable3D>()
        {
            //new Quad(new Vector3f(-1,0,0),new Vector3f(2,0,1),new Vector3f(2,1,1),new Vector3f(-1,4,0), Color.Blue) {Position=new Vector3f(0,0,200), Scale=new Vector3f(100,100,100)},
            //new Triangle(new Vector3f(0,0,0),new Vector3f(0,2,0),new Vector3f(1,0,1), Color.Red) {Scale=new Vector3f(100,100,100)},
            //new Cuboid(1,1,1,Color.Green) {Scale=new Vector3f(50,50,50), Position=new Vector3f(-70,0,0)},
            new Tank()
        };
        public static Vertex[] PerspectiveView(List<Vertex3D> shapeVertices, out List<float> scalingFactors)
        {
            scalingFactors = new List<float>();
            float camSDistance = Camera.Instance.Sdistance;
            List<Vertex> result = new List<Vertex>();
            for (int i = 0; i < shapeVertices.Count; i++)
            {
                var shapeVertex = shapeVertices[i];
                float scale = camSDistance / (shapeVertex.Position.Z + camSDistance);
                Vector3f s = new Vector3f(scale, scale, 0);
                scalingFactors.Add(scale);
                shapeVertex.Position.X *= s.X;
                shapeVertex.Position.Y *= -s.Y; //so Y axis is pointing up in 3d space
                shapeVertex.Position.Z *= s.Z;
                result.Add(new Vertex(new Vector2f(shapeVertex.Position.X, shapeVertex.Position.Y), shapeVertex.Color));
            }
            return result.ToArray();
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            List<List<Vertex3D>> shapes = new List<List<Vertex3D>>();
            //collecting all shapes
            foreach (var drawable in drawables)
            {
                shapes.AddRange(drawable.GetShapes());
            }

            //inverse z camerą
            for (int i = 0; i < shapes.Count; i++)
            {
                for (int j = 0; j < shapes[i].Count; j++)
                {
                    var v = shapes[i][j];
                    v.Position = Camera.Instance.InverseTransform.TransformPoint(v.Position);
                    shapes[i][j] = v;
                }
            }

            //draw
            PaintersAlgorithm.PerformDrawing(shapes, target, states);
            //lagi
            //Z_BuforAlgorithm.PerformDrawing(shapes, target, states);

        }
    }
    public static class PaintersAlgorithm
    {
        private static bool CzyWTrojkacie(Vector2f v, Vector2f v0, Vector2f v1, Vector2f v2)
        {
            Vector2f p = v - v2;
            Vector2f p0 = v0 - v2;
            Vector2f p1 = v1 - v2;
            float det = p0.Cross(p1);

            if (det == 0) return false;

            float t0 = p.Cross(p1) / det;
            float t1 = p0.Cross(p) / det;

            if (t0 < 0 || t1< 0 || t0+t1 > 1) return false;

            return true;
        }
        private static bool Test1(List<Vertex3D> a, List<Vertex3D> b)
        {
            float minAX = float.MaxValue, maxAX = float.MinValue, minBX = float.MaxValue, maxBX = float.MinValue;
            foreach (var vertA in a)
            {
                minAX = Math.Min(minAX, vertA.Position.X);
                maxAX = Math.Max(maxAX, vertA.Position.X);
            }
            foreach (var vertB in b)
            {
                minBX = Math.Min(minBX, vertB.Position.X);
                maxBX = Math.Max(maxBX, vertB.Position.X);
            }
            List<float> f = new List<float>() { minAX, maxAX, minBX, maxBX };
            f = f.OrderBy(i => i).ToList();
            if (f[0] == minAX && f[1] == maxAX) return true;
            if (f[0] == minBX && f[1] == maxBX) return true;
            return false;
        }
        private static bool Test2(List<Vertex3D> a, List<Vertex3D> b)
        {
            float minAY = float.MaxValue, maxAY = float.MinValue, minBY = float.MaxValue, maxBY = float.MinValue;
            foreach (var vertA in a)
            {
                minAY = Math.Min(minAY, vertA.Position.Y);
                maxAY = Math.Max(maxAY, vertA.Position.Y);
            }
            foreach (var vertB in b)
            {
                minBY = Math.Min(minBY, vertB.Position.Y);
                maxBY = Math.Max(maxBY, vertB.Position.Y);
            }
            List<float> f = new List<float>() { minAY, maxAY, minBY, maxBY };
            f = f.OrderBy(i => i).ToList();
            if (f[0] == minAY && f[1] == maxAY) return true;
            if (f[0] == minBY && f[1] == maxBY) return true;
            return false;
        }
        private static bool Test3(List<Vertex3D> a, List<Vertex3D> b)
        {
            //wyzanczenie rowanania plaszczyzny
            Vector3f v1 = a[1].Position - a[0].Position;
            Vector3f v2 = a[1].Position - a[2].Position;
            //wektor ortogonalny
            Vector3f d = new Vector3f(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.Z);
            float A = d.X;
            float B = d.Y;
            float C = d.Z;
            float D = -d.X * a[0].Position.X - d.Y * a[0].Position.Y - d.Z * a[0].Position.Z;
            for (int i = 0; i < b.Count; i++)
            {
                Vector3f vert = b[i].Position;
                if (A * vert.X + B * vert.Y + C * vert.Z + D >= 0) break;
                if (i == b.Count - 1) return true;
            }
            return false;
        }
        private static bool Test4(List<Vertex3D> a, List<Vertex3D> b)
        {
            //wyzanczenie rowanania plaszczyzny
            Vector3f v1 = b[1].Position - b[0].Position;
            Vector3f v2 = b[1].Position - b[2].Position;
            //wektor ortogonalny
            Vector3f d = v1.Cross(v2);
            float A = d.X;
            float B = d.Y;
            float C = d.Z;
            float D = -d.X * b[0].Position.X - d.Y * b[0].Position.Y - d.Z * b[0].Position.Z;
            for (int i = 0; i < a.Count; i++)
            {
                Vector3f vert = a[i].Position;
                if (A * vert.X + B * vert.Y + C * vert.Z + D >= 0) break;
                if (i == a.Count - 1) return true;
            }
            return false;
        }
        private static bool Test5(List<Vertex3D> a, List<Vertex3D> b)
        {
            //plaszczyzny musza by rozlaczne - jesli tak to return true
            List<float> t;
            Vertex[] shape2dA = Renderer.PerspectiveView(a, out t);
            Vertex[] shape2dB = Renderer.PerspectiveView(b, out t);
            //spr przecianania
            for (int i = 0; i < shape2dA.Length - 1; i++)
            {
                for (int j = 0; j < shape2dB.Length - 1; j++)
                {
                    Vector2f p1 = shape2dA[i].Position;
                    Vector2f p2 = shape2dA[i + 1].Position;
                    Vector2f p3 = shape2dB[j].Position;
                    Vector2f p4 = shape2dB[j + 1].Position;
                    float ilv1 = (p1 - p3).Cross(p4 - p3);
                    float ilv2 = (p2 - p3).Cross(p4 - p3);
                    float ilv3 = (p3 - p1).Cross(p2 - p1);
                    float ilv4 = (p4 - p1).Cross(p2 - p1);
                    if (ilv1 * ilv2 < 0 && ilv3 * ilv4 < 0) return true;
                    if (ilv1 == 0 && CzyNaOdcinku(p3, p4, p1)) return true;
                    if (ilv2 == 0 && CzyNaOdcinku(p3, p4, p2)) return true;
                    if (ilv3 == 0 && CzyNaOdcinku(p1, p2, p3)) return true;
                    if (ilv4 == 0 && CzyNaOdcinku(p1, p2, p4)) return true;
                }
            }
            //spr czy wszytkie wirzcholi sa na zewnatrz
            foreach (var vert in shape2dB)
            {
                bool x = CzyWTrojkacie(vert.Position, shape2dA[0].Position, shape2dA[1].Position, shape2dA[2].Position);
                //Console.WriteLine(x);
                if (x) return true;
            }
            foreach (var vert in shape2dA)
            {
                bool x = CzyWTrojkacie(vert.Position, shape2dB[0].Position, shape2dB[1].Position, shape2dB[2].Position);
                //Console.WriteLine(x);
                if (x) return true;
            }

            return false;
        }
        private static bool CzyNaOdcinku(Vector2f p1, Vector2f p2, Vector2f p3)
        {
            float minx = Math.Min(p1.X, p2.X), maxx = Math.Max(p1.X, p2.X);
            float miny = Math.Min(p1.Y, p2.Y), maxy = Math.Max(p1.Y, p2.Y);
            if (minx <= p3.X && p3.X <= maxx && miny <= p3.Y && p3.Y <= maxy) return true;
            return false;
        }
        private static void PreSortShapes(List<List<Vertex3D>> shapes)
        {
            //bubble sort XD
            for (int i = 0; i < shapes.Count-1; i++)
            {
                for (int j = i+1; j < shapes.Count-1; j++)
                {
                    float maxZA = float.MinValue;
                    foreach (var itemA in shapes[i]) maxZA = Math.Max(maxZA, (float)Math.Sqrt(itemA.Position.X * itemA.Position.X + itemA.Position.Y * itemA.Position.Y + (itemA.Position.Z + Camera.Instance.Sdistance) * (itemA.Position.Z + Camera.Instance.Sdistance)));
                    float maxZB = float.MinValue;
                    foreach (var itemB in shapes[j]) maxZB = Math.Max(maxZB, (float)Math.Sqrt(itemB.Position.X * itemB.Position.X + itemB.Position.Y * itemB.Position.Y + (itemB.Position.Z + Camera.Instance.Sdistance) * (itemB.Position.Z + Camera.Instance.Sdistance)));

                    if (maxZA >= maxZB)
                    {
                        List<Vertex3D> tmp = shapes[i];
                        shapes[i] = shapes[j];
                        shapes[j] = tmp;
                    }
                }
            }
            //shapes.Sort((a, b) =>
            //{
            //    //float centerZA = 0;
            //    //foreach (var itemA in a) centerZA += itemA.Position.Z;
            //    //centerZA /= a.Count;
            //    //float centerZB = 0;
            //    //foreach (var itemB in b) centerZB += itemB.Position.Z;
            //    //centerZB /= b.Count;

            //    //if (centerZA >= centerZB) return -1;
            //    //return 1;
            //    if (a.Equals(b)) return 0;
            //    float maxZA = float.MinValue;
            //    foreach (var itemA in a) maxZA = Math.Max(maxZA, (float)Math.Sqrt(itemA.Position.X * itemA.Position.X + itemA.Position.Y * itemA.Position.Y + (itemA.Position.Z + Camera.Instance.Sdistance) * (itemA.Position.Z + Camera.Instance.Sdistance)));
            //    float maxZB = float.MinValue;
            //    foreach (var itemB in b) maxZB = Math.Max(maxZB, (float)Math.Sqrt(itemB.Position.X * itemB.Position.X + itemB.Position.Y * itemB.Position.Y + (itemB.Position.Z + Camera.Instance.Sdistance) * (itemB.Position.Z + Camera.Instance.Sdistance)));

            //    if (maxZA >= maxZB) return -1;
            //    return 1;
            //});
        }
        private static void SortShapes(List<List<Vertex3D>> shapes)
        {
            //sortowanie parami z 5 testami
            shapes.Sort((a, b) =>
            {
                //TEST1 - jeżeli przedziały [xmin;xmax] są rozłączne to pozytywny
                if (Test1(a, b)) return 1;
                //TEST2 - jeżeli przedziały [ymin;ymax] są rozłączne to pozytywny
                if (Test2(a, b)) return 1;
                //TEST3
                if (Test3(a, b)) return 1;
                //TEST4
                if (Test4(a, b)) return 1;
                //TEST5
                if (Test5(a, b)) return 1;
                //wynik negatywny - zamien
                return -1;
            });
        }
        public static void PerformDrawing(List<List<Vertex3D>> shapes, RenderTarget target, RenderStates states)
        {
            //sortowanie wstepne (najwieksze center Z jako pierwsze)
            PreSortShapes(shapes);
            SortShapes(shapes);
            
            //rzutowanie na 2d i rysowanie na target
            foreach (var shape in shapes)
            {
                target.Draw(Renderer.PerspectiveView(shape, out var t), PrimitiveType.Triangles, states);
                // TODO: setpixel wypelnianie trojkata (jak punkt jest przed kamera czyli z<0 to nie malowac[zalatwi to przycinanie])
            }
        }
    }

    public static class Z_BuforAlgorithm
    {
        private static bool SprawdzZakres(double val, double min, double max)
        {
            return val >= min && val <= max;
        }
        public static void PerformDrawing(List<List<Vertex3D>> shapes, RenderTarget target, RenderStates states)
        {
            Vertex[] v = new Vertex[] { new Vertex() };
            for (int x = -(int)Camera.Instance.Width / 2; x < Camera.Instance.Width/2; x++)
            {
                for (int y = -(int)Camera.Instance.Height / 2; y < Camera.Instance.Height / 2; y++)
                {
                    float minZ = float.MaxValue;
                    Color color = Color.Black;
                    Vector2f p = new Vector2f(x, y);
                    for (int i = 0; i < shapes.Count; i++)
                    {
                        Vertex[] shape2D = Renderer.PerspectiveView(shapes[i], out var s);
                        //czy pkt jest na trojkacie
                        Vector2f v1 = shape2D[1].Position - shape2D[0].Position;
                        Vector2f v2 = shape2D[2].Position - shape2D[0].Position;
                        Vector2f v3 = p - shape2D[0].Position;
                        float dd = v2.Cross(v3);
                        float w1 = p.X * (v2.Y - v3.Y) + p.Y * (v3.X - v2.X) + v2.Cross(v3);
                        float w2 = p.Cross(v3);
                        float w3 = p.Cross(v2);
                        if (!(SprawdzZakres(w1, 0, dd) && SprawdzZakres(w2, 0, dd) && SprawdzZakres(w3, 0, dd))) continue;
                        //
                        List<float> distances = new List<float>();
                        float sumAllDistances = 0;
                        for (int j = 0; j < shape2D.Length; j++)
                        {
                            float d = p.Distance(shape2D[j].Position);
                            sumAllDistances += d;
                            distances.Add(d);
                        }
                        float finZ = 0;
                        for (int j = 0; j < distances.Count; j++)
                        {
                            float wsp = (sumAllDistances - distances[j]) / sumAllDistances;
                            finZ += (1 - wsp) * 1 / s[j];
                        }
                        if (minZ > finZ && finZ >= 0)
                        {
                            minZ = finZ;
                            color = shapes[i][0].Color; //TODO: mieszanie kolorow wierzcholkow
                        }
                    }
                    v[0].Position.X = x;
                    v[0].Position.Y = y;
                    v[0].Color = color;
                    //target.Draw(v, PrimitiveType.Points, states);
                }
            }
        }
    }
}
