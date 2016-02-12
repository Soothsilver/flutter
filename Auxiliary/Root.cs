/***
 * INSTRUCTIONS:
 * Always run Root.Init(); 
 * when using the Auxiliary project
 * To use all functions, add Root.Draw() and Root.Update() to the end of your Draw() and Update() functions.
 * Then put Root.DrawPhase() wherever you want the bulk of your graphical data to be drawn.
 * 
 */
// TODO LATER loading content with progress bar
// TODO LATER many fonts
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
    public static partial class Root
    {
        // Properties starting with "in" should not be used by your project
        //private static ContentManager inContent;
        
        private static FPSUPSCounter inFPSCounter;
        private static Game inGame;
        private static SpriteBatch inSpriteBatch;
        private static GraphicsDeviceManager inGraphics;
        public static KeyboardInput KeyboardInput;

        /// <summary>
        /// Some basic fonts and textures
        /// </summary>
        public static Library Library;
        public static ImprovedStack<GamePhase> PhaseStack = new ImprovedStack<GamePhase>();

        public static GamePhase CurrentPhase
        {
            get
            {
                if (PhaseStack.Count > 0) return PhaseStack.Peek(); else return null;
            }
            set
            {
                PhaseStack.Push(value);
            }
        }

        public static KeyboardState Keyboard_OldState = Keyboard.GetState();
        public static KeyboardState Keyboard_NewState = Keyboard.GetState();
        public static MouseState Mouse_OldState = Mouse.GetState();
        public static MouseState Mouse_NewState = Mouse.GetState();


        public static bool EXECUTE_DoNotLoadXXLTextures = false;
        public static bool DISPLAY_DisplayFPSCounter = false;
        public static Vector2 DISPLAY_DisplayFPSCounterWhere = new Vector2(5, 5);

        public static int ScreenWidth
        {
            get { return Root.inGraphics.PreferredBackBufferWidth; }
        }
        public static int ScreenHeight
        {
            get { return Root.inGraphics.PreferredBackBufferHeight; }
        }
        public static Rectangle Screen
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }

        public static void PushPhase(GamePhase phase)
        {
            CurrentPhase = phase;
            phase.Initialize(inGame);
        }
        public static void PopFromPhase()
        {
            GamePhase gp = PhaseStack.Peek();
            if (gp != null) gp.Destruct(inGame);
        }

        public static bool IsMouseOver(Rectangle rect)
        {
            return Mouse_NewState.X >= rect.X && Mouse_NewState.Y >= rect.Y && Mouse_NewState.X < rect.Right && Mouse_NewState.Y < rect.Bottom;
        }

        public static void SetResolution(int width, int height)
        {
            inGraphics.PreferredBackBufferWidth = width;
            inGraphics.PreferredBackBufferHeight = height;
            inGraphics.ApplyChanges();
        }
        public static void SetResolution(Resolution r)
        {
            inGraphics.PreferredBackBufferWidth = r.Width;
            inGraphics.PreferredBackBufferHeight = r.Height;
            inGraphics.ApplyChanges();
        }
        public static bool IsFullscreen
        {
            get { return inGraphics.IsFullScreen; }
            set { inGraphics.IsFullScreen = value; inGraphics.ApplyChanges(); }
        }
        public static void DrawPhase(GameTime gameTime)
        {
            foreach (GamePhase gp in PhaseStack)
            {
                gp.Draw(Root.inSpriteBatch, Root.inGame, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }
        public static void Draw(GameTime gameTime)
        {
            DrawToasts((float)gameTime.ElapsedGameTime.TotalSeconds);
            // FPS
            inFPSCounter.DrawCycle();
            if (DISPLAY_DisplayFPSCounter)
                inFPSCounter.DrawSelf(DISPLAY_DisplayFPSCounterWhere);
        }
        public static void Update(GameTime gameTime)
        {
            Keyboard_OldState = Keyboard_NewState;
            Mouse_OldState = Mouse_NewState;
            Keyboard_NewState = Keyboard.GetState();
            Mouse_NewState = Mouse.GetState();

            WasMouseLeftClick = Mouse_NewState.LeftButton == ButtonState.Released && Mouse_OldState.LeftButton == ButtonState.Pressed;
            WasMouseMiddleClick = Mouse_NewState.MiddleButton == ButtonState.Released && Mouse_OldState.MiddleButton == ButtonState.Pressed;
            WasMouseRightClick = Mouse_NewState.RightButton == ButtonState.Released && Mouse_OldState.RightButton == ButtonState.Pressed;


            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            inFPSCounter.UpdateCycle();
            if (Root.PhaseStack.Count > 0)
                Root.PhaseStack.Peek().Update(Root.inGame, (float)gameTime.ElapsedGameTime.TotalSeconds);

            SecondsSinceStart += elapsedSeconds;
            SecondsSinceStartInt = (int)SecondsSinceStart;
            /*
            foreach (GamePhase phase in Root.PhaseStack)
            {
                if (
                phase.Update(Root.inGame, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }*/
            for (int i = Root.PhaseStack.Count - 1; i >= 0; i--)
            {
                GamePhase ph = Root.PhaseStack[i];
                if (ph.ScheduledForElimination)
                    Root.PhaseStack.RemoveAt(i);
            }

            Root.UpdateToasts(elapsedSeconds);
        }
        /// <summary>
        /// Returns true only if a key was just pressed down and released.
        /// </summary>
        /// <param name="key">We test whether this key was pressed and released</param>
        /// <param name="modifiersPressed">This combination of keys must have been pressed at the time of release</param>
        /// <returns></returns>
        public static bool WasKeyPressed(Keys key, params ModifierKey[] modifiersPressed)
        {
            if (Keyboard_NewState.IsKeyDown(key) || Keyboard_OldState.IsKeyUp(key)) return false;
           
                foreach(ModifierKey mk in modifiersPressed)
                {
                    Keys mkKey = Keys.A;
                    Keys mkKey2 = Keys.B;
                    if (mk == ModifierKey.Alt) { mkKey = Keys.LeftAlt; mkKey2 = Keys.RightAlt; }
                    if (mk == ModifierKey.Ctrl) { mkKey = Keys.LeftControl; mkKey2 = Keys.RightControl; }
                    if (mk == ModifierKey.Shift) { mkKey = Keys.LeftShift; mkKey2 = Keys.RightShift; }
                    if (mk == ModifierKey.Windows) { mkKey = Keys.LeftWindows; mkKey2 = Keys.RightWindows; }
                    if (Keyboard_OldState.IsKeyUp(mkKey) && Keyboard_NewState.IsKeyUp(mkKey) &&
                    Keyboard_OldState.IsKeyUp(mkKey2) && Keyboard_NewState.IsKeyUp(mkKey2)
                    ) return false;
                }
            
            return true;
        }
        public static bool WasMouseLeftClick { get; set; }
        public static bool WasMouseMiddleClick { get; set; }
        public static bool WasMouseRightClick { get; set; }
        public static void ConsumeLeftClick() { WasMouseLeftClick = false; }
        public static void ConsumeMiddleClick() { WasMouseMiddleClick = false; }
        public static void ConsumeRightClick() { WasMouseRightClick = false; }


        public static void Init(Game game, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Resolution defaultResolution = null, bool fullscreenMode = false)
        {
            inGame = game;
            inSpriteBatch = spriteBatch;
            inGraphics = graphics;

            // Load basic textures
            Library = new Auxiliary.Library(game);
            Library.LoadBaseTextures(EXECUTE_DoNotLoadXXLTextures);
            // Load graphical functions
            Auxiliary.Primitives.Init(spriteBatch, game.GraphicsDevice);
            // Load FPS Counter
            Root.inFPSCounter = new FPSUPSCounter(spriteBatch);
            // Load keyboard input
            Root.KeyboardInput = new KeyboardInput(inGame.Window.Handle);
            // Set resolution
            if (defaultResolution != null)
                SetResolution(defaultResolution);
            Root.IsFullscreen = fullscreenMode;
            // Load application-wide settings
            bool settingsLoaded = Cother.ApplicationSettingsManagement.LoadSettings(inGame);
        }
        public static bool SaveSettings()
        {
            return Cother.ApplicationSettingsManagement.SaveSettings(inGame);
        }
    }
    public enum ModifierKey
    {
        Ctrl,
        Shift,
        Alt,
        Windows
    }
    [Serializable]
    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
        /*
        public static bool operator ==(Resolution r1, Resolution r2)
        {
            if (r1 == null ^ r2 == null) return false;
            return r1.Width == r2.Width && r1.Height == r2.Height;
        }
        public static bool operator !=(Resolution r1, Resolution r2)
        {
            if (r1 == null && r2 == null) return false;
            if (r1 == null ^ r2 == null) return true;
            return r1.Width != r2.Width || r1.Height != r2.Height;
        }*/
        public override bool Equals(object obj)
        {
            if (obj is Resolution)
            {
                Resolution r = (Resolution)obj;
                return this.Width == r.Width && this.Height == r.Height;
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return Width * 10000 + Height;
        }
    }
}
