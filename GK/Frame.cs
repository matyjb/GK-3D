﻿using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public class Frame : Drawable
    {
        private Color[,] Bitmap { get; set; }
        public int Width { get => Bitmap.GetLength(0); }
        public int Height { get => Bitmap.GetLength(1); }

        public Frame(int width, int height)
        {
            Bitmap = new Color[width, height];
        }
        public void Clear()
        {
            Parallel.For(0, Bitmap.GetLength(0), row => {
                Parallel.For(0, Bitmap.GetLength(1), col => {
                    SetPixel(row, col, Color.Black);
                });
            });
        }
        public void SetPixel(int x, int y, Color color)
        {
            try { Bitmap[x, y] = color; } catch { }
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            Image image = new Image(Bitmap);
            Texture texture = new Texture(image);
            Sprite sprite = new Sprite(texture) { Position = new SFML.System.Vector2f(-Bitmap.GetLength(0)/2, -Bitmap.GetLength(1)/2)};
            target.Draw(sprite);
            sprite.Dispose();
            texture.Dispose();
            image.Dispose();
        }
    }
}