using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SFML_Tesseract
{
    public class RenderItem
    {
        public Vertex[] Vertices { get; set; }
        public PrimitiveType Type { get; set; }
        public RenderStates States { get; set; }

        public RenderItem(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            Vertices = vertices;
            Type = type;
            States = states;
        }
    }
    public sealed class RenderEngine : Drawable
    {
        public static RenderEngine Instance { get; } = new RenderEngine();
        public static Vector2f[] PerspectiveView(Transform3D finalTransform, Vector3f[] vertors)
        {
            float camDistance = Camera.Instance.Sdistance;
            List<Vector2f> result = new List<Vector2f>();
            for (int i = 0; i < vertors.Length; i++)
            {
                vertors[i] = finalTransform.TransformPoint(vertors[i]);
                Vector3f s = new Vector3f(camDistance / (vertors[i].Z + camDistance), camDistance / (vertors[i].Z + camDistance), 0);
                vertors[i].X *= s.X;
                vertors[i].Y *= s.Y;
                vertors[i].Z *= s.Z;
                result.Add(new Vector2f(vertors[i].X, vertors[i].Y));
            }
            return result.ToArray();
        }
        public List<RenderItem> RenderItems { get; } = new List<RenderItem>();
        static RenderEngine()
        {
        }

        private RenderEngine()
        {
        }

        public void Draw(RenderTarget target, RenderStates states)
        {

            // TODO: algorytm sortowania 
            foreach (RenderItem item in RenderItems)
            {
                target.Draw(item.Vertices, item.Type, item.States);
            }
            RenderItems.Clear();
        }
    }
}
