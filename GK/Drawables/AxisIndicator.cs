using SFML.Graphics;
using SFML.System;
using GK.DrawingView;
using GK.Transforming;

namespace GK.Drawables
{
    class AxisIndicator : Transformable3D, Drawable
    {
        public void Draw(RenderTarget target, RenderStates states)
        {
            Vector3f vX = new Vector3f(20, 0, 0);
            Vector3f vY = new Vector3f(0, -20, 0);
            Vector3f vZ = new Vector3f(0, 0, 20);

            Position = Camera.Instance.Position;
            Transform3D t = Camera.Instance.InverseTransform * Transform;
            vX = t.TransformPoint(vX);
            vY = t.TransformPoint(vY);
            vZ = t.TransformPoint(vZ);


            //vertices
            Vertex[] vs = new Vertex[]
            {
                new Vertex(new Vector2f(vX.X,-vX.Y),Color.Red),
                new Vertex(new Vector2f(),Color.Red),
                new Vertex(new Vector2f(-vY.X,vY.Y),Color.Green),
                new Vertex(new Vector2f(),Color.Green),
                new Vertex(new Vector2f(vZ.X,-vZ.Y),Color.Blue),
                new Vertex(new Vector2f(),Color.Blue),
            };
            target.Draw(vs, PrimitiveType.Lines);

        }
    }
}
