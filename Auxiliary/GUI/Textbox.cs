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
    public class Textbox : UIElement
    {
       
        public event Action<Textbox> EnterPress;
        protected virtual void OnEnterPress(Textbox textbox)
        {
            if (EnterPress != null)
                EnterPress(textbox);
        }
        public string Text { get; set; }
        public bool IsPassword { get; set; }
        public bool IsMultiline { get; set; }
        public override void Activate()
        {
            Root.KeyboardInput.ClearBuffer();
            base.Activate();
        }
        public override void Update()
        {
            if (this.IsActive)
            {
                if (Root.KeyboardInput.Buffer != "")
                {
                    string sbuffer = "";
                    for (int i = 0; i < Root.KeyboardInput.Buffer.Length; i++)
                    {
                        char c = Root.KeyboardInput.Buffer[i];
                        if (!Char.IsControl(c))
                        {
                            sbuffer += c;
                        }
                    }

                    Text += sbuffer;
                    Root.KeyboardInput.ClearBuffer();
                }
                if (Root.KeyboardInput.BackSpace)
                {
                    if (Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);
                }
                if (Root.WasKeyPressed(Keys.Enter))
                {
                    if (IsMultiline) Text += Environment.NewLine;
                    else
                        OnEnterPress(this);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            string txt = Text;
            if (IsPassword)
            {
                txt = "";
                for (int i = 0; i < Text.Length; i++)
                    txt += "*";

            }
            Color outerBorderColor = Skin.OuterBorderColor;
            Color innerBorderColor = Skin.InnerBorderColor;
            Color innerButtonColor = Skin.WhiteBackgroundColor;
            Color textColor = Skin.TextColor;
            Primitives.FillRectangle(Rectangle, Color.FromNonPremultiplied(226, 187, 50, 255));//Color.White);
            Primitives.DrawRectangle(Rectangle, Color.Black, 2);
       //     Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawMultiLineText(txt + ((int)(Root.SecondsSinceStart * 2) % 2 == 0 && IsActive ? "|" : ""), new Rectangle(Rectangle.X + 8, Rectangle.Y + 3 + 2, Rectangle.Width - 10, Rectangle.Height - 4), textColor, Library.FontConsoleNormal, Primitives.TextAlignment.TopLeft);
        }
        public Textbox(string text, int x, int y, int width)
        {
            int defaultheight = 40;
            Rectangle = new Rectangle(x, y, width, defaultheight);
            Text = text;
        }
        public Textbox(string text, Rectangle rect, bool multiline = false)
        {
            Rectangle = rect;
            IsMultiline = multiline;
            Text = text;
        }
    }
  
}
