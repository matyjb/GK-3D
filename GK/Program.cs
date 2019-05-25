using GK.Drawables;
using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using Transform = GK.Math3D.Transform;

namespace GK
{
    class Program
    {
        public static uint Width = 800;
        public static uint Height = 600;
        public static RenderWindow window = new RenderWindow(new VideoMode(Width, Height), "GK");
        public static RenderEngine engine = RenderEngine.Instance;
        public static Time deltaTime;
        public static bool IsMouseCenterSnapped = false;

        static void Main(string[] args)
        {
            engine.Window = window;
            Camera.Instance.Position = new Vec3(0, 0, -10);
            Clock deltaClock = new Clock();
            Font font = new Font("./Fonts/arial.ttf");
            Text fpsAmount = new Text("fps: 0", font)
            {
                CharacterSize = 14,
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 1,
                Style = Text.Styles.Bold,
            };


            deltaTime = deltaClock.Restart();
            window.SetFramerateLimit(90);
            window.SetKeyRepeatEnabled(false);

            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            window.Resized += Window_Resized;
            window.Closed += Window_Closed;
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;
            window.MouseWheelScrolled += Window_MouseWheelScrolled;

            ///////////
            Vec3 v0 = new Vec3(0, 0, 0);
            Vec3 v1 = new Vec3(0, 0, 1);
            Vec3 v2 = new Vec3(0, 1, 0);
            Vec3 v3 = new Vec3(0, 1, 1);
            Vec3 v4 = new Vec3(1, 0, 0);
            Vec3 v5 = new Vec3(1, 0, 1);
            Vec3 v6 = new Vec3(1, 1, 0);
            Vec3 v7 = new Vec3(1, 1, 1);
            Mesh cube = new Mesh()
            {
                // SOUTH
                new Triangle( v0, v2, v6, Color.Green ),
                new Triangle( v0, v6, v4, Color.Green ),

		        // EAST                                                      
		        new Triangle( v4, v6, v7, Color.Red ),
                new Triangle( v4, v7, v5, Color.Red ),

		        // NORTH                                                     
		        new Triangle( v5, v7, v3, Color.Green ),
                new Triangle( v5, v3, v1, Color.Green ),

		        // WEST                                                      
		        new Triangle( v1, v3, v2, Color.Red ),
                new Triangle( v1, v2, v0, Color.Red ),

		        // TOP                                                       
		        new Triangle( v2, v3, v7, Color.Blue ),
                new Triangle( v2, v7, v6, Color.Blue ),

		        // BOTTOM                                                    
		        new Triangle( v5, v1, v0, Color.Blue ),
                new Triangle( v5, v0, v4, Color.Blue ),
            };

            ///////////
            while (window.IsOpen)
            {
                fpsAmount.DisplayedString = string.Format("fps: {0:0.00}", 1f / deltaTime.AsSeconds());
                window.DispatchEvents();
                Keys();
                window.Clear();
                /////////////////
                ///rotation and translation
                engine.Mesh = cube;
                engine.GlobalTransform = Camera.Instance.InverseTransform;
                engine.Projection = Camera.Instance.ProjectionTransform;



                /////////////////
                window.Draw(engine);
                window.Draw(fpsAmount);
                window.Draw(new AxisIndicator());
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        private static void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            float d = (float)Math.Abs(Camera.Instance.Near - Math.Round(Camera.Instance.Near)) / 2;
            Camera.Instance.Near += d * -e.Delta;
            Camera.Instance.Near = Math.Max(Camera.Instance.Near, 0.01f);
            Camera.Instance.Near = Math.Min(Camera.Instance.Near, 0.99f);
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        private static void Window_Resized(object sender, SizeEventArgs e)
        {
            Width = e.Width;
            Height = e.Height;
            View newView = new View(new FloatRect(0, 0, Width, Height));
            window.SetView(newView);
            Camera.Instance.Width = Width;
            Camera.Instance.Height = Height;
        }

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                IsMouseCenterSnapped = true;
                window.SetMouseCursorVisible(false);
            }
            else if (e.Button == Mouse.Button.Right)
            {
                IsMouseCenterSnapped = false;
                window.SetMouseCursorVisible(true);
            }
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (IsMouseCenterSnapped)
            {
                Vector2i windowCenter = (Vector2i)window.Size / 2;
                Mouse.SetPosition(windowCenter, window);
                //translate mouse movement to 3d rotation
                Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;

                float rotationScale = Camera.Instance.RotatingSpeedPerSec;
                //mouse move half screen = 90 deg rotation
                float angleXnoZ = -delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
                float angleYnoZ = -delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right
                //including Z rotation
                float sinZ = (float)Math.Sin(Camera.Instance.Rotation.Z);
                float cosZ = (float)Math.Cos(Camera.Instance.Rotation.Z);
                float angleX = angleXnoZ * cosZ + angleYnoZ * sinZ;
                float angleY = angleXnoZ * sinZ - angleYnoZ * cosZ;

                ////bound camera X angle to [-90 ; 90]
                float finalAngleY = Camera.Instance.Rotation.X + angleY;
                finalAngleY = (float)Math.Max(finalAngleY, -Math.PI / 2);
                finalAngleY = (float)Math.Min(finalAngleY, Math.PI / 2);
                finalAngleY -= Camera.Instance.Rotation.X;

                Camera.Instance.Rotation += new Vec3(finalAngleY, -angleX, 0);
            }
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            Transform t = Transform.Identity.Rotate(new Vec3(0, Camera.Instance.Rotation.Y, 0));

            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    //Moving
                    case Keyboard.Key.W:
                        Camera.Instance.Position += t * new Vec3(0, 0, Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.S:
                        Camera.Instance.Position += t * new Vec3(0, 0, -Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.A:
                        Camera.Instance.Position += t * new Vec3(Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.D:
                        Camera.Instance.Position += t * new Vec3(-Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.LShift:
                        Camera.Instance.Position += t * new Vec3(0, -Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Space:
                        Camera.Instance.Position += t * new Vec3(0, Camera.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    //camera tilt
                    case Keyboard.Key.Q:
                        Camera.Instance.Rotation += new Vec3(0, 0, Camera.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.E:
                        Camera.Instance.Rotation -= new Vec3(0, 0, Camera.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
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
