using GK.Drawables;
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
    abstract class Scene : Drawable
    {
        public Camera mainCamera = new Camera();
        public Transform worldTransform = Transform.Identity;

        public List<LightSource> lightSources = new List<LightSource>();
        public List<Drawable3D> drawables = new List<Drawable3D>();
        protected Time deltaTime = new Time();
        protected AxisIndicator axisIndicator = new AxisIndicator();

        protected static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        //this font should go to some kind resource manager
        protected Font font = new Font("./Fonts/arial.ttf");

        public virtual void Load() { }
        public virtual void Update(Time deltaTime) { this.deltaTime = deltaTime; Keys(); }

        public void Draw(RenderTarget target, RenderStates states)
        {
            RenderEngine.Instance.RenderScene(this, target, states);
            // debug stuff
            if (Options.Instance.ShowAxis)
                target.Draw(axisIndicator);
            // text
            if (Options.Instance.ShowDebugHUD)
            {
                Vec3 p = mainCamera.Position;
                Vec3 r = mainCamera.Rotation * 180 / (float)Math.PI;
                string coords = string.Format("##Coordinates:\nx:{0}\ny:{1}\nz:{2}", p.X, p.Y, p.Z);
                string rot = string.Format("##Camera Rotation:\nx:{0}\ny:{1}\nz:{2}", r.X, r.Y, r.Z);
                string debugText = string.Format("fps: {0}\n{1}\n{2}", string.Format("{0:0.00}", 1f / deltaTime.AsSeconds()), coords, rot);
                Text t = new Text(debugText, font)
                {
                    CharacterSize = 14,
                    FillColor = Color.White,
                    OutlineColor = Color.Black,
                    OutlineThickness = 1,
                    Style = Text.Styles.Bold,
                };
                target.Draw(t);
                t.Dispose();
            }
        }

        public virtual void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) { }
        public virtual void Window_Closed(object sender, EventArgs e) { }
        public virtual void Window_Resized(object sender, SizeEventArgs e) { }
        public virtual void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) { }
        public virtual void Window_MouseMoved(object sender, MouseMoveEventArgs e) { }
        public virtual void Keys() { }
        public virtual void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Code);
        }
        public virtual void Window_KeyPressed(object sender, KeyEventArgs e)
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
                    Texture texture = new Texture(r.Size.X, r.Size.Y); texture.Update((Window)sender);
                    Image img = texture.CopyToImage();
                    string filename = DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss") + ".jpg";
                    img.SaveToFile(filename);
                    Console.WriteLine("Screenshot \"{0}\" saved to working directory", filename);
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
