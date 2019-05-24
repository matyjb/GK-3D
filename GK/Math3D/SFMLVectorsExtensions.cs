using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.Math3D
{
    public static class SFMLVectorsExtensions
    {
        public static Vector2i Div(this Vector2i left, float right)
        {
            return new Vector2i((int)(left.X / right), (int)(left.Y / right));
        }
        public static float Dot(this Vector2f left, Vector2f right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        public static float Cross(this Vector2f left, Vector2f right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
    }
}
