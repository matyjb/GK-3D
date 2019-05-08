using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System.Collections.Generic;

namespace GK.Drawables
{
    public class Quad : Transformable3D, IDrawable3D
    {
        public Vector3Df v0;
        public Vector3Df v1;
        public Vector3Df v2;
        public Vector3Df v3;
        public Color Color;

        public Quad(Vector3Df v0, Vector3Df v1, Vector3Df v2, Vector3Df v3, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            Color = color;
        }
        public List<Triangle3Df> GetTriangle3Dfs()
        {
            Triangle t0 = new Triangle(v0, v1, v2, Color) { ParentTransform = Transform };
            Triangle t1 = new Triangle(v0, v2, v3, Color) { ParentTransform = Transform };
            List<Triangle3Df> result = new List<Triangle3Df>();
            result.AddRange(t0.GetTriangle3Dfs());
            result.AddRange(t1.GetTriangle3Dfs());
            return result;
        }
    }
}
