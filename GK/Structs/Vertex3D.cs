using SFML.Graphics;
using SFML.System;

namespace GK.Structs
{
    public struct Vertex3D
    {
        public Vector3f Position;
        public Color Color;
        public Vertex3D(Vector3f position) { Position = position; Color = Color.White; }
        public Vertex3D(Vector3f position, Color color) { Position = position; Color = color; }
    }
}
