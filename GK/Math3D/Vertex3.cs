using SFML.Graphics;

namespace GK.Math3D
{
    public struct Vertex3
    {
        public Vec3 Position { get; set; }
        public Color Color { get; set; }

        public Vertex3(Vec3 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
