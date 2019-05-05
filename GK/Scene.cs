using GK.Drawables;
using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System.Collections.Generic;

namespace GK
{
    public class Scene : Drawable
    {
        public Camera Camera { get; } = new Camera();
        public List<IDrawable3D> Drawables = new List<IDrawable3D>()
        {
            new Triangle(new Vector3Df(0,0,1),new Vector3Df(1,0,1),new Vector3Df(0,2,1), Color.Blue),
        };

        public void Draw(RenderTarget target, RenderStates states)
        {
            /*
             1. uzyskać od wszystkich Drawables trojkaty tzn List<Triangle3Df>
             2. wpakować wszsytkie do jednej listy
             3. wszystkie trojkaty z listy pomnozyc o tak:
                Camera.ProjectionMatrix*Camera.InverseTransform*Triangle
                3a. unormowac po w
             //(malarski (ale głupi))
             4. posortować po Z
             5. dla kazdego trojkata zrzutować Vertexy3D na Vertex i wstawić do Vertex[]
             6. kazdy vector jeszcze trzeba pomnozyc przez [width, -Height]
             //zbufor

             //
             6. target.draw
             */
            List<Triangle3Df> tris = new List<Triangle3Df>();
            foreach (var drawable in Drawables)
            {
                tris.AddRange(drawable.GetTriangle3Dfs());
            }
            List<Triangle3Df> trisProjected = new List<Triangle3Df>(tris);
            Transform3D proj = Camera.ProjectionMatrix * Camera.InverseTransform;
            for (int i = 0; i < tris.Count; i++)
            {
                var t = trisProjected[i];
                t = proj * t;
                for (int j = 0; j < 3; j++)
                {
                    var tv = t[j];
                    tv.Position = tv.Position.NormalW1();
                    tv.Position.X *= -0.5f * Camera.Width;
                    tv.Position.Y *= 0.5f * Camera.Height;
                    tv.Position.Z *= -1;
                    t[j] = tv;
                }
                trisProjected[i] = t;
            }
            //trisProjected.Sort((a, b) =>
            //{

            //})
            foreach (var tri in trisProjected)
            {
            System.Console.WriteLine(tri.v0.Position.Z);
                Vertex[] verticesArray = new Vertex[3]
                {
                    (Vertex)tri.v0,
                    (Vertex)tri.v1,
                    (Vertex)tri.v2,
                };
                target.Draw(verticesArray, PrimitiveType.Triangles, states);

            }
        }

    }
}
