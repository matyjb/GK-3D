using SFML.Graphics;
using SFML.System;
using GK.Interfaces;
using GK.Structs;
using GK.Transforming;
using System.Collections.Generic;

namespace GK.Drawables
{
    class Triangle : Transformable3D, IDrawable3D
    {
        public Vector3f v0;
        public Vector3f v1;
        public Vector3f v2;
        public Color Color;

        public Triangle(Vector3f v0, Vector3f v1, Vector3f v2, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            Color = color;
        }
        public List<List<Vertex3D>> GetShapes()
        {
            Transform3D t = ParentTransform * Transform;
            return new List<List<Vertex3D>>()
            {
                new List<Vertex3D>()
                {
                    new Vertex3D(t.TransformPoint(v0),Color),
                    new Vertex3D(t.TransformPoint(v1),Color),
                    new Vertex3D(t.TransformPoint(v2),Color)
                }
            };
        }
    }
}
