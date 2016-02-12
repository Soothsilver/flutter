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
using System.IO;

namespace Auxiliary
{
    public static class ExtensionMethods
    {
        public static Dictionary<string, T> LoadContent<T>(this ContentManager contentManager, string contentFolder)
        {
            var dir = new DirectoryInfo(contentManager.RootDirectory
                + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            var result = new Dictionary<string, T>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }

            return result;
        }

    }
    public class Library : DrawableGameComponent
    {
        private static ContentManager LibContent;

        public static Texture2D GetTexture2DFromGUIIcon(GUIIcon icon)
        {
            switch (icon)
            {
                case GUIIcon.Error: return Library.IconError;
                case GUIIcon.Information: return Library.IconInformation;
                case GUIIcon.Question: return Library.IconQuestion;
                case GUIIcon.Warning: return Library.IconWarning;
            }
            return null;
        }
        public static Texture2D Pixel;
        public static Texture2D Circle1000x1000;
        public static Texture2D EmptyCircle1000x1000;
        public static Texture2D IconInformation;
        public static Texture2D IconWarning;
        public static Texture2D IconError;
        public static Texture2D IconQuestion;
        public static Texture2D IconPlay;
        public static Texture2D IconPause;
        public static Texture2D IconStop;
        public static Texture2D IconFullscreen;

        public static SpriteFont FontCourierNewNormal;
        public static SpriteFont FontConsoleNormal;
        public static SpriteFont FontConsoleNormalBold;

        public Library(Game game)
            : base(game)
        {
            LibContent = new ContentManager(game.Services);
            LibContent.RootDirectory = "AuxiliaryContent";
        }

        public static void LoadBaseTextures(bool doNotLoadXXLTextures = false)
        {
            Pixel = LibContent.Load<Texture2D>("pixel");
            FontCourierNewNormal = LibContent.Load<SpriteFont>("fontCourierNewNormal");
            FontConsoleNormal = LibContent.Load<SpriteFont>("fontTahomaNormal");
            FontConsoleNormalBold = LibContent.Load<SpriteFont>("fontTahomaNormalBold");
        }
    }
}
