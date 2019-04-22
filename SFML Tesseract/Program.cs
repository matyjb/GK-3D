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
            Position = new Vector3f(-50, -250, 100),
            Rotation =new Vector3f(30 * (float)Math.PI / 180, 30 * (float)Math.PI / 180, 30 * (float)Math.PI / 180),
            FillColor = Color.Red,
        };
        public static Plane3D plane2 = new Plane3D()
        {
            Position = new Vector3f(-100, 300, 100),
            Height = 1000,
            Width = 1000,
            Rotation = new Vector3f(90 * (float)Math.PI / 180, 0,0),
            FillColor = Color.Blue,
        };
        public static AxisIndicator axisIndicator = new AxisIndicator();
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
            window.Resized += Window_Resized;
            
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;
            while(window.IsOpen)
            {
                window.DispatchEvents();
                Keys();
                window.Clear();
                window.Draw(plane);
                window.Draw(plane2);
                window.Draw(renEngine);
                window.Draw(axisIndicator);
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        private static void Window_Resized(object sender, SizeEventArgs e)
        {
            float fovAngle = c.FOVAngle;
            View newView = new View(new FloatRect(-e.Width/2f, -e.Height/2f, e.Width, e.Height));
            window.SetView(newView);
            c.view = newView;
            c.FOVAngle = fovAngle;
        }

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            IsMouseCenterSnapped = true;
            window.SetMouseCursorVisible(false);
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (IsMouseCenterSnapped)
            {
                Vector2i windowCenter = (Vector2i)window.Size / 2;
                Mouse.SetPosition(windowCenter, window);
                //translate mouse movement to 3d rotation
                Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;
                float rotationScale = 0.7f;
                //mouse move half screen = 90 deg rotation
                float angleX = -delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
                float angleY = -delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right

                ////bound camera X angle to [-90 ; 90]
                float finalAngleY = Camera.Instance.Rotation.X + angleY;
                finalAngleY = (float)Math.Max(finalAngleY, -Math.PI / 2);
                finalAngleY = (float)Math.Min(finalAngleY, Math.PI / 2);
                finalAngleY -= Camera.Instance.Rotation.X;

                Camera.Instance.Rotation += new Vector3f(finalAngleY, angleX,0);
            }
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            float cameraAngleY = Camera.Instance.Rotation.Y;
            Transform3D t = Transform3D.Identity.Rotate(new Vector3f(0, cameraAngleY, 0));
            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    case Keyboard.Key.W:
                        c.Position += t.TransformPoint(new Vector3f(0, 0, cameraStepsPerSec * deltaTime.AsSeconds()));
                        break;
                    case Keyboard.Key.S:
                        c.Position += t.TransformPoint(new Vector3f(0, 0, -cameraStepsPerSec * deltaTime.AsSeconds()));
                        break;
                    case Keyboard.Key.A:
                        c.Position += t.TransformPoint(new Vector3f(-cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0));
                        break;
                    case Keyboard.Key.D:
                        c.Position += t.TransformPoint(new Vector3f(cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0));
                        break;
                    case Keyboard.Key.LShift:
                        c.Position += t.TransformPoint(new Vector3f(0, cameraStepsPerSec * deltaTime.AsSeconds(), 0));
                        break;
                    case Keyboard.Key.Space:
                        c.Position += t.TransformPoint(new Vector3f(0, -cameraStepsPerSec * deltaTime.AsSeconds(), 0));
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
