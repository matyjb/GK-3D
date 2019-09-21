using SFML.System;
using System;

namespace GK.Math3D
{
    public struct Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public float Lenght { get => (float)Math.Sqrt(Dot(this)); }

        public Vec3(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }
        public static Vec3 operator +(Vec3 l, Vec3 r)
        {
            return new Vec3(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Vec3 operator -(Vec3 l, Vec3 r)
        {
            return l + (-r);
        }
        public static Vec3 operator -(Vec3 r)
        {
            return new Vec3(-r.X, -r.Y, -r.Z);
        }
        public static Vec3 operator *(float l, Vec3 r)
        {
            return new Vec3(l * r.X, l * r.Y, l * r.Z);
        }
        public static Vec3 operator *(Vec3 l, float r)
        {
            return new Vec3(r * l.X, r * l.Y, r * l.Z);
        }
        public static Vec3 operator /(float l, Vec3 r)
        {
            return new Vec3(l / r.X, l / r.Y, l / r.Z);
        }
        public static Vec3 operator /(Vec3 l, float r)
        {
            if (r == 0) return l;
            return new Vec3(l.X / r, l.Y / r, l.Z / r);
        }
        public static Vec3 operator %(float l, Vec3 r)
        {
            return new Vec3(l % r.X, l % r.Y, l % r.Z);
        }
        public static Vec3 operator %(Vec3 l, float r)
        {
            if (r == 0) return l;
            return new Vec3(l.X % r, l.Y % r, l.Z % r);
        }
        public Vec3 Cross(Vec3 right)
        {
            float x = Y * right.Z - Z * right.Y;
            float y = Z * right.X - X * right.Z;
            float z = X * right.Y - Y * right.X;
            return new Vec3(x, y, z);
        }
        public float Dot(Vec3 right)
        {
            return X * right.X + Y * right.Y + Z * right.Z;
        }
        public Vec3 Normal()
        {
            return this / Lenght;
        }
        public static explicit operator Vector2f(Vec3 from)
        {
            return new Vector2f(from.X, from.Y);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z},{W})";
        }
    }
}
