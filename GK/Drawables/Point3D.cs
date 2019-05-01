using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace GK.Drawables
{
    class Point3D : Transformable3D, Drawable
    {
        public Color Color { get; set; } = Color.Red;
        public void Draw(RenderTarget target, RenderStates states)
        {
            Transform3D t = Camera.Instance.InverseTransform * ParentTransform * Transform;
            Vector3f[] v = new Vector3f[] { Position };
            Vector2f[] vectors = RenderEngine.PerspectiveView(t, v);

            List<Vertex> vs = new List<Vertex>
            {
                new Vertex(vectors[0], Color),
            };
            RenderItem r = new RenderItem(vs.ToArray(), PrimitiveType.Points, states);
            RenderEngine.Instance.RenderItems.Add(r);

        }
    }
}
