using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System.Collections.Generic;

namespace GK.Drawables
{
    public class Cuboid : Transformable3D, IDrawable3D
    {
        public Vector3Df size;
        public Color Color;

        public Cuboid(Vector3Df size, Color color)
        {
            this.size = size;
            Color = color;
        }
        public List<Triangle3Df> GetTriangle3Dfs()
        {
            Vector3Df v0 = new Vector3Df(0, 0, 0);
            Vector3Df v1 = new Vector3Df(0, 0, size.Z);
            Vector3Df v2 = new Vector3Df(0, size.Y, 0);
            Vector3Df v3 = new Vector3Df(0, size.Y, size.Z);
            Vector3Df v4 = new Vector3Df(size.X, 0, 0);
            Vector3Df v5 = new Vector3Df(size.X, 0, size.Z);
            Vector3Df v6 = new Vector3Df(size.X, size.Y, 0);
            Vector3Df v7 = new Vector3Df(size.X, size.Y, size.Z);
            Quad q0 = new Quad(v0, v4, v5, v1, Color) { ParentTransform = Transform };
            Quad q1 = new Quad(v2, v6, v7, v3, Color) { ParentTransform = Transform };
            Quad q2 = new Quad(v0, v2, v3, v1, Color) { ParentTransform = Transform };
            Quad q3 = new Quad(v4, v6, v7, v5, Color) { ParentTransform = Transform };
            Quad q4 = new Quad(v0, v4, v6, v2, Color) { ParentTransform = Transform };
            Quad q5 = new Quad(v1, v5, v7, v3, Color) { ParentTransform = Transform };
            List<Triangle3Df> result = new List<Triangle3Df>();
            result.AddRange(q0.GetTriangle3Dfs());
            result.AddRange(q1.GetTriangle3Dfs());
            result.AddRange(q2.GetTriangle3Dfs());
            result.AddRange(q3.GetTriangle3Dfs());
            result.AddRange(q4.GetTriangle3Dfs());
            result.AddRange(q5.GetTriangle3Dfs());
            return result;
        }
    }
}
