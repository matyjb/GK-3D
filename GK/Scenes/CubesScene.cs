using GK.Drawables;
using GK.Drawables._2D;
using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using Transform = GK.Math3D.Transform;

namespace GK.Scenes
{
    class CubesScene : Scene
    {
        bool IsMouseLockedCenter { get; set; } = false;
        
        //this font should go to some kind resource manager
        Font font = new Font("./Fonts/arial.ttf");
        string fpsAmount = "";

        public override void Load()
        {
            base.Load();
            Vec3 v00 = new Vec3(0, 0, 0);
            Vec3 v01 = new Vec3(0, 0, 1);
            Vec3 v02 = new Vec3(0, 1, 0);
            Vec3 v03 = new Vec3(0, 1, 1);
            Vec3 v04 = new Vec3(1, 0, 0);
            Vec3 v05 = new Vec3(1, 0, 1);
            Vec3 v06 = new Vec3(1, 1, 0);
            Vec3 v07 = new Vec3(1, 1, 1);


            Vertex3 vert20 = new Vertex3(v00, (Vec4)Color.White);
            Vertex3 vert21 = new Vertex3(v01, (Vec4)Color.Blue);
            Vertex3 vert22 = new Vertex3(v02, (Vec4)Color.Green);
            Vertex3 vert23 = new Vertex3(v03, (Vec4)Color.Cyan);
            Vertex3 vert24 = new Vertex3(v04, (Vec4)Color.Red);
            Vertex3 vert25 = new Vertex3(v05, (Vec4)Color.Magenta);
            Vertex3 vert26 = new Vertex3(v06, (Vec4)Color.Yellow);
            Vertex3 vert27 = new Vertex3(v07, (Vec4)Color.Black);
            Mesh cube1 = new Mesh()
            {
                // SOUTH
                new Tri( v00, v02, v06, (Vec4)Color.Blue ),
                new Tri( v00, v06, v04, (Vec4)Color.Blue ),

		        // EAST                                                      
		        new Tri( v04, v06, v07, (Vec4)Color.Red ),
                new Tri( v04, v07, v05, (Vec4)Color.Red ),

		        // NORTH                                                     
		        new Tri( v05, v07, v03, (Vec4)Color.Blue ),
                new Tri( v05, v03, v01, (Vec4)Color.Blue ),

		        // WEST                                                      
		        new Tri( v01, v03, v02, (Vec4)Color.Red ),
                new Tri( v01, v02, v00, (Vec4)Color.Red ),

		        // TOP                                                       
		        new Tri( v02, v03, v07, (Vec4)Color.Green ),
                new Tri( v02, v07, v06, (Vec4)Color.Green ),
                                          
		        // BOTTOM                                                    
		        new Tri( v05, v01, v00, (Vec4)Color.Green ),
                new Tri( v05, v00, v04, (Vec4)Color.Green ),
            };
            Mesh cube2 = new Mesh()
            {              
                // SOUTH                  
                new Tri( v00, v02, v06, (Vec4)Color.Blue ),
                new Tri( v00, v06, v04, (Vec4)Color.Blue ),
                                          
		        // EAST                                                      
		        new Tri( v04, v06, v07, (Vec4)Color.Red ),
                new Tri( v04, v07, v05, (Vec4)Color.Red ),
                                          
		        // NORTH                                                     
		        new Tri( v05, v07, v03, (Vec4)Color.Blue ),
                new Tri( v05, v03, v01, (Vec4)Color.Blue ),
                                          
		        // WEST                                                      
		        new Tri( v01, v03, v02, (Vec4)Color.Red ),
                new Tri( v01, v02, v00, (Vec4)Color.Red ),
                                          
		        // TOP                                                       
		        new Tri( v02, v03, v07, (Vec4)Color.Green ),
                new Tri( v02, v07, v06, (Vec4)Color.Green ),
                                          
		        // BOTTOM                                                    
		        new Tri( v05, v01, v00, (Vec4)Color.Green ),
                new Tri( v05, v00, v04, (Vec4)Color.Green ),
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
            Sphere sphere = new Sphere(1, 3, (Vec4)Color.Green);

            Triangle triangle = new Triangle(v00, v02, v06, (Vec4)Color.Blue);

            cube1.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            cube2.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            cubeRGB.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            triangle.Origin = new Vec3(0.5f, 0.5f, 0.5f);
            sphere.Origin = new Vec3(0.5f, 0.5f, 0.5f);

            Vec3 moveV = new Vec3(2, 2, 3);
            cube2.Position = moveV;
            cube2.Scale = new Vec3(1.5f, 1.5f, 1.5f);
            cubeRGB.Position = moveV / 2;
            triangle.Position = moveV * 2;
            sphere.Position = new Vec3(0, 5, 0);
            drawables.Add(cube1);
            drawables.Add(cube2);
            drawables.Add(cubeRGB);
            drawables.Add(triangle);
            drawables.Add(sphere);
            lightSources.Add(new LightSource() { Position = new Vec3(-3, 3, -3), Intensity = 1f });
            lightSources.Add(new LightSource() { Position = new Vec3(3, -3, 5), Intensity = 1f });
            mainCamera.Position = new Vec3(0, 0, -10);
        }

        public override void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            float d = (float)Math.Abs(mainCamera.Near - Math.Round(mainCamera.Near)) / 2;
            mainCamera.Near += d * -e.Delta;
            mainCamera.Near = Math.Max(mainCamera.Near, 0.01f);
            mainCamera.Near = Math.Min(mainCamera.Near, 0.99f);
        }
        public override void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                IsMouseLockedCenter = true;
                ((Window)sender).SetMouseCursorVisible(false);
            }
            else if (e.Button == Mouse.Button.Right)
            {
                IsMouseLockedCenter = false;
                ((Window)sender).SetMouseCursorVisible(true);
            }
        }
        public override void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            Window window = (Window)sender;
            if (IsMouseLockedCenter)
            {
                Vector2i windowCenter = (Vector2i)(window.Size / 2);
                Mouse.SetPosition(windowCenter, window);
                //translate mouse movement to 3d rotation
                Vector2i delta = new Vector2i(e.X, e.Y) - windowCenter;

                float rotationScale = Options.Instance.RotatingSpeedPerSec;
                //mouse move half screen = 90 deg rotation
                float angleXnoZ = -delta.X / (float)windowCenter.X * (float)Math.PI / 2 * rotationScale; //up down
                float angleYnoZ = -delta.Y / (float)windowCenter.Y * (float)Math.PI / 2 * rotationScale; //left right
                //including Z rotation
                float sinZ = (float)Math.Sin(mainCamera.Rotation.Z);
                float cosZ = (float)Math.Cos(mainCamera.Rotation.Z);
                float angleX = angleXnoZ * cosZ + angleYnoZ * sinZ;
                float angleY = angleXnoZ * sinZ - angleYnoZ * cosZ;

                ////bound camera X angle to [-90 ; 90]
                float finalAngleY = mainCamera.Rotation.X + angleY;
                finalAngleY = (float)Math.Max(finalAngleY, -Math.PI / 2);
                finalAngleY = (float)Math.Min(finalAngleY, Math.PI / 2);
                finalAngleY -= mainCamera.Rotation.X;

                mainCamera.Rotation += new Vec3(finalAngleY, -angleX, 0);
            }
        }
        public override void Keys()
        {
            Transform t = Transform.Identity.Rotate(new Vec3(0, mainCamera.Rotation.Y, 0));

            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    //Moving
                    case Keyboard.Key.W:
                        mainCamera.Position += t * new Vec3(0, 0, Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.S:
                        mainCamera.Position += t * new Vec3(0, 0, -Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.A:
                        mainCamera.Position += t * new Vec3(Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.D:
                        mainCamera.Position += t * new Vec3(-Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0, 0);
                        break;
                    case Keyboard.Key.LShift:
                        mainCamera.Position += t * new Vec3(0, -Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    case Keyboard.Key.Space:
                        mainCamera.Position += t * new Vec3(0, Options.Instance.MovingSpeedPerSec * deltaTime.AsSeconds(), 0);
                        break;
                    //camera tilt
                    case Keyboard.Key.Q:
                        mainCamera.Rotation += new Vec3(0, 0, Options.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                    case Keyboard.Key.E:
                        mainCamera.Rotation -= new Vec3(0, 0, Options.Instance.RotatingSpeedPerSec * deltaTime.AsSeconds());
                        break;
                }
            }
        }
        //public override void Window_KeyPressed(object sender, KeyEventArgs e)
        //{
        //    switch (e.Code)
        //    {
        //        //debug - wireframe
        //        //case Keyboard.Key.F1:
        //        //    Options.Instance.ShowWireframe = !Options.Instance.ShowWireframe;
        //        //    break;
        //        ////screenshot
        //        //case Keyboard.Key.F2:
        //        //    RenderWindow r = (RenderWindow)sender;
        //        //    Texture texture = new Texture(r.Size.X,r.Size.Y); texture.Update(window);
        //        //    Image img = texture.CopyToImage();
        //        //    img.SaveToFile(DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss") + ".jpg");
        //        //    img.Dispose();
        //        //    texture.Dispose();
        //        //    break;
        //        ////debug hud
        //        //case Keyboard.Key.F3:
        //        //    Options.Instance.ShowAxis = !Options.Instance.ShowAxis;
        //        //    Options.Instance.ShowDebugHUD = !Options.Instance.ShowDebugHUD;
        //        //    break;
        //        default:
        //            base.Window_KeyPressed(sender, e);
        //            break;
        //    }
        //}
    }
}
