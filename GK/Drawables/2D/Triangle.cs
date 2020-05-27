using GK.Interfaces;
using GK.Math3D;

namespace GK.Drawables._2D
{
    class Triangle : Transformable, Drawable3D
    {
        public Vertex3 v0 { get; set; }
        public Vertex3 v1 { get; set; }
        public Vertex3 v2 { get; set; }

        //light
        public float kd { get; set; }
        public float ks { get; set; }
        public float n { get; set; }

        public Triangle(Vertex3 v0, Vertex3 v1, Vertex3 v2, float ks = 1, float kd = 1, float n = 10)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.ks = ks;
            this.kd = kd;
            this.n = n;
        }
        public Triangle(Tri t)
        {
            v0 = t.v0;
            v1 = t.v1;
            v2 = t.v2;
            ks = t.ks;
            kd = t.kd;
            n = t.n;
        }
        public Triangle(Vec3 v0, Vec3 v1, Vec3 v2, Vec4 color)
            : this(new Vertex3(v0, color), new Vertex3(v1, color), new Vertex3(v2, color))
        { }
        public Mesh GetMesh()
        {
            Mesh m = new Mesh()
            {
                new Tri(v0,v1,v2,ks,kd,n),
                new Tri(v0,v2,v1,ks,kd,n)
            };
            m.ParentTransform = Transform;
            return m;
        }
    }
}
