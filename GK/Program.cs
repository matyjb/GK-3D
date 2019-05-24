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
        public static Time deltaTime;

        static void Main(string[] args)
        {
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
                new Triangle( v0, v2, v6 ),
                new Triangle( v0, v6, v4 ),

		        // EAST                                                      
		        new Triangle( v4, v6, v7 ),
                new Triangle( v4, v7, v5 ),

		        // NORTH                                                     
		        new Triangle( v5, v7, v3 ),
                new Triangle( v5, v3, v1 ),

		        // WEST                                                      
		        new Triangle( v1, v3, v2 ),
                new Triangle( v1, v2, v0 ),

		        // TOP                                                       
		        new Triangle( v2, v3, v7 ),
                new Triangle( v2, v7, v6 ),

		        // BOTTOM                                                    
		        new Triangle( v5, v1, v0 ),
                new Triangle( v5, v0, v4 ),
            };

            // Projection Matrix
            float fNear = 0.8f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = (float)Height / (float)Width;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * Math.PI);

            float[,] m = new float[4, 4];
            m[0, 0] = fAspectRatio * fFovRad;
            m[1, 1] = fFovRad;
            m[2, 2] = fFar / (fFar - fNear);
            m[3, 2] = (-fFar * fNear) / (fFar - fNear);
            m[2, 3] = 1.0f;
            m[3, 3] = 0.0f;
            Transform matProj = new Transform(m);
            float theta = 0;

            ///////////
            while (window.IsOpen)
            {
                fpsAmount.DisplayedString = string.Format("fps: {0:0.00}", 1f / deltaTime.AsSeconds());
                window.DispatchEvents();
                Keys();
                window.Clear();
                /////////////////
                ///rotation 
                theta += 1 * deltaTime.AsSeconds();
                Transform rotTran = Transform.Identity.Rotate(new Vec3(theta / 2, 0, theta)).Translate(new Vec3(0, 0, 3));
                foreach (Triangle triangle in cube)
                {
                    Vertex[] vertexArray = new Vertex[4];
                    for (int i = 0; i < 3; i++)
                    {
                        Vec3 tmp = matProj * rotTran * triangle[i];
                        //move and scale into view
                        tmp.X += 1f;
                        tmp.Y += 1f;
                        tmp.X *= Width / 2;
                        tmp.Y *= Height / 2;
                        vertexArray[i] = new Vertex((Vector2f)tmp, Color.White);
                        if (i == 0) vertexArray[3] = new Vertex((Vector2f)tmp, Color.White);
                    }

                    window.Draw(vertexArray, PrimitiveType.LineStrip);
                }



                /////////////////
                window.Draw(fpsAmount);
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        private static void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        private static void Window_Resized(object sender, SizeEventArgs e)
        {
        }

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            foreach (var key in pressedKeys)
            {
                switch (key)
                {
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
