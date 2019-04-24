using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SFML_Tesseract
{
    class Plane3D : Transformable3D, Drawable
    {
        public float ZCenterDistance { get => GetZDistance(); }


        public float Width { get; set; } = 400;
        public float Height { get; set; } = 400;

        public Color FillColor { get; set; } = Color.Black;
        public Color OutlineColor { get; set; } = Color.White;

        private float GetZDistance()
        {
            Vector3f center = new Vector3f(Width / 2, Height / 2, 0);
            Transform3D t = Camera.Instance.InverseTransform * ParentTransform * Transform;
            center = t.TransformPoint(center);
            // TODO: wyliczyc lepiej ten z
            return center.Z;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (ZCenterDistance  > 0)
            {
                Transform3D t = Camera.Instance.InverseTransform * ParentTransform * Transform;
                Vector3f[] v = new Vector3f[] { new Vector3f(0, 0, 0), new Vector3f(0, Height, 0), new Vector3f(Width, Height, 0), new Vector3f(Width, 0, 0) };
                float camDistance = Camera.Instance.Sdistance;
                for (int i = 0; i < v.Length; i++)
                {
                    v[i] = t.TransformPoint(v[i]);
                    Vector3f s = new Vector3f(camDistance / (v[i].Z + camDistance), camDistance / (v[i].Z + camDistance), 0);
                    v[i].X *= s.X;
                    v[i].Y *= s.Y;
                    v[i].Z *= s.Z;
                }

                List<Vertex> vs = new List<Vertex>
                {
                    new Vertex(new Vector2f(v[0].X,v[0].Y), FillColor),
                    new Vertex(new Vector2f(v[1].X,v[1].Y), FillColor),
                    new Vertex(new Vector2f(v[2].X,v[2].Y), FillColor),
                    new Vertex(new Vector2f(v[3].X,v[3].Y), FillColor),
                };
                RenderItem r = new RenderItem(vs.ToArray(), PrimitiveType.Quads,states);
                RenderEngine.Instance.RenderItems.Add(r);

                List<Vertex> vs2 = new List<Vertex>
                {
                    new Vertex(new Vector2f(v[0].X, v[0].Y), OutlineColor),
                    new Vertex(new Vector2f(v[1].X, v[1].Y), OutlineColor),
                    new Vertex(new Vector2f(v[2].X, v[2].Y), OutlineColor),
                    new Vertex(new Vector2f(v[3].X, v[3].Y), OutlineColor),
                    new Vertex(new Vector2f(v[0].X, v[0].Y), OutlineColor),
                };

                RenderItem r2 = new RenderItem(vs2.ToArray(), PrimitiveType.LineStrip,states);
                RenderEngine.Instance.RenderItems.Add(r2);
            }
        }
    }
}
