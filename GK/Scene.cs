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
            new Triangle(new Vector3Df(-1,-1,0),new Vector3Df(3,1,1),new Vector3Df(2,3,0), Color.Green){Position=new Vector3Df(0,0,2) },
            new Quad(new Vector3Df(0,1,0),new Vector3Df(2,0.5f,0),new Vector3Df(3,0.7f,3),new Vector3Df(-1,1.5f,2.5f), Color.Red){Position=new Vector3Df(0,0,1) },
            new Cuboid(new Vector3Df(3,1,2), Color.White){Position=new Vector3Df(0,-3,1) },
        };

        public void Draw(RenderTarget target, RenderStates states)
        {
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
                    var tmpW = tv.Position.W; //to save sign
                    tv.Position = tv.Position.NormalW1();
                    //scale to screen dimensions
                    tv.Position.X *= Camera.Width / 2;
                    tv.Position.Y *= Camera.Height / 2;

                    //if vector was in front of the camera flip axis
                    if (tmpW < 0)
                    {
                        tv.Position.X *= -1;
                        tv.Position.Y *= -1;
                        tv.Position.Z *= -1;
                    }
                    //Y axis is pointing up in 3d space but down on 2d screen so flip Y-axis
                    tv.Position.Y *= -1;

                    t[j] = tv;
                }
                trisProjected[i] = t;
            }

            //drawing - z buffer
            int halfwidth = RenderFrame.Width / 2;
            int halfhight = RenderFrame.Height / 2;
            Parallel.For(0, RenderFrame.Lenght, i =>
            {
                int pixX = i%RenderFrame.Width;
                int pixY = i/RenderFrame.Width;
                float spaceX = pixX - halfwidth;
                float spaceY = pixY - halfhight;
                float maxZ = float.MinValue;
                foreach (var t in trisProjected)
                {
                    float newZ = t.GetZ(spaceX, spaceY);
                    if (newZ > 1/Camera.fNear && newZ > maxZ)
                    {
                        maxZ = newZ;
                        RenderFrame.SetPixel(pixX, pixY, t.v0.Color);
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
