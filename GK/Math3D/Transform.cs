using System;

namespace GK.Math3D
{
    public struct Transform
    {
        public static Transform Identity { get => new Transform(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1); }
        public float[,] Matrix { get; private set; }

        public Transform(float a00, float a01, float a02, float a03,
                           float a10, float a11, float a12, float a13,
                           float a20, float a21, float a22, float a23,
                           float a30, float a31, float a32, float a33)
        {
            Matrix = new float[4, 4] { { a00, a01, a02, a03 },
                                       { a10, a11, a12, a13 },
                                       { a20, a21, a22, a23 },
                                       { a30, a31, a32, a33 } };
        }
        public Transform(float[,] matrix)
        {
            if (matrix.GetLength(0) != 4) throw new Exception("długość wymiaru 0 macierzy musi być równy 4");
            if (matrix.GetLength(1) != 4) throw new Exception("długość wymiaru 1 macierzy musi być równy 4");
            Matrix = matrix;
        }
        
        public Transform Translate(Vec3 v)
        {
            Transform tmp = new Transform(1, 0, 0, v.X, 0, 1, 0, v.Y, 0, 0, 1, v.Z, 0, 0, 0, 1);
            Matrix = (tmp * this).Matrix;
            return this;
        }
        public Transform Rotate(Vec3 anglesRad)
        {
            float cosX = (float)Math.Cos(anglesRad.X);
            float sinX = (float)Math.Sin(anglesRad.X);
            float cosY = (float)Math.Cos(anglesRad.Y);
            float sinY = (float)Math.Sin(anglesRad.Y);
            float cosZ = (float)Math.Cos(anglesRad.Z);
            float sinZ = (float)Math.Sin(anglesRad.Z);
            Transform rotX = new Transform(1, 0, 0, 0,
                                               0, cosX, -sinX, 0,
                                               0, sinX, cosX, 0,
                                               0, 0, 0, 1);
            Transform rotY = new Transform(cosY, 0, -sinY, 0,
                                                  0, 1, 0, 0,
                                               sinY, 0, cosY, 0,
                                                  0, 0, 0, 1);
            Transform rotZ = new Transform(cosZ, -sinZ, 0, 0,
                                               sinZ, cosZ, 0, 0,
                                                  0, 0, 1, 0,
                                                  0, 0, 0, 1);
            Transform rot = rotY * rotX * rotZ;
            Matrix = (rot * this).Matrix;
            return this;
        }
        public Transform Scale(Vec3 s)
        {
            Transform scl = new Transform(s.X, 0, 0, 0, 0, s.Y, 0, 0, 0, 0, s.Z, 0, 0, 0, 0, 1);
            Matrix = (scl * this).Matrix;
            return this;
        }
        public static Vec3 operator *(Transform l, Vec3 r)
        {
            float[] tmp = new float[] { r.X, r.Y, r.Z, r.W };
            float[] result = new float[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                float t = 0;
                for (int j = 0; j < 4; j++)
                {
                    t += l.Matrix[i, j] * tmp[j];
                }
                result[i] = t;
            }
            if(result[3] != 0)
                return new Vec3(result[0], result[1], result[2]) / result[3];
            else
                return new Vec3(result[0], result[1], result[2]);
        }
        public static Vertex3 operator *(Transform l, Vertex3 r)
        {
            Vec3 v = l * r.Position;
            return new Vertex3(v, r.Color);
        }

        public static Tri operator *(Transform l, Tri r)
        {
            Tri result = new Tri(r);
            result.v0 = l * r.v0;
            result.v1 = l * r.v1;
            result.v2 = l * r.v2;
            return result;
        }
        public static Transform operator *(Transform left, Transform right)
        {
            float[,] result = new float[4, 4];
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    for (int k = 0; k < 4; k++)
                        result[row, col] = result[row, col] + left.Matrix[row, k] * right.Matrix[k, col];
                }
            }


            return new Transform(result);
        }
    }
}
