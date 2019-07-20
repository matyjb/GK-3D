using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GK.Math3D
{
    public struct Triangle : IEnumerable<Vertex3>
    {
        public Vertex3 v0 { get; set; }
        public Vertex3 v1 { get; set; }
        public Vertex3 v2 { get; set; }

        //light
        public float kd { get; set; }
        public float ks { get; set; }
        public float n { get; set; }


        public Vec3 NormalVector { get {
                Vec3 l0 = v1.Position - v0.Position;
                Vec3 l1 = v2.Position - v0.Position;
                return l0.Cross(l1).Normal();
            } }
        public Vec3 Center
        {
            get
            {
                return (v0.Position + v1.Position + v2.Position) / 3;
            }
        }

        public Vertex3 this[int index]
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

        public Triangle(Vertex3 v0, Vertex3 v1, Vertex3 v2, float ks = 1,float kd = 1, float n = 10)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.ks = ks;
            this.kd = kd;
            this.n = n;
        }
        public Triangle(Triangle t)
        {
            v0 = t.v0;
            v1 = t.v1;
            v2 = t.v2;
            ks = t.ks;
            kd = t.kd;
            n = t.n;
        }
        public Triangle(Vec3 v0, Vec3 v1, Vec3 v2, Vec4Color color)
            :this(new Vertex3(v0,color),new Vertex3(v1,color),new Vertex3(v2,color))
        {

        }
        public IEnumerator<Vertex3> GetEnumerator()
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
