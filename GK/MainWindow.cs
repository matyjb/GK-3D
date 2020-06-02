using GK.Scenes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace GK
{
    class MainWindow : RenderWindow
    {
        public Stack<Scene> scenes = new Stack<Scene>();
        Clock clock = new Clock();

        public MainWindow(uint width, uint height, string title) : base(new VideoMode(width, height), title)
        {
            SetFramerateLimit(90);
            SetKeyRepeatEnabled(false);

            // events
            KeyPressed += Window_KeyPressed;
            KeyReleased += Window_KeyReleased;
            Resized += Window_Resized;
            Closed += Window_Closed;
            MouseMoved += Window_MouseMoved;
            MouseButtonPressed += Window_MouseButtonPressed;
            MouseWheelScrolled += Window_MouseWheelScrolled;

            scenes.Push(new CubesScene());
        }

        public void StartMainLoop()
        {
            while (IsOpen)
            {
                DispatchEvents();
                scenes.Peek().Update(clock.Restart());
                Clear();
                Draw(scenes.Peek());
                Display();
            }
        }

        void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            scenes.Peek().Window_MouseWheelScrolled(sender, e);
        }

        void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        void Window_Resized(object sender, SizeEventArgs e)
        {
            View newView = new View(new FloatRect(0, 0, e.Width, e.Height));
            SetView(newView);
        }

        void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            scenes.Peek().Window_MouseButtonPressed(sender, e);
        }

        void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            scenes.Peek().Window_MouseMoved(sender, e);
        }

        void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            scenes.Peek().Window_KeyReleased(sender, e);
        }

        void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            scenes.Peek().Window_KeyPressed(sender, e);
        }
    }
}
