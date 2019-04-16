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
    class Cube3D : Drawable
    {
        public Vector3f Position { get; set; }
        public float Size { get; set; } = 100;
        public void Draw(RenderTarget target, RenderStates states)
        {
            Vector3f[] v = new Vector3f[]
            {
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
                new Vector3f(0,0,0),
            };
            //gen points
            for (int i = 0; i < v.Length; i++)
            {
                v[i].X = Convert.ToInt16(Convert.ToString(i, 2).PadLeft(3,'0')[2].ToString());
                v[i].Y = Convert.ToInt16(Convert.ToString(i, 2).PadLeft(3, '0')[1].ToString());
                v[i].Z = Convert.ToInt16(Convert.ToString(i, 2).PadLeft(3, '0')[0].ToString());
                v[i] *= Size;
                v[i] += Position - Camera.Instance.Position;
            }
            ////////////

            //Matrix rzut = new Matrix((double[,])Points.M.Clone());
            //for (int row = 0; row < rzut.Rows; row++)//rzut na 0XY
            //{
            //    if (row < rzut.Rows - 1)
            //        for (int col = 0; col < rzut.Columns; col++)
            //        {
            //            rzut.M[row, col] *= 100 / (rzut.M[rzut.Rows - 1, col] + 100);
            //        }
            //    else
            //        for (int col = 0; col < rzut.Columns; col++)
            //        {
            //            rzut.M[row, col] = 0;
            //        }
            //}
            for (int i = 0; i < v.Length; i++)
            {
                float camDistance = Camera.Instance.Sdistance;
                Vector3f s = new Vector3f(camDistance / (v[i].Z + camDistance), camDistance / (v[i].Z + camDistance), 0);
                v[i].X *= s.X;
                v[i].Y *= s.Y;
                v[i].Z *= s.Z;
            }

            for (int dl = 1; dl < v.Length; dl*=2)
            {
                bool[] done = new bool[v.Length];
                for (int i = 0; i < v.Length-dl; i++)
                {
                    if(!(done[i] || done[i + dl]))
                    {
                        done[i] = true;
                        done[i+dl] = true;
                        Vector2f a = new Vector2f(v[i].X, v[i].Y);
                        Vector2f b = new Vector2f(v[i+dl].X, v[i+dl].Y);
                        target.Draw(new Vertex[] { new Vertex(a), new Vertex(b) }, PrimitiveType.Lines);
                    }
                }
            }

        }

        //public override string ToString()
        //{
        //      return Points.ToString();
        //}
    }
}
