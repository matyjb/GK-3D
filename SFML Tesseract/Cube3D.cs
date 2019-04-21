using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace SFML_Tesseract
{
    class Cube3D : Transformable3D, Drawable
    {
        private float Size { get; set; } = 400;
        public Color FillColor { get; set; }
        public Color OutlineColor { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {

        }
    }
}
