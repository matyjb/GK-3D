using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public class Scene : Drawable
    {
        public Camera Camera { get; }
        //public List<IDrawable3D> Drawables = new List<IDrawable3D>()
        //{

        //};

        public void Draw(RenderTarget target, RenderStates states)
        {
            /*
             1. uzyskać od wszystkich Drawables trojkaty tzn List<Triangle3Df>
             2. wpakować wszsytkie do jednej listy
             3. wszystkie trojkaty z listy pomnozyc o tak:
                Camera.ProjectionMatrix*Camera.InverseTransform*Triangle
             //(malarski (ale głupi))
             4. posortować po Z
             5. dla kazdego trojkata zrzutować Vertexy3D na Vertex i wstawić do Vertex[]
             6. kazdy vector jeszcze trzeba pomnozyc przez [width, -Height]
             //zbufor

             //
             6. target.draw
             */
        }

    }
}
