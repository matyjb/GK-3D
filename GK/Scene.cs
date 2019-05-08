using GK.Drawables;
using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using SFML.System;
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
            Console.WriteLine(trisProjected[0].GetZ(0, 0));
            //int halfwidth = RenderFrame.Width / 2;
            //int halfhight = RenderFrame.Height / 2;
            //Parallel.For(0, RenderFrame.Lenght, i =>
            //{
            //    int pixX = i%RenderFrame.Width;
            //    int pixY = i/RenderFrame.Width;
            //    float spaceX = pixX - halfwidth;
            //    float spaceY = pixY - halfhight;
            //    float maxZ = float.MinValue;
            //    foreach (var t in trisProjected)
            //    {

            //        float newZ = t.GetZ(spaceX, spaceY);

            //        if (newZ > 1/Camera.fNear && newZ > maxZ)
            //        {
            //            maxZ = newZ;
            //            RenderFrame.SetPixel(pixX, pixY, t.v0.Color);
            //        }
            //    }
            //});

            RenderTexture zBufferTexture = new RenderTexture((uint)Camera.Width, (uint)Camera.Height);
            Vector2f cameracenter = new Vector2f(Camera.Width / 2, Camera.Height / 2);
            foreach (var t in trisProjected)
            {
                //TODO: funkcja mapująca
                byte v0ZColor = (byte)Math.Abs(t.v0.Position.Z*2.5f);
                byte v1ZColor = (byte)Math.Abs(t.v1.Position.Z*2.5f);
                byte v2ZColor = (byte)Math.Abs(t.v2.Position.Z*2.5f);
                var v0 = (Vector2f)t.v0.Position;
                var v1 = (Vector2f)t.v1.Position;
                var v2 = (Vector2f)t.v2.Position;
                v0.Y *= -1;
                v1.Y *= -1;
                v2.Y *= -1;
                v0 += cameracenter;
                v1 += cameracenter;
                v2 += cameracenter;
                Vertex[] v = new Vertex[]
                {
                    new Vertex(v0,new Color(v0ZColor,v0ZColor,v0ZColor)),
                    new Vertex(v1,new Color(v1ZColor,v1ZColor,v1ZColor)),
                    new Vertex(v2,new Color(v2ZColor,v2ZColor,v2ZColor)),
                };
                //TODO: shader
                zBufferTexture.Draw(v, PrimitiveType.Triangles);

            }
            Sprite sprite = new Sprite(zBufferTexture.Texture) { Position = -cameracenter };
            target.Draw(sprite);
            sprite.Dispose();
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


            //target.Draw(RenderFrame, states);
        }

    }
}
