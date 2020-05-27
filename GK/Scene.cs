using GK.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using Transform = GK.Math3D.Transform;

namespace GK
{
    abstract class Scene
    {
        public Camera mainCamera = new Camera();
        public Transform worldTransform = Transform.Identity;

        public List<LightSource> lightSources = new List<LightSource>();
        public List<Drawable3D> drawables = new List<Drawable3D>();
        protected Time deltaTime = new Time();

        protected static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();


        public virtual void Load() { }
        public virtual void Update(Time deltaTime) { Keys(); this.deltaTime = deltaTime; }
        public virtual Texture RenderScene(uint width, uint height) 
        {
            Texture texture = RenderEngine.Instance.RenderScene(this, width, height);
            return texture; 
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
            pressedKeys.Add(e.Code);
        }
    }
}
