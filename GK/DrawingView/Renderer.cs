using SFML.Graphics;
using SFML.System;
using GK.Drawables;
using GK.Interfaces;
using GK.Structs;
using System.Collections.Generic;

namespace GK.DrawingView
{
    public sealed class Renderer : Drawable
    {
        public static Renderer Instance { get; } = new Renderer();
        static Renderer() { }
        private Renderer() { }

        public List<IDrawable3D> drawables = new List<IDrawable3D>()
        {
            new Quad(new Vector3f(-1,0,0),new Vector3f(2,0,1),new Vector3f(2,1,1),new Vector3f(-1,4,0), Color.Blue) {Position=new Vector3f(0,0,200), Scale=new Vector3f(100,100,100)},
            new Triangle(new Vector3f(0,0,0),new Vector3f(0,2,0),new Vector3f(1,0,1), Color.Red) {Scale=new Vector3f(100,100,100)},
            new Cuboid(1,1,1,Color.Green) {Scale=new Vector3f(50,50,50)}
        };
        public static Vertex[] PerspectiveView(List<Vertex3D> shapeVertices)
        {
            float camSDistance = Camera.Instance.Sdistance;
            List<Vertex> result = new List<Vertex>();
            for (int i = 0; i < shapeVertices.Count; i++)
            {
                var shapeVertex = shapeVertices[i];
                Vector3f s = new Vector3f(camSDistance / (shapeVertex.Position.Z + camSDistance), camSDistance / (shapeVertex.Position.Z + camSDistance), 0);
                shapeVertex.Position.X *= s.X;
                shapeVertex.Position.Y *= - s.Y; //so Y axis is pointing up in 3d space
                shapeVertex.Position.Z *= s.Z;
                result.Add(new Vertex(new Vector2f(shapeVertex.Position.X, shapeVertex.Position.Y), shapeVertex.Color));
            }
            return result.ToArray();
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            List<List<Vertex3D>> shapes = new List<List<Vertex3D>>();
            //collecting all shapes and
            //copy shapes to not destroy vertices inside drawables
            foreach (var drawable in drawables)
            {
                //List<List<Vertex3D>> copyShapes = new List<List<Vertex3D>>(drawable.GetShapes());
                //for (int i = 0; i < copyShapes.Count; i++)
                //{
                //    copyShapes[i] = new List<Vertex3D>(copyShapes[i]);
                //}
                //shapes.AddRange(copyShapes);
                shapes.AddRange(drawable.GetShapes());
            }


            //inverse z camerą
            for (int i = 0; i < shapes.Count; i++)
            {
                for (int j = 0; j < shapes[i].Count; j++)
                {
                    var v = shapes[i][j];
                    v.Position = Camera.Instance.InverseTransform.TransformPoint(v.Position);
                    shapes[i][j] = v;
                }
            }
            //sortowanie*

            //wywalanie czego nie widac/przyciananie*

            //rzutowanie na 2d i rysowanie na target
            foreach (var shape in shapes)
            {
                target.Draw(PerspectiveView(shape), PrimitiveType.Triangles, states);
            }

        }
    }
}
