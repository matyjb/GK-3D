using GK.Interfaces;
using GK.Structs;
using GK.Transforming;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace GK.Drawables
{
    class Cuboid : Transformable3D, IDrawable3D
    {
        public float XLenght;
        public float YLenght;
        public float ZLenght;
        public Color Color;

        public Cuboid(float xLenght, float yLenght, float zLenght, Color color)
        {
            XLenght = xLenght;
            YLenght = yLenght;
            ZLenght = zLenght;
            Color = color;
        }

        public List<List<Vertex3D>> GetShapes()
        {
            Transform3D t = ParentTransform * Transform;
            Vector3f v0 = new Vector3f(0, 0, 0);
            Vector3f v1 = new Vector3f(0, 0, ZLenght);
            Vector3f v2 = new Vector3f(0, YLenght, 0);
            Vector3f v3 = new Vector3f(0, YLenght, ZLenght);
            Vector3f v4 = new Vector3f(XLenght, 0, 0);
            Vector3f v5 = new Vector3f(XLenght, 0, ZLenght);
            Vector3f v6 = new Vector3f(XLenght, YLenght, 0);
            Vector3f v7 = new Vector3f(XLenght, YLenght, ZLenght);
            Quad t0 = new Quad(v0, v1, v3, v2, Color) { ParentTransform = t }; //X=0
            Quad t1 = new Quad(v4, v5, v7, v6, Color) { ParentTransform = t }; //X=1
            Quad t2 = new Quad(v0, v1, v5, v4, Color) { ParentTransform = t }; //Y=0
            Quad t3 = new Quad(v2, v3, v7, v6, Color) { ParentTransform = t }; //Y=1
            Quad t4 = new Quad(v0, v4, v6, v2, Color) { ParentTransform = t }; //Z=0
            Quad t5 = new Quad(v1, v5, v7, v3, Color) { ParentTransform = t }; //Z=1
            List<List<Vertex3D>> l = new List<List<Vertex3D>>();
            l.AddRange(t0.GetShapes());
            l.AddRange(t1.GetShapes());
            l.AddRange(t2.GetShapes());
            l.AddRange(t3.GetShapes());
            l.AddRange(t4.GetShapes());
            l.AddRange(t5.GetShapes());
            return l;
        }
    }
}
