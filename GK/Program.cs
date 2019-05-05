using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace GK
{
    class Program
    {
        public static RenderWindow window = new RenderWindow(new VideoMode(800, 600), "GK");
        public static Time deltaTime;

        public static bool IsMouseCenterSnapped = false;
        public static float cameraStepsPerSec = 100;

        static void Main(string[] args)
        {
            Clock deltaClock = new Clock();
            deltaTime = deltaClock.Restart();
            //window.SetView(c.view);
            window.SetFramerateLimit(60);
            window.SetKeyRepeatEnabled(false);

            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            window.Resized += Window_Resized;
            window.Closed += Window_Closed;
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;

            while(window.IsOpen)
            {
                window.DispatchEvents();
                Keys();
                window.Clear();
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        private static void Window_Resized(object sender, SizeEventArgs e)
        {
            //float fovAngle = c.FOVAngle;
            //View newView = new View(new FloatRect(-e.Width/2f, -e.Height/2f, e.Width, e.Height));
            //window.SetView(newView);
            //c.view = newView;
            //c.FOVAngle = fovAngle;
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
            //if (IsMouseCenterSnapped)
            //{
            //    Vector2i windowCenter = (Vector2i)window.Size / 2;
            //    Mouse.SetPosition(windowCenter, window);
            //    //translate mouse movement to 3d rotation
            //    Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;

            //    float rotationScale = 0.7f;
            //    //mouse move half screen = 90 deg rotation
            //    float angleXnoZ = -delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
            //    float angleYnoZ = -delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right
            //    //including Z rotation
            //    float sinZ = (float)Math.Sin(Camera.Instance.Rotation.Z);
            //    float cosZ = (float)Math.Cos(Camera.Instance.Rotation.Z);
            //    float angleX = angleXnoZ * cosZ + angleYnoZ * sinZ;
            //    float angleY = angleXnoZ * sinZ - angleYnoZ * cosZ;

            //    ////bound camera X angle to [-90 ; 90]
            //    float finalAngleY = Camera.Instance.Rotation.X + angleY;
            //    finalAngleY = (float)Math.Max(finalAngleY, -Math.PI / 2);
            //    finalAngleY = (float)Math.Min(finalAngleY, Math.PI / 2);
            //    finalAngleY -= Camera.Instance.Rotation.X;

            //    Camera.Instance.Rotation += new Vector3f(finalAngleY, angleX,0);
            //}
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            //float cameraAngleY = Camera.Instance.Rotation.Y;
            //Transform3D t = Transform3D.Identity.Rotate(new Vector3f(0, cameraAngleY, 0));
            //foreach (var key in pressedKeys)
            //{
            //    switch (key)
            //    {
            //        //camera moving
            //        case Keyboard.Key.W:
            //            c.Position += t.TransformPoint(new Vector3f(0, 0, cameraStepsPerSec * deltaTime.AsSeconds()));
            //            break;
            //        case Keyboard.Key.S:
            //            c.Position += t.TransformPoint(new Vector3f(0, 0, -cameraStepsPerSec * deltaTime.AsSeconds()));
            //            break;
            //        case Keyboard.Key.A:
            //            c.Position += t.TransformPoint(new Vector3f(-cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0));
            //            break;
            //        case Keyboard.Key.D:
            //            c.Position += t.TransformPoint(new Vector3f(cameraStepsPerSec * deltaTime.AsSeconds(), 0, 0));
            //            break;
            //        case Keyboard.Key.LShift:
            //            c.Position += t.TransformPoint(new Vector3f(0, -cameraStepsPerSec * deltaTime.AsSeconds(), 0));
            //            break;
            //        case Keyboard.Key.Space:
            //            c.Position += t.TransformPoint(new Vector3f(0, cameraStepsPerSec * deltaTime.AsSeconds(), 0));
            //            break;
            //        //camera tilt
            //        case Keyboard.Key.Q:
            //            c.Rotation += new Vector3f(0, 0, cameraStepsPerSec * deltaTime.AsSeconds() / 200);
            //            break;
            //        case Keyboard.Key.E:
            //            c.Rotation -= new Vector3f(0, 0, cameraStepsPerSec * deltaTime.AsSeconds() / 200);
            //            break;
            //        //FOV
            //        case Keyboard.Key.P:
            //            c.FOVAngle -= cameraStepsPerSec * deltaTime.AsSeconds() / 200;
            //            break;
            //        case Keyboard.Key.O:
            //            c.FOVAngle += cameraStepsPerSec * deltaTime.AsSeconds() / 200;
            //            break;
            //        case Keyboard.Key.Escape:
            //            window.Close();
            //            break;

            //    }
            //}
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
