//using SFML.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SFML_Tesseract
//{
//    class Cube4D : Drawable
//    {
//        public Matrix Points { get; set; } = new Matrix(new double[4, 16]
//        {
//            {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1 },
//            {0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,1 },
//            {0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1 },
//            {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1 },
//        });

//        public Cube4D(double scale=100)
//        {
//            for (int row = 0; row < Points.Rows; row++)
//            {
//                for (int col = 0; col < Points.Columns; col++)
//                {
//                    Points.M[row, col] *= scale;
//                    Points.M[row, col] += scale;
//                }
//            }
//        }
//        public void Draw(RenderTarget target, RenderStates states)
//        {
//            Matrix rzut = new Matrix((double[,])Points.M.Clone());
//            for (int row = 0; row < rzut.Rows; row++)//rzut na 0XYZ
//            {
//                if(row<rzut.Rows-1)
//                    for (int col = 0; col < rzut.Columns; col++)
//                    {
//                        rzut.M[row, col] *= -100/(rzut.M[rzut.Rows-1,col]-100);
//                    }
//                else
//                    for (int col = 0; col < rzut.Columns; col++)
//                    {
//                        rzut.M[row, col] = 0;
//                    }
//            }
//            for (int row = 0; row < rzut.Rows; row++)//rzut na 0XY
//            {
//                if (row < rzut.Rows - 2)
//                    for (int col = 0; col < rzut.Columns; col++)
//                    {
//                        rzut.M[row, col] *= -100 / (rzut.M[rzut.Rows - 2, col] - 100);
//                    }
//                else
//                    for (int col = 0; col < rzut.Columns; col++)
//                    {
//                        rzut.M[row, col] = 0;
//                    }
//            }

//        }

//        public override string ToString()
//        {
//            return Points.ToString();
//        }
//    }
//}
