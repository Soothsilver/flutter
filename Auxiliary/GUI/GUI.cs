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
using System.Reflection;
namespace Auxiliary
{
    public static partial class Root
    {
        public static UIElement GUIActiveElement = null;
        public static float SecondsSinceStart = 0;
        public static int SecondsSinceStartInt = 0;
        public static MessageBoxResult ReturnedMessageBoxResult = MessageBoxResult.Awaiting;
        public static void MessageBox(string text, string caption = "Message", GUIIcon icon = GUIIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            MessageBoxPhase phaseMsgBox = new MessageBoxPhase(text, caption, icon, buttons);
            Root.PushPhase(phaseMsgBox);
        }
    }
    public class UIElement
    {

        public GUISkin Skin = GUISkin.DefaultSkin;
        public Rectangle Rectangle;
        public Rectangle InnerRectangle
        {
            get
            {
                return new Rectangle(Rectangle.X + Skin.TotalBorderThickness, Rectangle.Y + Skin.TotalBorderThickness, Rectangle.Width - 2 * Skin.TotalBorderThickness, Rectangle.Height - 2 * Skin.TotalBorderThickness);
            }
        }
        public Rectangle InnerRectangleWithBorder
        {
            get
            {
                return new Rectangle(Rectangle.X + Skin.OuterBorderThickness + Skin.InnerBorderThickness, Rectangle.Y + Skin.OuterBorderThickness + Skin.InnerBorderThickness, Rectangle.Width - 2 * (Skin.OuterBorderThickness + Skin.InnerBorderThickness), Rectangle.Height - 2 * (Skin.OuterBorderThickness + Skin.InnerBorderThickness));

            }
        }
        public virtual void Draw()
        {
            Primitives.FillRectangle(Rectangle, Color.Red);
        }
        public virtual void Update()
        {
            if (Root.WasMouseLeftClick && Root.IsMouseOver(Rectangle))
            {
                Activate();
                Root.ConsumeLeftClick();
            }
        }
        public virtual void Activate()
        {
            Root.GUIActiveElement = this;
        }
        public virtual void Deactivate()
        {
            Root.GUIActiveElement = null;
        }
        public bool IsActive
        {
            get
            {
                return Root.GUIActiveElement == this;
            }
        }
    }
    public class GUISkin
    {
        public Color InnerBorderColorMouseOver;
        public Color InnerBorderColorMousePressed;
        public Color InnerBorderColor;
        public int InnerBorderThickness;
        public Color OuterBorderColor;
        public Color OuterBorderColorMouseOver;
        public int OuterBorderThickness;
        public int TotalBorderThickness
        {
            get
            {
                return 2 * OuterBorderThickness + InnerBorderThickness;
            }
        }
        public Color GreyBackgroundColor;
        public Color GreyBackgroundColorMouseOver;
        public Color DialogBackgroundColor;
        public Color WhiteBackgroundColor;
        public Color ItemSelectedBackgroundColor;
        public Color ItemMouseOverBackgroundColor;
        public SpriteFont Font;
        public Color TextColor;
        public Color TextColorMouseOver;
        public int ListItemHeight;

        public GUISkin Clone()
        {
            GUISkin newSkin = new GUISkin();
            Type skinType = this.GetType();
            FieldInfo[] fields = skinType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                fi.SetValue(newSkin, fi.GetValue(this));
            }
            return newSkin;
        }

        public static GUISkin DefaultSkin;
        public static GUISkin SimplisticSkin;
        static GUISkin()
        {
            DefaultSkin = new GUISkin();
            DefaultSkin.InnerBorderColorMouseOver = Color.Aquamarine;
            DefaultSkin.InnerBorderColor = Color.LightBlue;
            DefaultSkin.InnerBorderColorMousePressed = Color.DarkBlue;
            DefaultSkin.InnerBorderThickness = 3;
            DefaultSkin.OuterBorderColor = Color.Black;
            DefaultSkin.OuterBorderColorMouseOver = Color.Black;
            DefaultSkin.OuterBorderThickness = 1;
            DefaultSkin.Font = Library.FontConsoleNormal;
            DefaultSkin.TextColor = Color.Black;
            DefaultSkin.TextColorMouseOver = Color.Black;
            DefaultSkin.GreyBackgroundColor = Color.FromNonPremultiplied(Color.CornflowerBlue.R + 20, Color.CornflowerBlue.G + 20, Color.CornflowerBlue.B + 20, 255);
            DefaultSkin.GreyBackgroundColorMouseOver = Color.FromNonPremultiplied(Color.CornflowerBlue.R + 26, Color.CornflowerBlue.G + 26, Color.CornflowerBlue.B + 26, 255);
            DefaultSkin.WhiteBackgroundColor = Color.White;
            DefaultSkin.DialogBackgroundColor = Color.FromNonPremultiplied(Color.CornflowerBlue.R + 45, Color.CornflowerBlue.G + 45, Color.CornflowerBlue.B + 45, 255);
            DefaultSkin.ListItemHeight = 30;
            DefaultSkin.ItemSelectedBackgroundColor = Color.PowderBlue;
            DefaultSkin.ItemMouseOverBackgroundColor = Color.LightYellow;

            SimplisticSkin = DefaultSkin.Clone();
            SimplisticSkin.InnerBorderColor = Color.MediumAquamarine;
            SimplisticSkin.InnerBorderThickness = 1;
        }
    }
}
