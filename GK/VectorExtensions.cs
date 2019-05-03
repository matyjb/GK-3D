using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public static class VectorExtensions
    {
        public static Vector3f Cross(this Vector3f left, Vector3f right)
        {
            float x = left.Y * right.Z - left.Z * right.Y;
            float y = left.Z * right.X - left.X * right.Z;
            float z = left.X * right.Y - left.Y * right.Z;
            return new Vector3f(x, y, z);
        }
        public static float Dot(this Vector3f left, Vector3f right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        public static float Sum(this Vector3f left)
        {
            return left.X + left.Y + left.Z;
        }

        public static float Cross(this Vector2f left, Vector2f right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
        public static float Dot(this Vector2f left, Vector2f right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        public static float Sum(this Vector2f left)
        {
            return left.X + left.Y;
        }
        public static float Distance(this Vector2f left, Vector2f right)
        {
            return (float)Math.Sqrt((left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y));
        }
    }
}
