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
    public class ImprovedVideoPlayer
    {
        public VideoPlayer VideoPlayer = new VideoPlayer();
        public Texture2D StoppedTexture;
        public bool HasPlayPauseButton;
        public bool HasExtendToFullscreenButton;
        public bool OnClickDisplayAsOverlay = true;
        public bool OnClickPlayPause;
        public bool OnClickExtendToFullscreen;
        public bool DisplayOverlayAndAllowClicking = true;
        public bool IsInFullscreenMode = false;
        public bool PerformScaling = true;
        private bool MouseOverPlayPauseButton;
        private bool MouseOverStopButton;
        private bool MouseOverFullscreenButton;
        private bool MouseOverThisElement;
      
        public void Update(bool alreadyFullscreen = false, bool mouseisoverthis = false)
        {
            if (Root.WasMouseLeftClick && (MouseOverThisElement || mouseisoverthis))
            {
                Root.ConsumeLeftClick();
                if (MouseOverPlayPauseButton)
                {
                    if (VideoPlayer.State == MediaState.Playing) Pause();
                    else Play();
                }
                else if (MouseOverStopButton)
                {
                    Stop();
                }
                else if (MouseOverFullscreenButton)
                {
                    GoFullscreen();
                }
                else if (OnClickExtendToFullscreen)
                {
                    if (!alreadyFullscreen)
                        GoFullscreen();
                    else
                        Root.PopFromPhase();
                }
                else if (OnClickPlayPause)
                {
                    if (VideoPlayer.State == MediaState.Playing) Pause();
                    else Play();
                }
            }
        }

        private void GoFullscreen()
        {
            FullscreenVideoGamePhase fvgp = new FullscreenVideoGamePhase(this);
            Root.PushPhase(fvgp);
        }
        public void Draw(SpriteBatch sb, Rectangle rect, bool alreadyFullscreen = false)
        {
            MouseOverFullscreenButton = false;
            MouseOverPlayPauseButton = false;
            MouseOverStopButton = false;
            MouseOverThisElement = false;
            if (State == MediaState.Playing || State == MediaState.Paused)
            {
                Texture2D tex = this.GetTexture();
                if (tex != null)
                {
                    Primitives.DrawImage(tex, rect, Color.White, scale: PerformScaling, scaleUp: false, scaleBgColor: Color.Black);
                }
                else
                {
                    Primitives.FillRectangle(rect, Color.DarkGreen);
                }
            }
            else
            {
                if (StoppedTexture != null)
                    Primitives.DrawImage(StoppedTexture, rect, Color.White, scale: PerformScaling, scaleUp: false, scaleBgColor: Color.Black);
                else
                    Primitives.FillRectangle(rect, Color.Black);
            }
            if (Root.IsMouseOver(rect))
            {
                MouseOverThisElement = true;
                if (DisplayOverlayAndAllowClicking)
                {
                    Color clrSemitransparent = Color.FromNonPremultiplied(255, 255, 255, 150);
                    Rectangle rectBottom = new Rectangle(rect.X, rect.Bottom - 5, rect.Width, 5);
                    Primitives.FillRectangle(rectBottom, Color.Gray);
                    double percent = VideoPlayer.PlayPosition.TotalSeconds / VideoPlayer.Video.Duration.TotalSeconds;
                    Primitives.FillRectangle(new Rectangle(rectBottom.X, rectBottom.Y, (int)(rectBottom.Width * percent), rectBottom.Height), Color.White);
                    int ICONSIZE = 30;
                    Rectangle rectPlayPause = new Rectangle(rectBottom.X + 1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                    Rectangle rectStop = new Rectangle(rectBottom.X + ICONSIZE+1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                    Rectangle rectFullscreen = new Rectangle(rectBottom.Right - ICONSIZE-1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                    if (HasPlayPauseButton)
                    {
                        Texture2D icon = VideoPlayer.State == MediaState.Playing ? Library.IconPause : Library.IconPlay;
                        MouseOverPlayPauseButton = Root.IsMouseOver(rectPlayPause);
                        sb.Draw(icon, rectPlayPause, MouseOverPlayPauseButton ? Color.White : clrSemitransparent);
                        if (State == MediaState.Playing || State == MediaState.Paused)
                        {
                            MouseOverStopButton = Root.IsMouseOver(rectStop);
                            sb.Draw(Library.IconStop, rectStop, MouseOverStopButton ? Color.White : clrSemitransparent);
                        }
                    }
                    if (HasExtendToFullscreenButton && !alreadyFullscreen)
                    {
                        MouseOverFullscreenButton = Root.IsMouseOver(rectFullscreen);
                        sb.Draw(Library.IconFullscreen, rectFullscreen, MouseOverFullscreenButton ? Color.White : clrSemitransparent);
                    }
                    
                    if (OnClickDisplayAsOverlay)
                    {
                        if (OnClickPlayPause)
                            sb.Draw(VideoPlayer.State == MediaState.Playing ? Library.IconPause : Library.IconPlay, new Rectangle(rect.X + rect.Width / 2 - 30, rect.Y + rect.Height / 2 - 30, 60, 60), clrSemitransparent);
                        if (OnClickExtendToFullscreen && !alreadyFullscreen)
                            sb.Draw(Library.IconFullscreen, new Rectangle(rect.X + rect.Width / 2 - 30, rect.Y + rect.Height / 2 - 30, 60, 60), clrSemitransparent);
                    }
                }
            }
        }
        public ImprovedVideoPlayer()
        {

        }
        public ImprovedVideoPlayer(Video video, Texture2D defaultTexture, bool hasButtons = true, bool onclickFullscreen = false, bool onclickPlayPause = false)
        {
            Video = video;
            StoppedTexture = defaultTexture;
            if (hasButtons)
            {
                HasExtendToFullscreenButton = true;
                HasPlayPauseButton = true;
            }
            if (onclickPlayPause)
                OnClickPlayPause = true;
            if (onclickFullscreen)
                OnClickExtendToFullscreen = true;
        }
        public Texture2D GetTexture()
        {
            return VideoPlayer.GetTexture();
        }
        public bool IsLooped
        {
            get { return VideoPlayer.IsLooped; }
            set { VideoPlayer.IsLooped = value; }
        }
        public void Pause()
        {
            VideoPlayer.Pause();
        }
        public bool IsMuted
        {
            get { return VideoPlayer.IsMuted; }
            set { VideoPlayer.IsMuted = value; }
        }
        public void Play()
        {
            if (Video != null)
                VideoPlayer.Play(Video);
        }
        public void Play(Video video)
        {
            Video = video;
            VideoPlayer.Play(video);
        }
        public void PlayAndStopAtFirstFrame(Video video)
        {
            VideoPlayer.Play(video);
            VideoPlayer.Pause();
        }
        public TimeSpan PlayPosition
        {
            get { return VideoPlayer.PlayPosition; }
        }
        public void Resume()
        {
            VideoPlayer.Resume();
        }
        public MediaState State
        {
            get { return VideoPlayer.State; }
        }
        public void Stop()
        {
            VideoPlayer.Stop();
        }
        public float Volume
        {
            get { return VideoPlayer.Volume; }
            set { VideoPlayer.Volume = value; }
        }
        public Video Video { get; set; }
    }
}
