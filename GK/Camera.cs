using GK.Math3D;
using System;

namespace GK
{
    public class Camera : Transformable3D
    {
        public float Width { get; set; } = 800;
        public float Height { get; set; } = 600;
        public float fNear { get; set; } = 0.8f;
        public float fFar { get; set; } = 1000f;
        public float FovDeg { get; set; } = 90;

        public Camera()
        {
            Origin = new Vector3Df(0, 0, 1);
        }
        public Transform3D ProjectionMatrix
        {
            get
            {
                float fFovRad = 1 / (float)Math.Tan(FovDeg * 0.5f / 180.0f * Math.PI);
                float[,] matrix = new float[4,4];
                matrix[0,0] = Height / Width * fFovRad;
                matrix[1,1] = fFovRad;
                matrix[2,2] = fFar / (fFar - fNear);
                matrix[3,2] = (-fFar * fNear) / (fFar - fNear);
                matrix[2,3] = 1;
                matrix[3,3] = 0;
                return new Transform3D(matrix);
            }
        }
    }
}
