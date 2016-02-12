using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Auxiliary
{
    public static partial class Primitives
    {
        private static SpriteBatch SpriteBatch;
        private static GraphicsDevice GraphicsDevice;

        /* Kreslení primitivních tvarů */
        public static void FillRectangle(Rectangle rectangle, Color color)
        {
            SpriteBatch.Draw(Library.Pixel, rectangle, color);
        }
        public static void DrawRectangle(Rectangle rectangle, Color color, int thickness = 1)
        {
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        }
        public static void DrawAndFillRectangle(Rectangle rectangle, Color innerColor, Color outerColor, int thickness = 1)
        {
            FillRectangle(rectangle, innerColor);
            DrawRectangle(rectangle, outerColor, thickness);
        }
        public static void DrawPoint(Vector2 position, Color color, int size = 1)
        {
            FillRectangle(new Rectangle((int)position.X - size /2, (int)position.Y - size/2, size, size), color);
        }
        public static void DrawLine(Vector2 startPoint, Vector2 endPoint, Color color, int width = 1)
        {
            float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
            float length = Vector2.Distance(startPoint, endPoint);
            SpriteBatch.Draw(Library.Pixel, startPoint, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
        public static void DrawSingleLineText(string text, Vector2 position, Color color, SpriteFont font = null, float scale = 1)
        {
            if (font == null) font = Library.FontConsoleNormal;   
            SpriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        public static void DrawImage(Texture2D texture, Rectangle rectangle, Color? color = null, bool scale = false, bool scaleUp = true, Color? scaleBgColor = null)
        {
            Color clr = color ?? Color.White;
            
            if (scale)
            {
                Color clrB = scaleBgColor ?? Color.Black;
                Primitives.FillRectangle(rectangle, clrB);
                SpriteBatch.Draw(texture, Utilities.ScaleRectangle(rectangle, texture.Width, texture.Height, scaleUp), clr);

            }
            else
            {
                SpriteBatch.Draw(texture, rectangle, clr);
            }
        }

        /// <summary>
        /// Draws a filled circle. Unlike the non-quick method, this is much less CPU-intensive but may produce pixelated look
        /// on extremely small or extremely large circles. It is recommended you use this instead of the non-quick method.
        /// </summary>
        public static void FillCircleQuick(Vector2 center, int radius, Color color)
        {
            float scale = (float)radius / 500f;
            SpriteBatch.Draw(Library.Circle1000x1000, center, null, color, 0, new Vector2(500, 500), scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Draws an outline of a circle. Unlike the non-quick method, this is much less CPU-intensive, however, it does not allow you to 
        /// specify the width of the outline. If you need to specify that, you must use the non-quick method. In that case, however, it is recommended
        /// that you do not change the width or the radius often as whenever you do, the texture of the circle is redrawn which causes a CPU slowdown. 
        /// It may also miss some pixels at radii smaller than 20 pixels.
        /// (This class keeps a cache of circle textures and stores them in a dictionary based on radius and thickness)
        /// </summary>
        public static void DrawCircleQuick(Vector2 center, int radius, Color color)
        {
            float scale = (float)radius / 500f;
            SpriteBatch.Draw(Library.EmptyCircle1000x1000, center, null, color, 0, new Vector2(500, 500), scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a filled circle. 
        /// WARNING! This and the DrawCircle method store circle textures in memory for performance reasons.
        /// If you draw multiple circles of different radii, you may have both performance and memory problems.
        /// </summary>
        public static void FillCircle(Vector2 center, int radius, Color color)
        {
            innerDrawCircle(center, radius, color, true, 1);
        }
      
        /// <summary>
        /// Draws an outline of a circle. 
        /// WARNING! This and the FillCircle method store circle textures in memory for performance reasons.
        /// If you draw multiple circles of different radii, you may have both performance and memory problems.
        /// </summary>
        public static void DrawCircle(Vector2 center, int radius, Color color, int thickness = 1)
        {
            innerDrawCircle(center, radius, color, false, thickness);
        }
        /// <summary>
        /// Clears all cached Circle textures. This will clear space from memory, but drawing circles will take longer.
        /// </summary>
        public static void ClearCircleCache()
        {
            CirclesCache.Clear();
            GC.Collect();
        }
        private static void innerDrawCircle(Vector2 center, int radius, Color color, bool filled, int thickness)
        {
            bool containsKeyRadius = CirclesCache.ContainsKey(radius);
            if (containsKeyRadius)
            {
                foreach(Circle c in CirclesCache[radius])
                {
                    if (c.Filled == filled && (filled || c.Thickness == thickness))
                    {
                        SpriteBatch.Draw(c.Texture, center, null, color, 0, new Vector2(radius + 1, radius + 1), 1, SpriteEffects.None, 0);
                        return;
                    }
                }
            }
            int outerRadius = radius * 2 + 2;
            Texture2D texture = new Texture2D(Primitives.GraphicsDevice, outerRadius, outerRadius);
            if (filled) thickness = radius + 1;

            Color[] data = new Color[outerRadius * outerRadius];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            double angleStep = 0.5f / radius;

            int lowpoint = radius - thickness;
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                for (int i = radius; i > lowpoint; i--)
                {
                    int x = (int)Math.Round(radius + i * Math.Cos(angle));
                    int y = (int)Math.Round(radius + i * Math.Sin(angle));
                    data[y * outerRadius + x + 1] = color;
                }
            }
            texture.SetData(data);
            if (containsKeyRadius)
                CirclesCache[radius].Add(new Circle(texture, radius, filled, thickness));
            else
                CirclesCache.Add(radius, new List<Circle>(new Circle[] { new Circle(texture, radius, filled, thickness) }));

            SpriteBatch.Draw(texture, center, null, color, 0, new Vector2(radius + 1, radius + 1), 1, SpriteEffects.None, 0);
        }
        private static Dictionary<int, List<Circle>> CirclesCache = new Dictionary<int, List<Circle>>();
        /// <summary>
        /// This method is called automatically from Root.Init()
        /// </summary>
        public static void Init(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            SpriteBatch = spriteBatch;
            GraphicsDevice = graphicsDevice;
        }
        private class Circle
        {
            public Texture2D Texture;
            public int Radius;
            public bool Filled;
            public int Thickness;
            public Circle(Texture2D texture, int radius, bool filled, int thickness)
            {
                Texture = texture; Radius = radius; Filled = filled; Thickness = thickness;
            }
        }
    }
 
}
