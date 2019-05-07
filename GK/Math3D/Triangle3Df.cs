using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GK.Math3D
{
    public struct Triangle3Df : IEnumerable<Vertex3Df>
    {
        public Vertex3Df v0 { get; set; }
        public Vertex3Df v1 { get; set; }
        public Vertex3Df v2 { get; set; }

        public Vertex3Df this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return v0;
                    case 1: return v1;
                    case 2: return v2;
                    default: throw new IndexOutOfRangeException("index must be 0, 1 or 2");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: v0 = value; break;
                    case 1: v1 = value; break;
                    case 2: v2 = value; break;
                    default: throw new IndexOutOfRangeException("index must be 0, 1 or 2");
                }
            }
        }

        public Triangle3Df(Vertex3Df v0, Vertex3Df v1, Vertex3Df v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
        public FloatRect BoundingBox2D()
        {
            float minx, miny, maxx, maxy;
            maxx = maxy = float.MinValue;
            minx = miny = float.MaxValue;
            foreach (var vert in this)
            {
                minx = Math.Min(minx, vert.Position.X);
                miny = Math.Min(miny, vert.Position.Y);
                maxx = Math.Max(maxx, vert.Position.X);
                maxy = Math.Max(maxy, vert.Position.Y);
            }
            return new FloatRect(minx, miny, maxx - minx, maxy - miny);
        }
        public bool PointIsInside(float x, float y, out float w1, out float w2)
        {
            float s1 = v2.Position.Y - v0.Position.Y;
            float s2 = v2.Position.X - v0.Position.X;
            float s3 = v1.Position.Y - v0.Position.Y;
            float s4 = y - v0.Position.Y;

            w1 = (v0.Position.X * s1 + s4 * s2 - x * s1) / (s3 * s2 - (v1.Position.X - v0.Position.X) * s1);
            w2 = (s4 - w1 * s3) / s1;
            return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
        }
        public bool PointIsInside(float x, float y)
        {
            return PointIsInside(x, y, out var t0, out var t1);
        }
        public float GetZ(float x, float y)
        {
            //check if inside 
            bool inside = PointIsInside(x, y, out float w1, out float w2);
            if (!inside) return float.MinValue;

            //is inside
            float w0 = 1 - w1 - w2;
            return v0.Position.Z * w0 + v1.Position.Z * w1 + v2.Position.Z * w2;
        }
        public IEnumerator<Vertex3Df> GetEnumerator()
        {
            yield return v0;
            yield return v1;
            yield return v2;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
