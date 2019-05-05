using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.Math3D
{
    public class Vector3Df
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; } //for transforming

        public float Lenght { get => (float)Math.Sqrt(Dot(this)); }

        public Vector3Df()
        {
            X = Y = Z = 0;
            W = 1;
        }
        public Vector3Df(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }
        public static Vector3Df operator +(Vector3Df l, Vector3Df r)
        {
            return new Vector3Df(l.X+r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Vector3Df operator -(Vector3Df l, Vector3Df r)
        {
            return l +(-r);
        }
        public static Vector3Df operator -(Vector3Df r)
        {
            return new Vector3Df(-r.X,-r.Y,-r.Z);
        }
        public static Vector3Df operator *(float l, Vector3Df r)
        {
            return new Vector3Df(l * r.X, l * r.Y, l * r.Z);
        }
        public static Vector3Df operator *(Vector3Df l, float r)
        {
            return new Vector3Df(r * l.X, r * l.Y, r * l.Z);
        }
        public static Vector3Df operator /(float l, Vector3Df r)
        {
            return new Vector3Df(l / r.X, l / r.Y, l / r.Z);
        }
        public static Vector3Df operator /(Vector3Df l, float r)
        {
            return new Vector3Df(l.X / r, l.Y / r, l.Z / r);
        }
        public Vector3Df Cross(Vector3Df right)
        {
            float x = Y * right.Z - Z * right.Y;
            float y = Z * right.X - X * right.Z;
            float z = X * right.Y - Y * right.Z;
            return new Vector3Df(x, y, z);
        }
        public float Dot(Vector3Df right)
        {
            return X * right.X + Y * right.Y + Z * right.Z;
        }
        public Vector3Df Normal()
        {
            return this / Lenght;
        }
        public Vector3Df NormalW1()
        {
            return new Vector3Df(X / W, Y / W, Z / W);
        }
        public static explicit operator Vector2f(Vector3Df from)
        {
            return new Vector2f(from.X, from.Y);
        }
    }
}
