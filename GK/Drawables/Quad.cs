using GK.Interfaces;
using GK.Structs;
using GK.Transforming;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace GK.Drawables
{
    class Quad : Transformable3D, IDrawable3D
    {
        public Vector3f v0;
        public Vector3f v1;
        public Vector3f v2;
        public Vector3f v3;
        public Color Color;

        public Quad(Vector3f v0, Vector3f v1, Vector3f v2, Vector3f v3, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            Color = color;
        }
        public List<List<Vertex3D>> GetShapes()
        {
            Transform3D t = Transform;
            Triangle t0 = new Triangle(v0, v1, v2, Color) { ParentTransform = t };
            Triangle t1 = new Triangle(v0, v2, v3, Color) { ParentTransform = t };
            List<List<Vertex3D>> l = new List<List<Vertex3D>>();
            l.AddRange(t0.GetShapes());
            l.AddRange(t1.GetShapes());
            return l;
        }
    }
}
