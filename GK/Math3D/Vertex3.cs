using SFML.Graphics;

namespace GK.Math3D
{
    public struct Vertex3
    {
        public Vec3 Position { get; set; }
        public Vec4Color Color { get; set; }

        public Vertex3(Vec3 position, Vec4Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
