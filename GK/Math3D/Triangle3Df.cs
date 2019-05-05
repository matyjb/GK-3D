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
