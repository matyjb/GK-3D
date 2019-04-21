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

        public static bool IsMouseCenterSnapped = false;
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
            
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;
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

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            IsMouseCenterSnapped = true;
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (IsMouseCenterSnapped)
            {
                Vector2i windowCenter = (Vector2i)window.Size / 2;
                Mouse.SetPosition(windowCenter, window);
                Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;
                float rotationScale = 1;
                //mouse move half screen = 90 deg rotation
                float angleX = - delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
                float angleY = - delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right

                Camera.Instance.Rotation += new Vector3f(angleY, angleX, 0);
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
                        if (IsMouseCenterSnapped) IsMouseCenterSnapped = false;
                        else window.Close();
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
