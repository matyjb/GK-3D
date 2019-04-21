//using SFML.Graphics;
//using SFML.System;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SFML_Tesseract
//{
//    public sealed class Camera
//    {
//        private static readonly Camera instance = new Camera();
//        public static Camera Instance { get => instance; }

//        public Vector2f Resolution { get => new Vector2f(Width, Height); set { Width = value.X; Height = value.Y; } }
//        public float Width { get => view.Size.X; set => view = new View(new FloatRect(-value / 2, -Height / 2, value, Height)); }
//        public float Height { get => view.Size.Y; set => view = new View(new FloatRect(-Width / 2, -value / 2, Width, value)); }
//        public View view { get; private set; } = new View(new FloatRect(-400, -300, 800, 600));

//        public float Sdistance { get; private set; } = 400;

//        public float FOVAngle { get => 2 * (float)Math.Atan(Width / 2 / Sdistance); set => Sdistance = Width / 2 / (float)Math.Tan(value/2); }

//        public Vector3f Position { get; set; }


//        // Explicit static constructor to tell C# compiler
//        // not to mark type as beforefieldinit
//        static Camera()
//        {
//        }

//        private Camera()
//        {
//        }

//    }
//}
