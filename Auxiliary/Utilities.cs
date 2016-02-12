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
    public static class Utilities
    {
        public static Rectangle ScaleRectangle(Rectangle target, int originalWidth, int originalHeight, bool alsoScaleUp)
        {
            if (!alsoScaleUp && originalWidth < target.Width && originalHeight < target.Height) return new Rectangle(target.X + target.Width/2 - originalWidth/2, target.Y + target.Height/2-originalHeight/2,originalWidth,originalHeight);
            float orWidth = originalWidth;
            float orHeight = originalHeight;
            float maxWidth = target.Width;
            float maxHeight = target.Height;
            float xPresah = orWidth / maxWidth;
            float yPresah = orHeight / maxHeight;
            if (xPresah >= yPresah)
            {
                float xZvetseni = 1f / xPresah;
                orWidth = maxWidth;
                orHeight = orHeight * xZvetseni;
            }
            else
            {
                float yZvetseni = 1f / yPresah;
                orHeight = maxHeight;
                orWidth = orWidth * yZvetseni;
            }
            return new Rectangle(target.X + target.Width / 2 - (int)orWidth / 2, target.Y + target.Height / 2 - (int)orHeight / 2, (int)orWidth, (int)orHeight);
        }
        public static List<Resolution> GetCommonResolutions()
        {
            return new List<Resolution>(
                new Resolution[] {
                        new Resolution(1280, 800),
                        new Resolution(1024, 768),
                        new Resolution(1366, 768),
                        new Resolution(1280, 1024),
                        new Resolution(1440, 900),
                        new Resolution(1680, 1050),
                        new Resolution(1920, 1080),
                        new Resolution(1600, 900),
                        new Resolution(1152, 864)
                    }
                );
        }
    
    }
}
