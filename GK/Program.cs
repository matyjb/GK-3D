using GK.Drawables;
using GK.Drawables._2D;
using GK.Interfaces;
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
        public static bool IsMouseLockedCenter = false;

        static void Main()
        {
            engine.Window = window;
            Camera.Instance.Position = new Vec3(0, 0, -10);
            Clock deltaClock = new Clock();
            Font font = new Font("./Fonts/arial.ttf");
            string fpsAmount = "";


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

            Vec3 v00 = new Vec3(0, 0, 0);
            Vec3 v01 = new Vec3(0, 0, 1);
            Vec3 v02 = new Vec3(0, 1, 0);
            Vec3 v03 = new Vec3(0, 1, 1);
            Vec3 v04 = new Vec3(1, 0, 0);
            Vec3 v05 = new Vec3(1, 0, 1);
            Vec3 v06 = new Vec3(1, 1, 0);
            Vec3 v07 = new Vec3(1, 1, 1);


            Vertex3 vert20 = new Vertex3(v00, (Vec4Color)Color.White);
            Vertex3 vert21 = new Vertex3(v01, (Vec4Color)Color.Blue);
            Vertex3 vert22 = new Vertex3(v02, (Vec4Color)Color.Green);
            Vertex3 vert23 = new Vertex3(v03, (Vec4Color)Color.Cyan);
            Vertex3 vert24 = new Vertex3(v04, (Vec4Color)Color.Red);
            Vertex3 vert25 = new Vertex3(v05, (Vec4Color)Color.Magenta);
            Vertex3 vert26 = new Vertex3(v06, (Vec4Color)Color.Yellow);
            Vertex3 vert27 = new Vertex3(v07, (Vec4Color)Color.Black);
            Mesh cube1 = new Mesh()
            {
                // SOUTH
                new Tri( v00, v02, v06, (Vec4Color)Color.Blue ),
                new Tri( v00, v06, v04, (Vec4Color)Color.Blue ),

		        // EAST                                                      
		        new Tri( v04, v06, v07, (Vec4Color)Color.Red ),
                new Tri( v04, v07, v05, (Vec4Color)Color.Red ),

		        // NORTH                                                     
		        new Tri( v05, v07, v03, (Vec4Color)Color.Blue ),
                new Tri( v05, v03, v01, (Vec4Color)Color.Blue ),

		        // WEST                                                      
		        new Tri( v01, v03, v02, (Vec4Color)Color.Red ),
                new Tri( v01, v02, v00, (Vec4Color)Color.Red ),

		        // TOP                                                       
		        new Tri( v02, v03, v07, (Vec4Color)Color.Green ),
                new Tri( v02, v07, v06, (Vec4Color)Color.Green ),
                                          
		        // BOTTOM                                                    
		        new Tri( v05, v01, v00, (Vec4Color)Color.Green ),
                new Tri( v05, v00, v04, (Vec4Color)Color.Green ),
            };
            Mesh cube2 = new Mesh()
            {              
                // SOUTH                  
                new Tri( v00, v02, v06, (Vec4Color)Color.Blue ),
                new Tri( v00, v06, v04, (Vec4Color)Color.Blue ),
                                          
		        // EAST                                                      
		        new Tri( v04, v06, v07, (Vec4Color)Color.Red ),
                new Tri( v04, v07, v05, (Vec4Color)Color.Red ),
                                          
		        // NORTH                                                     
		        new Tri( v05, v07, v03, (Vec4Color)Color.Blue ),
                new Tri( v05, v03, v01, (Vec4Color)Color.Blue ),
                                          
		        // WEST                                                      
		        new Tri( v01, v03, v02, (Vec4Color)Color.Red ),
                new Tri( v01, v02, v00, (Vec4Color)Color.Red ),
                                          
		        // TOP                                                       
		        new Tri( v02, v03, v07, (Vec4Color)Color.Green ),
                new Tri( v02, v07, v06, (Vec4Color)Color.Green ),
                                          
		        // BOTTOM                                                    
		        new Tri( v05, v01, v00, (Vec4Color)Color.Green ),
                new Tri( v05, v00, v04, (Vec4Color)Color.Green ),
            };
            Mesh cubeRGB = new Mesh()
            {              
                // SOUTH                  
                new Tri( vert20, vert22, vert26),
                new Tri( vert20, vert26, vert24),
                                          
		        // EAST                               
		        new Tri( vert24, vert26, vert27),
                new Tri( vert24, vert27, vert25),
                                          
		        // NORTH                              
		        new Tri( vert25, vert27, vert23),
                new Tri( vert25, vert23, vert21),
                                          
		        // WEST                               
		        new Tri( vert21, vert23, vert22),
                new Tri( vert21, vert22, vert20),
                                          
		        // TOP                                
		        new Tri( vert22, vert23, vert27),
                new Tri( vert22, vert27, vert26),
                                          
		        // BOTTOM                             
		        new Tri( vert25, vert21, vert20),
                new Tri( vert25, vert20, vert24),
            };
            Sphere sphere = new Sphere(1, 3, (Vec4Color)Color.Green);

            Triangle triangle = new Triangle(v00, v02, v06, (Vec4Color)Color.Blue);

            cube1.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            cube2.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            cubeRGB.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            triangle.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            sphere.Origin = new Vec3(0.5f, 0.5f, 0.5f);

            Vec3 moveV = new Vec3(2, 2, 3);
            cube2.Position = moveV;
            cube2.Scale = new Vec3(1.5f,1.5f,1.5f);
            cubeRGB.Position = moveV / 2;
            triangle.Position = moveV * 2;
            sphere.Position = new Vec3(0,5,0);


            List<Drawable3D> drawables = new List<Drawable3D>
            {
                cube1,
                cube2,
                cubeRGB,
                triangle,
                sphere,
            };
            List<LightSource> lightSources = new List<LightSource>
            {
                new LightSource(){Position = new Vec3(-3,3,-3), Intensity=1f },
                //new LightSource(){Position = new Vec3(1,6,6), Intensity=0.1f},
            };
            ///////////
            while (window.IsOpen)
            {
                fpsAmount = string.Format("{0:0.00}", 1f / deltaTime.AsSeconds());
                window.DispatchEvents();
                Keys();
                window.Clear();
                /////////////////
                ///rotation and translation
                //rotation cube2
                Vec3 v = new Vec3(1 / 2f, 1 / 3f, 1 / 4f) * (float)Math.PI * deltaTime.AsSeconds();
                cube1.Rotation += v;
                cube2.Rotation -= v;
                cubeRGB.Rotation += v/2;
                triangle.Rotation += v*2;


                engine.Drawables3D = drawables;
                engine.LightSources = lightSources;
                engine.MatInvCamera = Camera.Instance.InverseTransform;
                engine.Projection = Camera.Instance.ProjectionTransform;



                /////////////////
                window.Draw(engine);

                if (Options.Instance.ShowDebugHUD)
                {
                    Vec3 p = Camera.Instance.Position;
                    Vec3 r = Camera.Instance.Rotation * 180 / (float)Math.PI;
                    string coords = string.Format("##Coordinates:\nx:{0}\ny:{1}\nz:{2}", p.X, p.Y, p.Z);
                    string rot = string.Format("##Camera Rotation:\nx:{0}\ny:{1}\nz:{2}", r.X, r.Y, r.Z);
                    string debugText = string.Format("fps: {0}\n{1}\n{2}", fpsAmount, coords, rot);
                    Text t = new Text(debugText, font)
                    {
                        CharacterSize = 14,
                        FillColor = Color.White,
                        OutlineColor = Color.Black,
                        OutlineThickness = 1,
                        Style = Text.Styles.Bold,
                    };
                    window.Draw(t);
                    t.Dispose();
                }
                if (Options.Instance.ShowAxis)
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
                IsMouseLockedCenter = true;
                window.SetMouseCursorVisible(false);
            }
            else if (e.Button == Mouse.Button.Right)
            {
                IsMouseLockedCenter = false;
                window.SetMouseCursorVisible(true);
            }
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (IsMouseLockedCenter)
            {
                Vector2i windowCenter = (Vector2i)window.Size / 2;
                Mouse.SetPosition(windowCenter, window);
                //translate mouse movement to 3d rotation
                Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;

                float rotationScale = Options.Instance.RotatingSpeedPerSec;
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
                        Camera.Instance.Position += t * new Vec3(0, 0, Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.S:
                        Camera.Instance.Position += t * new Vec3(0, 0, -Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.A:
                        Camera.Instance.Position += t * new Vec3(Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.D:
                        Camera.Instance.Position += t * new Vec3(-Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.LShift:
                        Camera.Instance.Position += t * new Vec3(0, -Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Space:
                        Camera.Instance.Position += t * new Vec3(0, Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    //camera tilt
                    case Keyboard.Key.Q:
                        Camera.Instance.Rotation += new Vec3(0, 0, Options.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.E:
                        Camera.Instance.Rotation -= new Vec3(0, 0, Options.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
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
            switch (e.Code)
            {
                //debug - wireframe
                case Keyboard.Key.F1:
                    Options.Instance.ShowWireframe = !Options.Instance.ShowWireframe;
                    break;
                //screenshot
                case Keyboard.Key.F2:
                    RenderWindow r = (RenderWindow)sender;
                    Texture texture = new Texture(r.Size.X,r.Size.Y); texture.Update(window);
                    Image img = texture.CopyToImage();
                    img.SaveToFile(DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss") + ".jpg");
                    img.Dispose();
                    texture.Dispose();
                    break;
                //debug hud
                case Keyboard.Key.F3:
                    Options.Instance.ShowAxis = !Options.Instance.ShowAxis;
                    Options.Instance.ShowDebugHUD = !Options.Instance.ShowDebugHUD;
                    break;
                default:
                    pressedKeys.Add(e.Code);
                    break;
            }
        }
    }
}
