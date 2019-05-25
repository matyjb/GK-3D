using System.Collections;
using System.Collections.Generic;

namespace GK.Math3D
{
    public class Mesh : IEnumerable<Triangle>
    {
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public void Add(Triangle t)
        {
            Triangles.Add(t);
        }

        public IEnumerator<Triangle> GetEnumerator()
        {
            foreach (var t in Triangles)
                yield return t;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
