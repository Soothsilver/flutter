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
    public class Button : UIElement
    {
     
        public string Caption;
        public event Action<Button> Click;

        protected virtual void OnClick(Button button)
        {
            if (Click != null)
            {
                Click(button);
            }
        }
        public override void Update()
        {
            if (Root.WasMouseLeftClick && Root.IsMouseOver(Rectangle))
            {
                Root.ConsumeLeftClick();
                OnClick(this);
            }
            base.Update();
        }
        public bool IsMouseOverThis;
        public Button(string text, Rectangle rect)
        {
            Caption = text;
            Rectangle = rect;
        }
        public override void Draw()
        {
            IsMouseOverThis = Root.IsMouseOver(Rectangle);
            bool pressed = IsMouseOverThis && Root.Mouse_NewState.LeftButton == ButtonState.Pressed;
            Color outerBorderColor = IsMouseOverThis ? Skin.OuterBorderColorMouseOver : Skin.OuterBorderColor;
            Color innerBorderColor = pressed ? Skin.InnerBorderColorMousePressed : (IsMouseOverThis ? Skin.InnerBorderColorMouseOver : Skin.InnerBorderColor);
            Color innerButtonColor = IsMouseOverThis ? Skin.GreyBackgroundColorMouseOver: Skin.GreyBackgroundColor;
            Color textColor = IsMouseOverThis ? Skin.TextColorMouseOver : Skin.TextColor;
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawMultiLineText(Caption, InnerRectangle, textColor, Skin.Font, Primitives.TextAlignment.Middle);
        }
    }
 
}
