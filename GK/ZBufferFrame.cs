using GK.Math3D;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public class ZBufferFrame : Drawable
    {
        public Color[,] Bitmap { get; private set; }
        public float[,] ZBuffer { get; private set; }

        public int Width { get => Bitmap.GetLength(0); }
        public int Height { get => Bitmap.GetLength(1); }
        public Vector2i Size { get => new Vector2i(Width, Height); }

        private Camera Camera;
        public ZBufferFrame(int width, int height, Camera camera)
        {
            Bitmap = new Color[width, height];
            ZBuffer = new float[width, height];
            Camera = camera;
        }

        public void Clear()
        {
            Array.Clear(Bitmap, 0, Bitmap.Length);
            Array.Clear(ZBuffer, 0, ZBuffer.Length);
        }
        public void DrawTriangle(Triangle3Df triangle)
        {
            //find bounding box
            float minX, minY, maxX, maxY;
            minX = minY = float.MaxValue;
            maxX = maxY = float.MinValue;
            foreach (var vert in triangle)
            {
                minX = Math.Min(minX, vert.Position.X);
                minY = Math.Min(minY, vert.Position.Y);
                maxX = Math.Max(maxX, vert.Position.X);
                maxY = Math.Max(maxY, vert.Position.Y);
            }
            //round it to pixels
            minX = (float)Math.Floor(minX);
            minY = (float)Math.Floor(minY);
            maxX = (float)Math.Ceiling(maxX);
            maxY = (float)Math.Ceiling(maxY);
            //bound it to visible area
            Vector2f b = (Vector2f)Size.Div(2f);
            minX = Math.Max(minX, -b.X+1);
            minY = Math.Max(minY, -b.Y+1);
            maxX = Math.Min(maxX, b.X);
            maxY = Math.Min(maxY, b.Y);
            //cast vertices of triangle to Vector2f
            Vector2f v0 = (Vector2f)triangle.v0.Position;
            Vector2f v1 = (Vector2f)triangle.v1.Position;
            Vector2f v2 = (Vector2f)triangle.v2.Position;

            //check if triangle is not a line
            // TODO: precision problem
            if (triangle.v0.Position.X == triangle.v1.Position.X && triangle.v1.Position.X == triangle.v2.Position.X) return;
            if (triangle.v0.Position.Y == triangle.v1.Position.Y && triangle.v1.Position.Y == triangle.v2.Position.Y) return;
            //iterate through all pixels in bounding box
            // TODO: Paraller boost iteration?
            for (int i = (int)minX; i <= maxX; i++)
            {
                for (int j = (int)minY; j <= maxY; j++)
                {
                    //check is it is triangle

                    //get barycentric coordinates
                    Vector2f point = new Vector2f(i, j);
                    Barycentric(point, v0, v1, v2, out var u, out var v, out var w);
                    //if any is negative = not in the triangle
                    // TODO: u v w is nan when points of triangle have massive coordinates
                    try
                    {
                        if (Math.Sign(u) < 0) continue;
                        if (Math.Sign(v) < 0) continue;
                        if (Math.Sign(w) < 0) continue;
                    }
                    catch
                    {
                        continue;
                    }
                    //calc array indexes
                    int x = i + (int)b.X-1;
                    int y = j + (int)b.Y-1;
                    //if the point is in the triangle calculate Z
                    float z = u * triangle.v0.Position.Z + v * triangle.v1.Position.Z + w * triangle.v2.Position.Z;
                    //compare with value in ZBuffer
                    if (ZBuffer[x, y] < z)
                    {
                        //if higher swap value and put color in Bitmap
                        ZBuffer[x, y] = z;
                        // TODO: calculate color
                        Bitmap[x, y] = triangle.v0.Color;
                    }

                }
            }

        }
        private void Barycentric(Vector2f p, Vector2f a, Vector2f b, Vector2f c, out float u, out float v, out float w)
        {
            Vector2f v0 = b - a, v1 = c - a, v2 = p - a;
            float d00 = v0.Dot(v0);
            float d01 = v0.Dot(v1);
            float d11 = v1.Dot(v1);
            float d20 = v2.Dot(v0);
            float d21 = v2.Dot(v1);
            double denom = (double)d00 * d11 - (double)d01 * d01;
            if (denom == 0)
            {
                Vector2f t = p - a;
                if (Math.Sqrt(t.Dot(t)) < 1) { u = 1; v = 0; w = 0; return; }
                t = p - b;
                if (Math.Sqrt(t.Dot(t)) < 1) { u = 0; v = 1; w = 0; return; }
                u = 0; v = 0; w = 1; return;
            }
            v = (float)((d11 * d20 - d01 * d21) / denom);
            w = (float)((d00 * d21 - d01 * d20) / denom);
            u = 1.0f - v - w;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            // TODO: moze jak sie przemiesci image i texture i sprite wyzej to nie bedzie ich trzeba budowac od nowa
            Image image = new Image(Bitmap);
            Texture texture = new Texture(image);
            Sprite sprite = new Sprite(texture) { Position = -(Vector2f)Size.Div(2f) };
            target.Draw(sprite);
            sprite.Dispose();
            texture.Dispose();
            image.Dispose();
        }
    }
}
