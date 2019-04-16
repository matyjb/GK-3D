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
        public static Cube3D cube = new Cube3D() {Position = new Vector3f(-50,-50,100) };
        public static Time deltaTime;

        public static float cameraStepsPerSec = 100;

        static void Main(string[] args)
        {
            //Matrix a = new Matrix(new double[3, 1] { { 1 }, { 2 }, { 4 } });
            //Matrix b = Matrix.RotationMatrix(90*Math.PI/180, new int[] { 0,1,0 });
            //Matrix r = b*a;

            //Console.WriteLine(a);
            //Console.WriteLine(b);
            //Console.WriteLine(r);
            //Cube4D c = new Cube4D();
            //Matrix r = Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 0, 1, 1 });
            //r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 1, 0, 1 });
            //r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 0, 0, 1 });
            //r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 1, 1, 0 });
            //r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 0, 1, 0 });
            //r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 1, 0, 0 });
            //c.Points = r * c.Points;
            //Console.WriteLine(c);
            Clock deltaClock = new Clock();
            deltaTime = deltaClock.Restart();
            window.SetView(c.view);
            window.SetFramerateLimit(60);
            //c.Position = new Vector3f(100, 100, 0);
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            while(window.IsOpen)
            {
                window.DispatchEvents();
                Keys();
                window.Clear();
                window.Draw(cube);
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
