using SFML.Graphics;

namespace GK.Math3D
{
    public struct Vertex3
    {
        public Vec3 Position { get; set; }
        public Vec4 Color { get; set; }

        public Vertex3(Vec3 position, Vec4 color)
        {
            Position = position;
            Color = color;
        }
    }
}
