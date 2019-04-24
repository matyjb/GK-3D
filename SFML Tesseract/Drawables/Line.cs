using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SFML_Tesseract.Drawables
{
    class Line : Transformable3D, Drawable
    {
        public Color Color { get; set; } = Color.Red;
        public Vector3f Position2 { get; set; } = new Vector3f();
        public void Draw(RenderTarget target, RenderStates states)
        {
            Transform3D t = Camera.Instance.InverseTransform * ParentTransform * Transform;
            Vector3f[] v = new Vector3f[] { Position, Position2 };
            Vector2f[] vectors = RenderEngine.PerspectiveView(t, v);

            List<Vertex> vs = new List<Vertex>
            {
                new Vertex(vectors[0], Color),
                new Vertex(vectors[1], Color),
            };
            RenderItem r = new RenderItem(vs.ToArray(), PrimitiveType.Lines, states);
            RenderEngine.Instance.RenderItems.Add(r);
        }
    }
}
