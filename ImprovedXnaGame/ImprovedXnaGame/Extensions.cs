using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ImprovedXnaGame
{
    public static class Extensions
    {
        public static Color Alpha(this Color color, int alpha)
        {
            return Color.FromNonPremultiplied(color.R, color.G, color.B, alpha);
        }
    }
}
