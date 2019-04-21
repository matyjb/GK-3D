using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Tesseract
{
    class Program
    {
        public static Camera c = Camera.Instance;
        public static RenderWindow window = new RenderWindow(new VideoMode((uint)c.Width, (uint)c.Height), "lmao");
        public static RenderEngine renEngine = RenderEngine.Instance;
        //public static Cube3D cube = new Cube3D() {Position = new Vector3f(-50,-50,100) };
        public static Plane3D plane = new Plane3D()
        {
            Position = new Vector3f(-50, -50, 100),
            Rotation =new Vector3f(30 * (float)Math.PI / 180, 30 * (float)Math.PI / 180, 30 * (float)Math.PI / 180),
            FillColor = Color.Red,
        };
        public static Time deltaTime;

        public static float cameraStepsPerSec = 100;

        static void Main(string[] args)
        {
            Clock deltaClock = new Clock();
            deltaTime = deltaClock.Restart();
            window.SetView(c.view);
            window.SetFramerateLimit(60);
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            while(window.IsOpen)
            {
                window.DispatchEvents();
                Keys();
                window.Clear();
                window.Draw(plane);
                window.Draw(renEngine);
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    case Keyboard.Key.W:
                        c.Position += new Vector3f(0, 0, cameraStepsPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.S:
                        c.Position += new Vector3f(0, 0, -cameraStepsPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.A:
                        c.Position += new Vector3f(-cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.D:
                        c.Position += new Vector3f(cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.LShift:
                        c.Position += new Vector3f(0, cameraStepsPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Space:
                        c.Position += new Vector3f(0, -cameraStepsPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Escape:
                        window.Close();
                        break;

                }
            }
        }
        private static void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Code);
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.Code);
        }
    }
}
