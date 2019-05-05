using SFML.Graphics;
using SFML.System;

namespace GK.Math3D
{
    public struct Vertex3Df
    {
        public Vector3Df Position;
        public Color Color;
        public Vertex3Df(Vector3Df position) { Position = position; Color = Color.White; }
        public Vertex3Df(Vector3Df position, Color color) { Position = position; Color = color; }

        public static explicit operator Vertex(Vertex3Df from)
        {
            return new Vertex((Vector2f)from.Position,from.Color);
        }
    }
}
