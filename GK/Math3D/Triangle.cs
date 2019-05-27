using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GK.Math3D
{
    public struct Triangle : IEnumerable<Vec3>
    {
        public Vec3 v0 { get; set; }
        public Vec3 v1 { get; set; }
        public Vec3 v2 { get; set; }

        public Color Color { get; set; }

        public Vec3 NormalVector { get {
                Vec3 l0 = v1 - v0;
                Vec3 l1 = v2 - v0;
                return l0.Cross(l1).Normal();
            } }
        public Vec3 Center
        {
            get
            {
                return (v0 + v1 + v2) / 3;
            }
        }

        public Vec3 this[int index]
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

        public Triangle(Vec3 v0, Vec3 v1, Vec3 v2, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            Color = color;
        }
        public Triangle(Triangle t)
        {
            v0 = t.v0;
            v1 = t.v1;
            v2 = t.v2;
            Color = t.Color;
        }
        public Triangle(Vec3 v0, Vec3 v1, Vec3 v2):this(v0,v1,v2,Color.White)
        {
        }
        public IEnumerator<Vec3> GetEnumerator()
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
