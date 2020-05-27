using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using Transform = GK.Math3D.Transform;
using Transformable = GK.Math3D.Transformable;

namespace GK.Drawables
{
    public class AxisIndicator : Transformable, Drawable
    {
        public void Draw(RenderTarget target, RenderStates states)
        {
            MainWindow window = (MainWindow)target;
            Vec3 vX = new Vec3(-20, 0, 0);
            Vec3 vY = new Vec3(0, -20, 0);
            Vec3 vZ = new Vec3(0, 0, -20);
            Vec3 cameraCenter = new Vec3(target.Size.X/ 2, target.Size.Y / 2, 0);

            Camera sceneCamera = window.scenes.Peek().mainCamera;
            Position = sceneCamera.Position;
            Transform t = sceneCamera.InverseTransform * Transform;
            t.Translate(cameraCenter);
            vX = t * vX;
            vY = t * vY;
            vZ = t * vZ;

            //vertices
            Vertex[] vs = new Vertex[]
            {
                new Vertex(new Vector2f(vX.X,vX.Y),Color.Red),
                new Vertex(new Vector2f(cameraCenter.X,cameraCenter.Y),Color.Red),
                new Vertex(new Vector2f(vY.X,vY.Y),Color.Green),
                new Vertex(new Vector2f(cameraCenter.X,cameraCenter.Y),Color.Green),
                new Vertex(new Vector2f(vZ.X,vZ.Y),Color.Blue),
                new Vertex(new Vector2f(cameraCenter.X,cameraCenter.Y),Color.Blue),
            };
            target.Draw(vs, PrimitiveType.Lines);
        }
    }
}
