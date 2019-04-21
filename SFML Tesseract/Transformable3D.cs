using SFML.System;

namespace SFML_Tesseract
{
    public class Transformable3D
    {
        private Vector3f position;
        private Vector3f rotation;
        private Vector3f scale = new Vector3f(1, 1, 1);
        private Vector3f origin;
        public Vector3f Position { get=>position; set { position = value;transformUpdateNeeded = true; } }
        public Vector3f Rotation { get=>rotation; set { rotation = value; transformUpdateNeeded = true; } }
        public Vector3f Scale { get=>scale; set { scale = value; transformUpdateNeeded = true; } }
        public Vector3f Origin { get=>origin; set { origin = value; transformUpdateNeeded = true; } }

        private bool transformUpdateNeeded = false;

        public Transform3D ParentTransform { get; set; } = Transform3D.Identity;

        private Transform3D transform = Transform3D.Identity;
        public Transform3D Transform
        {
            get
            {
                if (transformUpdateNeeded)
                {
                    transform = Transform3D.Identity.Translate(-Origin).Rotate(Rotation).Scale(Scale).Translate(Position);
                    transformUpdateNeeded = false;
                }

                return transform;
            }
        }
        public Transform3D InverseTransform
        {
            // TODO: dorobic cache
            get
            {
                float M11 = Transform.Matrix[0, 0];
                float M12 = Transform.Matrix[0, 1];
                float M13 = Transform.Matrix[0, 2];
                float M14 = Transform.Matrix[0, 3];
                float M21 = Transform.Matrix[1, 0];
                float M22 = Transform.Matrix[1, 1];
                float M23 = Transform.Matrix[1, 2];
                float M24 = Transform.Matrix[1, 3];
                float M31 = Transform.Matrix[2, 0];
                float M32 = Transform.Matrix[2, 1];
                float M33 = Transform.Matrix[2, 2];
                float M34 = Transform.Matrix[2, 3];
                float M41 = Transform.Matrix[3, 0];
                float M42 = Transform.Matrix[3, 1];
                float M43 = Transform.Matrix[3, 2];
                float M44 = Transform.Matrix[3, 3];

                float det = (M11 * M22 * M33 * M44) + (M11 * M23 * M34 * M42) + (M11 * M24 * M32 * M43)
                - (M11 * M24 * M33 * M42) - (M11 * M23 * M32 * M44) - (M11 * M22 * M34 * M43)
                - (M12 * M21 * M33 * M44) - (M13 * M21 * M34 * M42) - (M14 * M21 * M32 * M43)
                + (M14 * M21 * M33 * M42) + (M13 * M21 * M32 * M44) + (M12 * M21 * M34 * M43)
                + (M12 * M23 * M31 * M44) + (M13 * M24 * M31 * M42) + (M14 * M22 * M31 * M43)
                - (M14 * M23 * M31 * M42) - (M13 * M22 * M31 * M44) - (M12 * M24 * M31 * M43)
                - (M12 * M23 * M34 * M41) - (M13 * M24 * M32 * M41) - (M14 * M22 * M33 * M41)
                + (M14 * M23 * M32 * M41) + (M13 * M22 * M34 * M41) + (M12 * M24 * M33 * M41);
                float ans11 = (M22 * M33 * M44 + M23 * M34 * M42 + M24 * M32 * M43 - M24 * M33 * M42 - M23 * M32 * M44 - M22 * M34 * M43) / det;
                float ans12 = (-M12 * M33 * M44 - M13 * M34 * M42 - M14 * M32 * M43 + M14 * M33 * M42 + M13 * M32 * M44 + M12 * M34 * M43) / det;
                float ans13 = (M12 * M23 * M44 + M13 * M24 * M42 + M14 * M22 * M43 - M14 * M23 * M42 - M13 * M22 * M44 - M12 * M24 * M43) / det;

                float ans14 = (-M12 * M23 * M34 - M13 * M24 * M32 - M14 * M22 * M33 + M14 * M23 * M32 + M13 * M22 * M34 + M12 * M24 * M33) / det;

                float ans21 = (-M21 * M33 * M44 - M23 * M34 * M41 - M24 * M31 * M43 + M24 * M33 * M41 + M23 * M31 * M44 + M21 * M34 * M43) / det;
                float ans22 = (M11 * M33 * M44 + M13 * M34 * M41 + M14 * M31 * M43 - M14 * M33 * M41 - M13 * M31 * M44 - M11 * M34 * M43) / det;
                float ans23 = (-M11 * M23 * M44 - M13 * M24 * M41 - M14 * M21 * M43 + M14 * M23 * M41 + M13 * M21 * M44 + M11 * M24 * M43) / det;
                float ans24 = (M11 * M23 * M34 + M13 * M24 * M31 + M14 * M21 * M33 - M14 * M23 * M31 - M13 * M21 * M34 - M11 * M24 * M33) / det;
                float ans31 = (M21 * M32 * M44 + M22 * M34 * M41 + M24 * M31 * M42 - M24 * M32 * M41 - M22 * M31 * M44 - M21 * M34 * M42) / det;
                float ans32 = (-M11 * M32 * M44 - M12 * M34 * M41 - M14 * M31 * M42 + M14 * M32 * M41 + M12 * M31 * M44 + M11 * M34 * M42) / det;
                float ans33 = (M11 * M22 * M44 + M12 * M24 * M41 + M14 * M21 * M42 - M14 * M22 * M41 - M12 * M21 * M44 - M11 * M24 * M42) / det;
                float ans34 = (-M11 * M22 * M34 - M12 * M24 * M31 - M14 * M21 * M32 + M14 * M22 * M31 + M12 * M21 * M34 + M11 * M24 * M32) / det;
                float ans41 = (-M21 * M32 * M43 - M22 * M33 * M41 - M23 * M31 * M42 + M23 * M32 * M41 + M22 * M31 * M43 + M21 * M33 * M42) / det;
                float ans42 = (M11 * M32 * M43 + M12 * M33 * M41 + M13 * M31 * M42 - M13 * M32 * M41 - M12 * M31 * M43 - M11 * M33 * M42) / det;
                float ans43 = (-M11 * M22 * M43 - M12 * M23 * M41 - M13 * M21 * M42 + M13 * M22 * M41 + M12 * M21 * M43 + M11 * M23 * M42) / det;
                float ans44 = (M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31 - M12 * M21 * M33 - M11 * M23 * M32) / det;

                return new Transform3D(ans11, ans12, ans13, ans14, ans21, ans22, ans23, ans24, ans31, ans32, ans33, ans34, ans41, ans42, ans43, ans44);
            }
        }
    }
}
