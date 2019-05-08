using GK.Interfaces;
using GK.Math3D;
using SFML.Graphics;
using System.Collections.Generic;

namespace GK.Drawables
{
    public class Triangle : Transformable3D, IDrawable3D
    {
        public Vector3Df v0;
        public Vector3Df v1;
        public Vector3Df v2;
        public Color Color;

        public Triangle(Vector3Df v0, Vector3Df v1, Vector3Df v2, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            Color = color;
        }
        public List<Triangle3Df> GetTriangle3Dfs()
        {
            Vertex3Df vert0 = new Vertex3Df(Transform * v0, Color);
            Vertex3Df vert1 = new Vertex3Df(Transform * v1, Color);
            Vertex3Df vert2 = new Vertex3Df(Transform * v2, Color);
            return new List<Triangle3Df>()
            {
                new Triangle3Df(vert0,vert1,vert2),
            };
        }
    }
}
