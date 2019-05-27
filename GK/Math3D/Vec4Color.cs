using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.Math3D
{
    /// <summary>
    /// Color vector where colors are represented by 0 to 1 float value
    /// </summary>
    public struct Vec4Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public float Lenght { get => (float)Math.Sqrt(Dot(this)); }

        /// <summary>
        /// Color vector where colors are represented by 0 to 1 float value
        /// Values will be clamped to 0 - 1 when converting to Color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Vec4Color(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public static Vec4Color operator +(Vec4Color l, Vec4Color r)
        {
            return new Vec4Color(l.R + r.R, l.G + r.G, l.B + r.B);
        }
        public static Vec4Color operator -(Vec4Color l, Vec4Color r)
        {
            return l + (-r);
        }
        public static Vec4Color operator -(Vec4Color r)
        {
            return new Vec4Color(-r.R, -r.G, -r.B);
        }
        public static Vec4Color operator *(float l, Vec4Color r)
        {
            return new Vec4Color(l * r.R, l * r.G, l * r.B);
        }
        public static Vec4Color operator *(Vec4Color l, float r)
        {
            return new Vec4Color(r * l.R, r * l.G, r * l.B);
        }
        public static Vec4Color operator /(float l, Vec4Color r)
        {
            return new Vec4Color(l / r.R, l / r.G, l / r.B);
        }
        public static Vec4Color operator /(Vec4Color l, float r)
        {
            if (r == 0) return l;
            return new Vec4Color(l.R / r, l.G / r, l.B / r);
        }
        public float Dot(Vec4Color right)
        {
            return R * right.R + G * right.G + B * right.B + A * right.A;
        }
        public Vec4Color Normal()
        {
            return this / Lenght;
        }
        public static explicit operator Color(Vec4Color from)
        {
            byte R = (byte)Math.Max(Math.Min(Math.Round(255 * from.R),255),0);
            byte G = (byte)Math.Max(Math.Min(Math.Round(255 * from.G),255),0);
            byte B = (byte)Math.Max(Math.Min(Math.Round(255 * from.B),255),0);
            byte A = (byte)Math.Max(Math.Min(Math.Round(255 * from.A),255),0);
            return new Color(R, G, B, A);
        }
        public static explicit operator Vec4Color(Color from)
        {
            float R = from.R/255f;
            float G = from.G/255f;
            float B = from.B/255f;
            float A = from.A/255f;
            return new Vec4Color(R, G, B, A);
        }
    }
}
