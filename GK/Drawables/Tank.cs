using GK.Interfaces;
using GK.Structs;
using GK.Transforming;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.Drawables
{
    class Tank : Transformable3D, IDrawable3D
    {
        public List<List<Vertex3D>> GetShapes()
        {
            Transform3D t = Transform;
            Cuboid c0 = new Cuboid(200, 100, 500, Color.Green)
            {
                Rotation = new Vector3f(0, (float)Math.PI / 6f,0),
                Origin = new Vector3f(100, 100 / 2f, 500 / 2f),
                ParentTransform = t,
            };
            Cuboid c1 = new Cuboid(100, 100 / 2f, 100, Color.Blue)
            {
                Rotation = new Vector3f(0, (float)Math.PI / 2f,0),
                Origin = new Vector3f(100 / 2f, 100 / 4f, 100 / 2f),
                //ParentTransform = c0.Transform,
                ParentTransform = t,
                Position = new Vector3f(0, 300/4f, 0)
            };
            Cuboid c2 = new Cuboid(100/4f, 100/4f,300, Color.Red)
            {
                Rotation = new Vector3f(0, 0, (float)Math.PI / 4f),
                Origin = new Vector3f(100 / 8f, 100 / 8f, 300/2f),
                //ParentTransform = c1.Transform,
                ParentTransform = t,
                Position = new Vector3f(0, 300 / 4f, 200)
            };
            List<List<Vertex3D>> l = new List<List<Vertex3D>>();
            l.AddRange(c0.GetShapes());
            l.AddRange(c1.GetShapes());
            l.AddRange(c2.GetShapes());
            return l;
        }
    }
}
