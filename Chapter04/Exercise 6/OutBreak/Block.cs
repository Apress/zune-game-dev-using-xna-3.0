using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OutBreak
{
    public class Block
    {
        public Vector2 Position;
        public Color Color;
        public bool IsVisible;

        public Block(Vector2 position, Color color, bool visible)
        {
            Position = position;
            Color = color;
            IsVisible = visible;
        }
    }
}