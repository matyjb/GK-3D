using SFML.Graphics;
using SFML.System;
using GK.Transforming;
using System;

namespace GK.DrawingView
{
    public sealed class Camera : Transformable3D
    {
        public static Camera Instance { get; } = new Camera();

        public Vector2f Resolution { get => new Vector2f(Width, Height); set { Width = value.X; Height = value.Y; } }
        public float Width { get => view.Size.X; set => view = new View(new FloatRect(-value / 2, -Height / 2, value, Height)); }
        public float Height { get => view.Size.Y; set => view = new View(new FloatRect(-Width / 2, -value / 2, Width, value)); }
        public View view { get; set; } = new View(new FloatRect(-400, -300, 800, 600));

        private float sdistance = 400;
        public float Sdistance { get=>sdistance; private set { sdistance = value; Origin = new Vector3f(0, 0, -value); } }

        public float FOVAngle { get => 2 * (float)Math.Atan(Width / 2 / Sdistance); set => Sdistance = Width / 2 / (float)Math.Tan(value / 2); }


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Camera()
        {
        }

        private Camera()
        {
            Origin = new Vector3f(0, 0, -sdistance);
            Position = new Vector3f(0, 0, -sdistance);
        }

    }
}
