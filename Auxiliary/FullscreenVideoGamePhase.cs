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
    public class FullscreenVideoGamePhase : GamePhase
    {
        public ImprovedVideoPlayer Player;
        private Rectangle rectVideo;
        

        public FullscreenVideoGamePhase(ImprovedVideoPlayer ivp)
        {
            Rectangle screen = Root.Screen;
            Player = ivp;
            rectVideo = new Rectangle(screen.Width / 2 - Player.Video.Width / 2, screen.Height / 2 - Player.Video.Height / 2, Player.Video.Width, Player.Video.Height);
            rectVideo = Utilities.ScaleRectangle(new Rectangle(screen.X + 3, screen.Y + 3, screen.Width - 6, screen.Height -6), rectVideo.Width, rectVideo.Height, false);
        }
        public override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasMouseLeftClick)
            {
                if (!Root.IsMouseOver(rectVideo))
                {
                    Root.ConsumeLeftClick();
                    EndFullscreen();
                }
            }
            if (Root.WasMouseRightClick)
            {
                Root.ConsumeRightClick();
                EndFullscreen();
            }
            Player.Update(alreadyFullscreen: true, mouseisoverthis: Root.IsMouseOver(rectVideo));
            base.Update(game, elapsedSeconds);
        }

        private void EndFullscreen()
        {
            Root.PopFromPhase();
        }
        public override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            Rectangle screen = Root.Screen;
            Primitives.FillRectangle(screen, Color.FromNonPremultiplied(0, 0, 0, 150));
            Player.Draw(sb, rectVideo, alreadyFullscreen: true);
            Primitives.DrawRectangle(new Rectangle(rectVideo.X - 2, rectVideo.Y - 2, rectVideo.Width + 4, rectVideo.Height + 4), Color.White, 2);

            base.Draw(sb, game, elapsedSeconds);
        }
    }
}
