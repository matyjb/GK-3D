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
            new Quad(new Vector3f(-1,0,0),new Vector3f(2,0,1),new Vector3f(2,1,1),new Vector3f(-1,4,0), Color.Blue) {Position=new Vector3f(0,0,200), Scale=new Vector3f(100,100,100)},
            new Triangle(new Vector3f(0,0,0),new Vector3f(0,2,0),new Vector3f(1,0,1), Color.Red) {Scale=new Vector3f(100,100,100)},
            new Cuboid(1,1,1,Color.Green) {Scale=new Vector3f(50,50,50), Position=new Vector3f(-70,0,0)}
        };
        public static Vertex[] PerspectiveView(List<Vertex3D> shapeVertices)
        {
            float camSDistance = Camera.Instance.Sdistance;
            List<Vertex> result = new List<Vertex>();
            for (int i = 0; i < shapeVertices.Count; i++)
            {
                var shapeVertex = shapeVertices[i];
                Vector3f s = new Vector3f(camSDistance / (shapeVertex.Position.Z + camSDistance), camSDistance / (shapeVertex.Position.Z + camSDistance), 0);
                shapeVertex.Position.X *= s.X;
                shapeVertex.Position.Y *= - s.Y; //so Y axis is pointing up in 3d space
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
            //sortowanie*
            //sortowanie wstepne (najwieksze center Z jako pierwsze)
            shapes.Sort((a, b) =>
            {
            //float centerZA = 0;
            //foreach (var itemA in a) centerZA += itemA.Position.Z;
            //centerZA /= a.Count;
            //float centerZB = 0;
            //foreach (var itemB in b) centerZB += itemB.Position.Z;
            //centerZB /= b.Count;

            //if (centerZA >= centerZB) return -1;
            //return 1;
                float maxZA = float.MinValue;
                foreach (var itemA in a) maxZA = Math.Max(maxZA, (float)Math.Sqrt(itemA.Position.X * itemA.Position.X + itemA.Position.Y * itemA.Position.Y + (itemA.Position.Z + Camera.Instance.Sdistance) * (itemA.Position.Z + Camera.Instance.Sdistance)));
                float maxZB = float.MinValue;
                foreach (var itemB in b) maxZB = Math.Max(maxZB, (float)Math.Sqrt(itemB.Position.X * itemB.Position.X + itemB.Position.Y * itemB.Position.Y + (itemB.Position.Z + Camera.Instance.Sdistance) * (itemB.Position.Z + Camera.Instance.Sdistance)));

                if (maxZA >= maxZB) return -1;
                return 1;
            });
            //sortowanie parami z 5 testami
            shapes.Sort((a, b) =>
            {
                //TEST1 - jeżeli przedziały [xmin;xmax] są rozłączne to pozytywny
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
                if(f[0] == minAX && f[1] == maxAX) return -1;
                if(f[0] == minBX && f[1] == maxBX) return -1;

                //TEST2 - jeżeli przedziały [ymin;ymax] są rozłączne to pozytywny
                float minAY = float.MaxValue, maxAY = float.MinValue, minBY = float.MaxValue, maxBY = float.MinValue;
                foreach (var vertA in a)
                {
                    minAY = Math.Min(minAY, vertA.Position.Y);
                    maxAY = Math.Max(maxAY, vertA.Position.Y);
                }
                foreach (var vertB in b)
                {
                    minBY = Math.Min(minAY, vertB.Position.Y);
                    maxBY = Math.Max(maxAY, vertB.Position.Y);
                }
                f = new List<float>() { minAX, maxAY, minBY, maxBY };
                f = f.OrderBy(i => i).ToList();
                if (f[0] == minAY && f[1] == maxAY) return -1;
                if (f[0] == minBY && f[1] == maxBY) return -1;

                //TEST3
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
                    if (i == b.Count - 1) return -1;
                }

                //TEST4
                //wyzanczenie rowanania plaszczyzny
                v1 = b[1].Position - b[0].Position;
                v2 = b[1].Position - b[2].Position;
                //wektor ortogonalny
                d = new Vector3f(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.Z);
                A = d.X;
                B = d.Y;
                C = d.Z;
                D = -d.X * b[0].Position.X - d.Y * b[0].Position.Y - d.Z * b[0].Position.Z;
                for (int i = 0; i < a.Count; i++)
                {
                    Vector3f vert = a[i].Position;
                    if (A * vert.X + B * vert.Y + C * vert.Z + D >= 0) break;
                    if (i == a.Count - 1) return -1;
                }

                //TEST5


                //wynik negatywny - zamien
                return 1;
            });



            //rzutowanie na 2d i rysowanie na target
            foreach (var shape in shapes)
            {
                target.Draw(PerspectiveView(shape), PrimitiveType.Triangles, states);
                // TODO: setpixel wypelnianie trojkata (jak punkt jest przed kamera czyli z<0 to nie malowac[zalatwi to przycinanie])
            }

        }
    }
}
