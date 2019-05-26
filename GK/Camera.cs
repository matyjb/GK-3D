using GK.Math3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public class Camera : Transformable
    {
        public static Camera Instance { get; } = new Camera();
        static Camera()
        {
        }
        private Camera()
        {
        }


        public float Near { get; set; } = 0.8f;
        public float Far { get; set; } = 1000.0f;
        public float Fov { get; set; } = 90.0f;
        public float Width { get; set; } = 800;
        public float Height { get; set; } = 600;

        public Transform ProjectionTransform { get
            {
                // Projection Matrix
                float fAspectRatio = Height / Width;
                float fFovRad = 1.0f / (float)Math.Tan(Fov * 0.5f / 180.0f * Math.PI);

                float[,] m = new float[4, 4];
                m[0, 0] = fAspectRatio * fFovRad;
                m[1, 1] = fFovRad;
                m[2, 2] = Far / (Far - Near);
                m[3, 2] = (-Far * Near) / (Far - Near);
                m[2, 3] = 1.0f;
                m[3, 3] = 0.0f;
                return new Transform(m);
            }
        }
    }
}
