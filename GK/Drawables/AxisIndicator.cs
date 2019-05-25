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
            Vec3 vX = new Vec3(-20, 0, 0);
            Vec3 vY = new Vec3(0, -20, 0);
            Vec3 vZ = new Vec3(0, 0, -20);
            Vec3 cameraCenter = new Vec3(Camera.Instance.Width / 2, Camera.Instance.Height / 2);

            Position = Camera.Instance.Position;
            Transform t = Camera.Instance.InverseTransform * Transform;
            t.Translate(cameraCenter);
            vX = t * vX;
            vY = t * vY;
            vZ = t * vZ;


            //vertices
            Vertex[] vs = new Vertex[]
            {
                new Vertex((Vector2f)vX,Color.Red),
                new Vertex((Vector2f)cameraCenter,Color.Red),
                new Vertex((Vector2f)vY,Color.Green),
                new Vertex((Vector2f)cameraCenter,Color.Green),
                new Vertex((Vector2f)vZ,Color.Blue),
                new Vertex((Vector2f)cameraCenter,Color.Blue),
            };
            target.Draw(vs, PrimitiveType.Lines);

        }
    }
}
