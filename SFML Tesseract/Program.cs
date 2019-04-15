using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Tesseract
{
    class Program
    {
        static void Main(string[] args)
        {
            //Matrix a = new Matrix(new double[3, 1] { { 1 }, { 2 }, { 4 } });
            //Matrix b = Matrix.RotationMatrix(90*Math.PI/180, new int[] { 0,1,0 });
            //Matrix r = b*a;

            //Console.WriteLine(a);
            //Console.WriteLine(b);
            //Console.WriteLine(r);
            Cube4D c = new Cube4D();
            Matrix r = Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 0, 1, 1 });
            r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 1, 0, 1 });
            r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 0, 0, 1 });
            r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 0, 1, 1, 0 });
            r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 0, 1, 0 });
            r *= Matrix.RotationMatrix(30 * Math.PI / 180, new int[] { 1, 1, 0, 0 });
            c.Points = r * c.Points;
            Console.WriteLine(c);
        }
    }
}
