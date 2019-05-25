using SFML.System;
using System;

namespace GK.Math3D
{
    public struct Vec2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }

        public float Lenght { get => (float)Math.Sqrt(Dot(this)); }

        public Vec2(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
            W = 1;
        }
        public static Vec2 operator +(Vec2 l, Vec2 r)
        {
            return new Vec2(l.X + r.X, l.Y + r.Y);
        }
        public static Vec2 operator -(Vec2 l, Vec2 r)
        {
            return l + (-r);
        }
        public static Vec2 operator -(Vec2 r)
        {
            return new Vec2(-r.X, -r.Y);
        }
        public static Vec2 operator *(float l, Vec2 r)
        {
            return new Vec2(l * r.X, l * r.Y);
        }
        public static Vec2 operator *(Vec2 l, float r)
        {
            return new Vec2(r * l.X, r * l.Y);
        }
        public static Vec2 operator /(float l, Vec2 r)
        {
            return new Vec2(l / r.X, l / r.Y);
        }
        public static Vec2 operator /(Vec2 l, float r)
        {
            if (r == 0) return l;
            return new Vec2(l.X / r, l.Y / r);
        }
        public float Cross(Vec2 right)
        {
            return X * right.Y - Y * right.X;
        }
        public float Dot(Vec2 right)
        {
            return X * right.Y + Y * right.X;
        }
        public Vec2 Normal()
        {
            return this / Lenght;
        }
        public static explicit operator Vector2f(Vec2 from)
        {
            return new Vector2f(from.X, from.Y);
        }
    }
}
