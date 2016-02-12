using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Auxiliary
{
    public class FPSUPSCounter
    {
        public int FPSSoFar;
        public int UPSSoFar;
        public int FPS;
        public int UPS;
        private string FPSUPSString;
        public DateTime SecondElapsesIn = DateTime.Now;
        private SpriteBatch SpriteBatch;

        public FPSUPSCounter(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public void DrawSelf(Vector2 where)
        {
            Auxiliary.Primitives.DrawAndFillRectangle(new Rectangle((int)where.X, (int)where.Y, 300, 30), Color.LightBlue, Color.Black, 2);
            Auxiliary.Primitives.DrawSingleLineText(FPSUPSString, new Vector2((int)where.X + 7, (int)where.Y + 5), Color.Black);
        }
        public void DrawCycle()
        {
            FPSSoFar++;
        }
        public void UpdateCycle()
        {
            UPSSoFar++;
            if (DateTime.Now > SecondElapsesIn)
            {
                UPS = UPSSoFar;
                FPS = FPSSoFar;
                UPSSoFar = 0;
                FPSSoFar = 0;
                FPSUPSString = "FPS: "+ FPS +"; UPS: "+ UPS;
                SecondElapsesIn = DateTime.Now.AddSeconds(1);
            }
        }
    }
}
