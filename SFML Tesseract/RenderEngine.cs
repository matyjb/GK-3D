using SFML.Graphics;
using System.Collections.Generic;

namespace SFML_Tesseract
{
    public class RenderItem
    {
        public Vertex[] Vertices { get; set; }
        public PrimitiveType Type { get; set; }
        public RenderStates States { get; set; }
    }
    public sealed class RenderEngine : Drawable
    {
        public static RenderEngine Instance { get; } = new RenderEngine();

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
