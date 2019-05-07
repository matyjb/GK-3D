using GK.Drawables;
using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GK
{
    public class Scene : Drawable
    {
        public Camera Camera { get; } = new Camera();
        public Frame RenderFrame { get; set; } = new Frame(800, 600);
        public List<IDrawable3D> Drawables = new List<IDrawable3D>()
        {
            new Triangle(new Vector3Df(0,0,0),new Vector3Df(1,0,0),new Vector3Df(0,2,0), Color.Blue){Position=new Vector3Df(0,0,1) },
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
            //collecting triangles
            List<Triangle3Df> tris = new List<Triangle3Df>();
            foreach (var drawable in Drawables)
            {
                tris.AddRange(drawable.GetTriangle3Dfs());
            }
            //projecting all to the screen
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

            //drawing
            int halfwidth = RenderFrame.Width / 2;
            int halfhight = RenderFrame.Height / 2;

            Parallel.For(0, RenderFrame.Lenght, i =>
            {
                int pixX = i%RenderFrame.Width;
                int pixY = i/RenderFrame.Width;
                float spaceX = pixX - halfwidth;
                float spaceY = pixY - halfhight;
                float minZ = float.MaxValue;
                foreach (var t in trisProjected)
                {
                    float newZ = t.GetZ(spaceX, spaceY);
                    if (newZ > 1 && newZ < minZ)
                    {
                        minZ = newZ;
                        RenderFrame.SetPixel(pixX, pixY, Color.Red);
                    }
                }
            });

            //foreach (var tri in trisProjected)
            //{
            //    Vertex[] verticesArray = new Vertex[3]
            //    {
            //        (Vertex)tri.v0,
            //        (Vertex)tri.v1,
            //        (Vertex)tri.v2,
            //    };
            //    target.Draw(verticesArray, PrimitiveType.Triangles, states);
            //}


            target.Draw(RenderFrame, states);
        }

    }
}
