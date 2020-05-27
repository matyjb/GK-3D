using GK.Math3D;
using System;
using System.Collections.Generic;

namespace GK.Drawables
{
    public class Sphere : Mesh
    {
        public Sphere(float radius, uint subdivisions, Vec4 color)
        {
            Mesh ico20 = BuildIcosahedron(radius, color);
            Queue<Tri> tris = new Queue<Tri>();
            foreach (var item in ico20) tris.Enqueue(item);

            uint j = 0;
            while(subdivisions > j)
            {
                for (int i = 0; i < 20 * Math.Pow(4,j); i++)
                {
                    Tri t = tris.Dequeue();
                    Vertex3 newV0 = ComputeHalfVertex(t.v0, t.v1, radius);
                    Vertex3 newV1 = ComputeHalfVertex(t.v1, t.v2, radius);
                    Vertex3 newV2 = ComputeHalfVertex(t.v2, t.v0, radius);

                    tris.Enqueue(new Tri(t.v0, newV0, newV2, t.ks, t.kd, t.n));
                    tris.Enqueue(new Tri(t.v1, newV1, newV0, t.ks, t.kd, t.n));
                    tris.Enqueue(new Tri(t.v2, newV2, newV1, t.ks, t.kd, t.n));
                    tris.Enqueue(new Tri(newV0, newV1, newV2, t.ks, t.kd, t.n));
                }
                j++;
            }
            foreach (var item in tris) Triangles.Add(item);
        }
        private Vertex3 ComputeHalfVertex(Vertex3 v1, Vertex3 v2, float radius)
        {
            Vec3 newV = v1.Position + v2.Position;
            float scale = radius / (float)Math.Sqrt(newV.X * newV.X + newV.Y * newV.Y + newV.Z * newV.Z);
            newV *= scale;
            return new Vertex3(newV,(v1.Color+v2.Color)/2);
        }
        private Mesh BuildIcosahedron(float radius, Vec4 color)
        {
            Mesh ico20 = new Mesh();
            //The golden ratio
            float t = (1 + (float)Math.Sqrt(5)) / 2;

            //Define the points needed to build a icosahedron, stolen from article
            Vec3[] vecs = new Vec3[12];
            vecs[0] = new Vec3(-radius, t * radius, 0).Normal();
            vecs[1] = new Vec3(radius, t * radius, 0).Normal();
            vecs[2] = new Vec3(-radius, -t * radius, 0).Normal();
            vecs[3] = new Vec3(radius, -t * radius, 0).Normal();

            vecs[4] = new Vec3(0, -radius, t * radius).Normal();
            vecs[5] = new Vec3(0, radius, t * radius).Normal();
            vecs[6] = new Vec3(0, -radius, -t * radius).Normal();
            vecs[7] = new Vec3(0, radius, -t * radius).Normal();

            vecs[8] = new Vec3(t * radius, 0, -radius).Normal();
            vecs[9] = new Vec3(t * radius, 0, radius).Normal();
            vecs[10] = new Vec3(-t * radius, 0, -radius).Normal();
            vecs[11] = new Vec3(-t * radius, 0, radius).Normal();

            // 5 faces around point 0
            ico20.Add(new Tri(vecs[0], vecs[11], vecs[5], color));
            ico20.Add(new Tri(vecs[0], vecs[5], vecs[1], color));
            ico20.Add(new Tri(vecs[0], vecs[1], vecs[7], color));
            ico20.Add(new Tri(vecs[0], vecs[7], vecs[10], color));
            ico20.Add(new Tri(vecs[0], vecs[10], vecs[11], color));

            // 5 adjacent faces
            ico20.Add(new Tri(vecs[1], vecs[5], vecs[9], color));
            ico20.Add(new Tri(vecs[5], vecs[11], vecs[4], color));
            ico20.Add(new Tri(vecs[11], vecs[10], vecs[2], color));
            ico20.Add(new Tri(vecs[10], vecs[7], vecs[6], color));
            ico20.Add(new Tri(vecs[7], vecs[1], vecs[8], color));

            // 5 faces around point 3
            ico20.Add(new Tri(vecs[3], vecs[9], vecs[4], color));
            ico20.Add(new Tri(vecs[3], vecs[4], vecs[2], color));
            ico20.Add(new Tri(vecs[3], vecs[2], vecs[6], color));
            ico20.Add(new Tri(vecs[3], vecs[6], vecs[8], color));
            ico20.Add(new Tri(vecs[3], vecs[8], vecs[9], color));

            // 5 adjacent faces
            ico20.Add(new Tri(vecs[4], vecs[9], vecs[5], color));
            ico20.Add(new Tri(vecs[2], vecs[4], vecs[11], color));
            ico20.Add(new Tri(vecs[6], vecs[2], vecs[10], color));
            ico20.Add(new Tri(vecs[8], vecs[6], vecs[7], color));
            ico20.Add(new Tri(vecs[9], vecs[8], vecs[1], color));
            return ico20;
        }

    }
}
