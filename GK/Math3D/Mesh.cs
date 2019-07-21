using GK.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace GK.Math3D
{
    public class Mesh : Transformable, IEnumerable<Tri>, Drawable3D
    {
        public List<Tri> Triangles { get; set; } = new List<Tri>();

        public void Add(Tri t)
        {
            Triangles.Add(t);
        }

        public IEnumerator<Tri> GetEnumerator()
        {
            foreach (var t in Triangles)
                yield return t;
        }

        public Mesh GetMesh()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
