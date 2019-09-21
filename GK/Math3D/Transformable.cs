using System;

namespace GK.Math3D
{
    public abstract class Transformable
    {
        private Vec3 position = new Vec3();
        private Vec3 rotation = new Vec3();
        private Vec3 scale = new Vec3(1, 1, 1);
        private Vec3 origin = new Vec3();
        private Transform parentTransform = Transform.Identity;
        public Vec3 Position { get => position; set { position = value; transformUpdateNeeded = inverseTransformUpdateNeeded = true; } }
        public Vec3 Rotation { get => rotation; set { rotation = value % ((float)Math.PI*2); transformUpdateNeeded = inverseTransformUpdateNeeded = true; } }
        public Vec3 Scale { get => scale; set { scale = value; transformUpdateNeeded = inverseTransformUpdateNeeded = true; } }
        public Vec3 Origin { get => origin; set { origin = value; transformUpdateNeeded = inverseTransformUpdateNeeded = true; } }
        public Transform ParentTransform { get => parentTransform; set { parentTransform = value; transformUpdateNeeded = inverseTransformUpdateNeeded = true; } }



        private Transform transform = Transform.Identity;
        private bool transformUpdateNeeded = false;
        private Transform inverseTransform = Transform.Identity;
        private bool inverseTransformUpdateNeeded = false;
        public Transform Transform
        {
            get
            {
                if (transformUpdateNeeded)
                {
                    transform = ParentTransform * Transform.Identity.Translate(-Origin).Rotate(Rotation).Scale(Scale).Translate(Position);
                    transformUpdateNeeded = false;
                }

                return transform;
            }
        }
        public Transform InverseTransform
        {
            get
            {
                if (inverseTransformUpdateNeeded)
                {
                    float m00 = Transform.Matrix[0, 0];
                    float m01 = Transform.Matrix[0, 1];
                    float m02 = Transform.Matrix[0, 2];
                    float m03 = Transform.Matrix[0, 3];
                    float m10 = Transform.Matrix[1, 0];
                    float m11 = Transform.Matrix[1, 1];
                    float m12 = Transform.Matrix[1, 2];
                    float m13 = Transform.Matrix[1, 3];
                    float m20 = Transform.Matrix[2, 0];
                    float m21 = Transform.Matrix[2, 1];
                    float m22 = Transform.Matrix[2, 2];
                    float m23 = Transform.Matrix[2, 3];
                    float m30 = Transform.Matrix[3, 0];
                    float m31 = Transform.Matrix[3, 1];
                    float m32 = Transform.Matrix[3, 2];
                    float m33 = Transform.Matrix[3, 3];

                    float det = m00 * m11 * m22 * m33 + m00 * m12 * m23 * m31 + m00 * m13 * m21 * m32
                    - m00 * m13 * m22 * m31 - m00 * m12 * m21 * m33 - m00 * m11 * m23 * m32
                    - m01 * m10 * m22 * m33 - m02 * m10 * m23 * m31 - m03 * m10 * m21 * m32
                    + m03 * m10 * m22 * m31 + m02 * m10 * m21 * m33 + m01 * m10 * m23 * m32
                    + m01 * m12 * m20 * m33 + m02 * m13 * m20 * m31 + m03 * m11 * m20 * m32
                    - m03 * m12 * m20 * m31 - m02 * m11 * m20 * m33 - m01 * m13 * m20 * m32
                    - m01 * m12 * m23 * m30 - m02 * m13 * m21 * m30 - m03 * m11 * m22 * m30
                    + m03 * m12 * m21 * m30 + m02 * m11 * m23 * m30 + m01 * m13 * m22 * m30;
                    float ans11 = (m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32 - m13 * m22 * m31 - m12 * m21 * m33 - m11 * m23 * m32) / det;
                    float ans12 = (-m01 * m22 * m33 - m02 * m23 * m31 - m03 * m21 * m32 + m03 * m22 * m31 + m02 * m21 * m33 + m01 * m23 * m32) / det;
                    float ans13 = (m01 * m12 * m33 + m02 * m13 * m31 + m03 * m11 * m32 - m03 * m12 * m31 - m02 * m11 * m33 - m01 * m13 * m32) / det;

                    float ans14 = (-m01 * m12 * m23 - m02 * m13 * m21 - m03 * m11 * m22 + m03 * m12 * m21 + m02 * m11 * m23 + m01 * m13 * m22) / det;

                    float ans21 = (-m10 * m22 * m33 - m12 * m23 * m30 - m13 * m20 * m32 + m13 * m22 * m30 + m12 * m20 * m33 + m10 * m23 * m32) / det;
                    float ans22 = (m00 * m22 * m33 + m02 * m23 * m30 + m03 * m20 * m32 - m03 * m22 * m30 - m02 * m20 * m33 - m00 * m23 * m32) / det;
                    float ans23 = (-m00 * m12 * m33 - m02 * m13 * m30 - m03 * m10 * m32 + m03 * m12 * m30 + m02 * m10 * m33 + m00 * m13 * m32) / det;
                    float ans24 = (m00 * m12 * m23 + m02 * m13 * m20 + m03 * m10 * m22 - m03 * m12 * m20 - m02 * m10 * m23 - m00 * m13 * m22) / det;
                    float ans31 = (m10 * m21 * m33 + m11 * m23 * m30 + m13 * m20 * m31 - m13 * m21 * m30 - m11 * m20 * m33 - m10 * m23 * m31) / det;
                    float ans32 = (-m00 * m21 * m33 - m01 * m23 * m30 - m03 * m20 * m31 + m03 * m21 * m30 + m01 * m20 * m33 + m00 * m23 * m31) / det;
                    float ans33 = (m00 * m11 * m33 + m01 * m13 * m30 + m03 * m10 * m31 - m03 * m11 * m30 - m01 * m10 * m33 - m00 * m13 * m31) / det;
                    float ans34 = (-m00 * m11 * m23 - m01 * m13 * m20 - m03 * m10 * m21 + m03 * m11 * m20 + m01 * m10 * m23 + m00 * m13 * m21) / det;
                    float ans41 = (-m10 * m21 * m32 - m11 * m22 * m30 - m12 * m20 * m31 + m12 * m21 * m30 + m11 * m20 * m32 + m10 * m22 * m31) / det;
                    float ans42 = (m00 * m21 * m32 + m01 * m22 * m30 + m02 * m20 * m31 - m02 * m21 * m30 - m01 * m20 * m32 - m00 * m22 * m31) / det;
                    float ans43 = (-m00 * m11 * m32 - m01 * m12 * m30 - m02 * m10 * m31 + m02 * m11 * m30 + m01 * m10 * m32 + m00 * m12 * m31) / det;
                    float ans44 = (m00 * m11 * m22 + m01 * m12 * m20 + m02 * m10 * m21 - m02 * m11 * m20 - m01 * m10 * m22 - m00 * m12 * m21) / det;

                    inverseTransform = ParentTransform * new Transform(ans11, ans12, ans13, ans14, ans21, ans22, ans23, ans24, ans31, ans32, ans33, ans34, ans41, ans42, ans43, ans44);
                    inverseTransformUpdateNeeded = false;
                }
                return inverseTransform;
            }
        }
    }
}
