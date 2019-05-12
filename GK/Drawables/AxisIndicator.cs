using GK.Math3D;
using SFML.Graphics;
using SFML.System;

namespace GK.Drawables
{
    public class AxisIndicator : Transformable3D, Drawable
    {
        public Camera Camera { get; private set; }
        public AxisIndicator(Camera camera)
        {
            Camera = camera;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            Vector3Df vX = new Vector3Df(20, 0, 0);
            Vector3Df vY = new Vector3Df(0, 20, 0);
            Vector3Df vZ = new Vector3Df(0, 0, 20);

            Position = Camera.Position;
            Transform3D t = Camera.InverseTransform * Transform;
            vX = t * vX;
            vY = t * vY;
            vZ = t * vZ;


            //vertices
            Vertex[] vs = new Vertex[]
            {
                new Vertex(new Vector2f(vX.X,-vX.Y),Color.Red),
                new Vertex(new Vector2f(),Color.Red),
                new Vertex(new Vector2f(vY.X,-vY.Y),Color.Green),
                new Vertex(new Vector2f(),Color.Green),
                new Vertex(new Vector2f(vZ.X,-vZ.Y),Color.Blue),
                new Vertex(new Vector2f(),Color.Blue),
            };
            target.Draw(vs, PrimitiveType.Lines);

        }
    }
}
