using SFML.Graphics;
using System;

namespace GK.Math3D
{
    /// <summary>
    /// Color vector where colors are represented by 0 to 1 float value
    /// </summary>
    public struct Vec4
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public float Lenght { get => (float)Math.Sqrt(Dot(this)); }

        /// <summary>
        /// Color vector where colors are represented by 0 to 1 float value
        /// Values will be clamped to 0 - 1 when converting from Color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Vec4(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public static Vec4 operator +(Vec4 l, Vec4 r)
        {
            return new Vec4(l.R + r.R, l.G + r.G, l.B + r.B, l.A + r.A);
        }
        public static Vec4 operator -(Vec4 l, Vec4 r)
        {
            return l + (-r);
        }
        public static Vec4 operator -(Vec4 r)
        {
            return new Vec4(-r.R, -r.G, -r.B, -r.A);
        }
        public static Vec4 operator *(float l, Vec4 r)
        {
            return new Vec4(l * r.R, l * r.G, l * r.B, l * r.A);
        }
        public static Vec4 operator *(Vec4 l, float r)
        {
            return new Vec4(r * l.R, r * l.G, r * l.B, r * l.A);
        }
        public static Vec4 operator /(float l, Vec4 r)
        {
            return new Vec4(l / r.R, l / r.G, l / r.B, l / r.A);
        }
        public static Vec4 operator /(Vec4 l, float r)
        {
            if (r == 0) return l;
            return new Vec4(l.R / r, l.G / r, l.B / r, l.A / r);
        }
        public float Dot(Vec4 right)
        {
            return R * right.R + G * right.G + B * right.B + A * right.A;
        }
        public Vec4 Normal()
        {
            return this / Lenght;
        }
        public static explicit operator Color(Vec4 from)
        {
            byte R = (byte)Math.Max(Math.Min(Math.Round(255 * from.R),255),0);
            byte G = (byte)Math.Max(Math.Min(Math.Round(255 * from.G),255),0);
            byte B = (byte)Math.Max(Math.Min(Math.Round(255 * from.B),255),0);
            byte A = (byte)Math.Max(Math.Min(Math.Round(255 * from.A),255),0);
            return new Color(R, G, B, A);
        }
        public static explicit operator Vec4(Color from)
        {
            float R = from.R/255f;
            float G = from.G/255f;
            float B = from.B/255f;
            float A = from.A/255f;
            return new Vec4(R, G, B, A);
        }

        public override string ToString()
        {
            return string.Format("R:{0:0.00},G:{1:0.00},B:{2:0.00},A:{3:0.00}", R,G,B,A);
        }
    }
}
