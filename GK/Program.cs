using GK.Drawables;
using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace GK
{
    class Program
    {
        public static View windowView = new View(new FloatRect(-400, -300, 800, 600));
        public static RenderWindow window = new RenderWindow(new VideoMode(800, 600), "GK");
        public static Camera sceneCamera;
        public static Scene scene = new Scene();
        public static Time deltaTime;
        public static Frame renderFrame;

        public static bool IsMouseCenterSnapped = false;
        public static float cameraStepsPerSec = 1;

        static void Main(string[] args)
        {
            renderFrame = scene.RenderFrame;
            sceneCamera = scene.Camera;
            sceneCamera.Position += new Vector3Df(0, 0, 0);
            AxisIndicator axisind = new AxisIndicator(sceneCamera);
            Clock deltaClock = new Clock();
            deltaTime = deltaClock.Restart();
            window.SetView(windowView);
            window.SetFramerateLimit(90);
            window.SetKeyRepeatEnabled(false);

            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            window.Resized += Window_Resized;
            window.Closed += Window_Closed;
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;
            window.MouseWheelScrolled += Window_MouseWheelScrolled;

            while(window.IsOpen)
            {
                //Console.WriteLine(1f/deltaTime.AsSeconds() + " fps");
                window.DispatchEvents();
                Keys();
                window.Clear();
                scene.RenderFrame.Clear();
                window.Draw(scene);
                window.Draw(axisind);
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        private static void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            float d = (float)Math.Abs(sceneCamera.fNear - Math.Round(sceneCamera.fNear))/2;
            sceneCamera.fNear += d * -e.Delta;
            sceneCamera.fNear = Math.Max(sceneCamera.fNear, 0.01f);
            sceneCamera.fNear = Math.Min(sceneCamera.fNear, 0.99f);
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        private static void Window_Resized(object sender, SizeEventArgs e)
        {
            windowView = new View(new FloatRect(-e.Width / 2, -e.Height / 2, e.Width, e.Height));
            window.SetView(windowView);

            scene.RenderFrame = new Frame((int)e.Width, (int)e.Height);
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

                float rotationScale = 0.5f;
                //mouse move half screen = 90 deg rotation
                float angleXnoZ = -delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
                float angleYnoZ = -delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right
                //including Z rotation
                float sinZ = (float)Math.Sin(sceneCamera.Rotation.Z);
                float cosZ = (float)Math.Cos(sceneCamera.Rotation.Z);
                float angleX = angleXnoZ * cosZ + angleYnoZ * sinZ;
                float angleY = angleXnoZ * sinZ - angleYnoZ * cosZ;

                ////bound camera X angle to [-90 ; 90]
                float finalAngleY = sceneCamera.Rotation.X + angleY;
                finalAngleY = (float)Math.Max(finalAngleY, -Math.PI / 2);
                finalAngleY = (float)Math.Min(finalAngleY, Math.PI / 2);
                finalAngleY -= sceneCamera.Rotation.X;

                sceneCamera.Rotation += new Vector3Df(finalAngleY, angleX, 0);
            }
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            float cameraAngleY = sceneCamera.Rotation.Y;
            Transform3D t = Transform3D.Identity.Rotate(new Vector3Df(0, cameraAngleY, 0));
            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    //camera moving
                    case Keyboard.Key.W:
                        sceneCamera.Position += t * new Vector3Df(0, 0, cameraStepsPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.S:
                        sceneCamera.Position += t * new Vector3Df(0, 0, -cameraStepsPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.A:
                        sceneCamera.Position += t * new Vector3Df(-cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.D:
                        sceneCamera.Position += t * new Vector3Df(cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.LShift:
                        sceneCamera.Position += t * new Vector3Df(0, -cameraStepsPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Space:
                        sceneCamera.Position += t * new Vector3Df(0, cameraStepsPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    //camera tilt
                    case Keyboard.Key.Q:
                        sceneCamera.Rotation += new Vector3Df(0, 0, cameraStepsPerSec * deltaTime.AsSeconds() / 2);
                        break;
                    case Keyboard.Key.E:
                        sceneCamera.Rotation -= new Vector3Df(0, 0, cameraStepsPerSec * deltaTime.AsSeconds() / 2);
                        break;
                    //FOV
                    //case Keyboard.Key.P:
                    //    sceneCamera.FOVAngle -= cameraStepsPerSec * deltaTime.AsSeconds() / 200;
                    //    break;
                    //case Keyboard.Key.O:
                    //    sceneCamera.FOVAngle += cameraStepsPerSec * deltaTime.AsSeconds() / 200;
                    //    break;
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
