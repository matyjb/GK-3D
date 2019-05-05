using System;

namespace GK.Math3D
{
    public class Transform3D
    {
        public static Transform3D Identity { get => new Transform3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1); }
        public float[,] Matrix { get; private set; }// = Identity.Matrix;

        public Transform3D(float a00, float a01, float a02, float a03,
                           float a10, float a11, float a12, float a13,
                           float a20, float a21, float a22, float a23,
                           float a30, float a31, float a32, float a33)
        {
            Matrix = new float[4, 4] { { a00, a01, a02, a03 },
                                       { a10, a11, a12, a13 },
                                       { a20, a21, a22, a23 },
                                       { a30, a31, a32, a33 } };
        }
        public Transform3D(float[,] matrix)
        {
            if (matrix.GetLength(0) != 4) throw new Exception("długość wymiaru 0 macierzy musi być równy 4");
            if (matrix.GetLength(1) != 4) throw new Exception("długość wymiaru 1 macierzy musi być równy 4");
            Matrix = matrix;
        }
        public static Vector3Df operator*(Transform3D l, Vector3Df r)
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
            return new Vector3Df(result[0], result[1], result[2]) { W = result[3] };
        }
        public static Triangle3Df operator *(Transform3D l, Triangle3Df r)
        {
            Vector3Df v0 = l * r[0].Position;
            Vector3Df v1 = l * r[1].Position;
            Vector3Df v2 = l * r[2].Position;
            return new Triangle3Df(new Vertex3Df(v0, r[0].Color), new Vertex3Df(v1, r[1].Color), new Vertex3Df(v2, r[0].Color));
        }
        public Transform3D Translate(Vector3Df v)
        {
            Transform3D tmp = new Transform3D(1, 0, 0, v.X, 0, 1, 0, v.Y, 0, 0, 1, v.Z, 0, 0, 0, 1);
            Matrix = (tmp * this).Matrix;
            return this;
        }
        public Transform3D Rotate(Vector3Df angles)
        {
            float cosX = (float)Math.Cos(angles.X);
            float sinX = (float)Math.Sin(angles.X);
            float cosY = (float)Math.Cos(angles.Y);
            float sinY = (float)Math.Sin(angles.Y);
            float cosZ = (float)Math.Cos(angles.Z);
            float sinZ = (float)Math.Sin(angles.Z);
            Transform3D rotX = new Transform3D(1, 0, 0, 0,
                                               0, cosX, -sinX, 0,
                                               0, sinX, cosX, 0,
                                               0, 0, 0, 1);
            Transform3D rotY = new Transform3D(cosY, 0, -sinY, 0,
                                                  0, 1, 0, 0,
                                               sinY, 0, cosY, 0,
                                                  0, 0, 0, 1);
            Transform3D rotZ = new Transform3D(cosZ, -sinZ, 0, 0,
                                               sinZ, cosZ, 0, 0,
                                                  0, 0, 1, 0,
                                                  0, 0, 0, 1);
            Transform3D rot = rotY * rotX * rotZ;
            Matrix = (rot * this).Matrix;
            return this;
        }
        public Transform3D Scale(Vector3Df s)
        {
            Transform3D scl = new Transform3D(s.X, 0, 0, 0, 0, s.Y, 0, 0, 0, 0, s.Z, 0, 0, 0, 0, 1);
            Matrix = (scl * this).Matrix;
            return this;
        }

        public static Transform3D operator *(Transform3D left, Transform3D right)
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


            return new Transform3D(result);
        }
    }
}
