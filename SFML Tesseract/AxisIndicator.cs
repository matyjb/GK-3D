using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Tesseract
{
    class AxisIndicator : Transformable3D, Drawable
    {
        public void Draw(RenderTarget target, RenderStates states)
        {
            Vector3f vX = new Vector3f(20, 0, 0);
            Vector3f vY = new Vector3f(0, 20, 0);
            Vector3f vZ = new Vector3f(0, 0, 20);

            //rotate by camera rotation and move to screen center
            //Transform3D r = Transform3D.Identity.Rotate(Camera.Instance.Rotation);
            //vX = r.TransformPoint(vX);
            //vY = r.TransformPoint(vY);
            //vZ = r.TransformPoint(vZ);
            Position = Camera.Instance.Position;
            Transform3D t = Camera.Instance.InverseTransform * Transform;
            Vector3f[] v = new Vector3f[] { vX, vY, vZ, new Vector3f() };
            float camDistance = Camera.Instance.Sdistance;
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = t.TransformPoint(v[i]);
                Vector3f s = new Vector3f(camDistance / (v[i].Z + camDistance), camDistance / (v[i].Z + camDistance), 0);
                v[i].X *= s.X;
                v[i].Y *= s.Y;
                v[i].Z *= s.Z;
            }


            //vertices
            Vertex[] vs = new Vertex[]
            {
                new Vertex(new Vector2f(v[0].X,v[0].Y), Color.Red),
                    new Vertex(new Vector2f(v[3].X,v[3].Y), Color.Red),
                    new Vertex(new Vector2f(v[1].X,v[1].Y), Color.Green),
                    new Vertex(new Vector2f(v[3].X,v[3].Y), Color.Green),
                    new Vertex(new Vector2f(v[2].X,v[2].Y), Color.Blue),
                    new Vertex(new Vector2f(v[3].X,v[3].Y), Color.Blue),
                //new Vertex(new Vector2f(vX.X,vX.Y),Color.Red),
                //new Vertex(new Vector2f(),Color.Red),
                //new Vertex(new Vector2f(vY.X,vY.Y),Color.Green),
                //new Vertex(new Vector2f(),Color.Green),
                //new Vertex(new Vector2f(vZ.X,vZ.Y),Color.Blue),
                //new Vertex(new Vector2f(),Color.Blue),
            };
            target.Draw(vs, PrimitiveType.Lines);

        }
    }
}
