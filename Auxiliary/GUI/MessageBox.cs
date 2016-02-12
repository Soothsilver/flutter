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
    public class MessageBoxPhase : GamePhase
    {
        public string Text { get; set; }
        public string Caption { get; set; }
        public GUIIcon Icon { get; set; }
        public MessageBoxButtons ButtonsType { get; set; }
        public List<Button> Buttons = new List<Button>();
        public GUISkin Skin = GUISkin.DefaultSkin;
        public int Width = 800;
        public int Height = 200;
        public int TopLeftX;
        public int TopLeftY;

        public override void Initialize(Game game)
        {
            Root.ReturnedMessageBoxResult = MessageBoxResult.Awaiting;

       
            // Total width and height and X and Y
            Rectangle bounds = Primitives.GetMultiLineTextBounds(Text, new Rectangle(0,0,700, 400), Skin.Font);
            Height = bounds.Height + 106;
            Width = bounds.Width + 65;
            TopLeftX = Root.ScreenWidth / 2 - Width / 2;
            TopLeftY = Root.ScreenHeight / 2 - Height / 2;
    
            // Arranging buttons
            int numbuttons = 0;
            bool doOKButton = false;
            bool doYesButton = false;
            bool doNoButton = false;
            bool doCancelButton = false;
            if (ButtonsType == MessageBoxButtons.OK) { numbuttons = 1; doOKButton = true; }
            if (ButtonsType == MessageBoxButtons.OKCancel) { numbuttons = 2; doOKButton = true; doCancelButton = true; }
            if (ButtonsType == MessageBoxButtons.YesNo) { numbuttons = 2; doYesButton = true; doNoButton = true; }
            if (ButtonsType == MessageBoxButtons.YesNoCancel) { numbuttons = 3; doYesButton = true; doNoButton = true; doCancelButton = true; }
            int buttonwidth = 140;
            int buttonspace = 20;
            if (Width < numbuttons * (buttonwidth + buttonspace) + 100)
                Width = numbuttons * (buttonwidth + buttonspace) + 100;
            int buttonheight = 40;
            int buttony = TopLeftY + Height - 10 - buttonheight;
            int x = TopLeftX + Width / 2 - (int)(((buttonwidth + buttonspace) * (numbuttons)) - buttonspace) / 2;
            if (doOKButton)
            {
                Button b = new Button("OK", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += new Action<Button>(button_Click);
                Buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doYesButton)
            {
                Button b = new Button("Yes", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += new Action<Button>(button_Click);
                Buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doNoButton)
            {
                Button b = new Button("No", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += new Action<Button>(button_Click);
                Buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doCancelButton)
            {
                Button b = new Button("Cancel", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += new Action<Button>(button_Click);
                Buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            base.Initialize(game);
        }

        void button_Click(Button obj)
        {
            MessageBoxResult msgResult = MessageBoxResult.Cancel;
            if (obj.Caption == "OK") msgResult = MessageBoxResult.OK;
            if (obj.Caption == "Cancel") msgResult = MessageBoxResult.Cancel;
            if (obj.Caption == "Yes") msgResult = MessageBoxResult.Yes;
            if (obj.Caption == "No") msgResult = MessageBoxResult.No;

            if (Root.PhaseStack.Count > 1)
            {
                GamePhase gp = Root.PhaseStack[Root.PhaseStack.Count - 2];
                gp.ReturnedMessageBoxResult = msgResult;
                Root.ReturnedMessageBoxResult = msgResult;
            }
            Root.PopFromPhase();
        }
        public override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            Rectangle rectBox =new Rectangle(TopLeftX, TopLeftY, Width, Height);
            Rectangle rectTitle = new Rectangle(TopLeftX, TopLeftY, Width, 28);
            Primitives.DrawAndFillRectangle(rectBox, Skin.DialogBackgroundColor, Skin.OuterBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(rectTitle, Skin.InnerBorderColor, Skin.OuterBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawSingleLineText(Caption, new Vector2(TopLeftX + 5, TopLeftY + 3), Skin.TextColor, Skin.Font);
            if (Icon != GUIIcon.None)
                sb.Draw(Library.GetTexture2DFromGUIIcon(Icon), new Rectangle(rectBox.X + 5, rectBox.Y + 60, 45, 45), Color.White);
            Primitives.DrawMultiLineText(Text, new Rectangle(rectBox.X + 55, rectBox.Y + 50, rectBox.Width - 65, rectBox.Height - 40), Skin.TextColor, Skin.Font, Primitives.TextAlignment.Top);
            foreach (Button b in Buttons)
            {
                b.Draw();
            }
            base.Draw(sb, game, elapsedSeconds);
        }
        public override void Update(Game game, float elapsedSeconds)
        {
            foreach (Button b in Buttons)
                b.Update();
            base.Update(game, elapsedSeconds);
        }
        public MessageBoxPhase(string text, string caption, GUIIcon icon, MessageBoxButtons buttons)
        {
            Text = text;
            Caption = caption;
            Icon = icon;
            ButtonsType = buttons;
        }
    }
    public enum GUIIcon
    {
        None,
        Information,
        Warning,
        Question,
        Error
    }
    public enum MessageBoxButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }
    public enum MessageBoxResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Awaiting
    }
}
