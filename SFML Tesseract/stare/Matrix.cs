//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SFML_Tesseract
//{
//    class Matrix
//    {
//        public double[,] M { get; set; }
//        public int Rows { get => M.GetLength(0); }
//        public int Columns { get => M.GetLength(1); }

//        public Matrix() : this(new double[1, 1]) { }
//        public Matrix(double[,] m)
//        {
//            M = m;
//        }

//        public static Matrix RotationMatrix(double fi, int[] v)
//        {
//            //if (v.Length != Columns) throw new Exception("zle dlugosc vektora okreslajace wokol czego krecimy");
//            int s = 0;
//            foreach (var item in v)  s += item;
//            if (s != v.Length-2) throw new Exception("suma w wektorze nie bardzo");

//            Stack<double> stk = new Stack<double>();
//            stk.Push(Math.Cos(fi));
//            stk.Push(Math.Sin(fi));
//            stk.Push(-Math.Sin(fi));
//            stk.Push(Math.Cos(fi));

//            Matrix result = new Matrix(new double[v.Length, v.Length]);
//            for (int i = 0; i < v.Length; i++)
//            {
//                for (int j = 0; j < v.Length; j++)
//                {
//                    if (i == j) result.M[i, j] = 1;
//                    if (v[i] != 1 && v[j] != 1)
//                        result.M[i, j] = stk.Pop();
//                }
//            }

//            return result;
//        }
//        public override string ToString()
//        {
//            string s = "";
//            for (int i = 0; i < Rows; i++)
//            {
//                for (int j = 0; j < Columns; j++)
//                {
//                    s += string.Format("{0:0.00}", M[i, j]) + ((j<Columns-1)?";":"");
//                }
//                s += Environment.NewLine;
//            }
//            return s;
//        }

        
//        public static Matrix operator *(Matrix l, Matrix r)
//        {
//            if (l.Columns != r.Rows) throw new Exception("matrices do not match correct dimensions");
//            double[,] result = new double[l.Rows, r.Columns];
//            for (int row = 0; row < result.GetLength(0); row++)
//            {
//                for (int col = 0; col < result.GetLength(1); col++)
//                {
//                    //result[row, col] = 0;
//                    for (int k = 0; k < l.Columns; k++) // OR k<b.GetLength(0)
//                        result[row, col] = result[row, col] + l.M[row, k] * r.M[k, col];
//                }
//            }


//            return new Matrix(result);
//        }
//    }
//}
