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
using Auxiliary;
using Cother;
using System.ComponentModel;
using System.Net;
namespace ImprovedXnaGame
{
    public partial class ImprovedGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Session Session;
        public List<string> CommandsToSendOverInternet = new List<string>();
        public List<string> CommandsBeingSentOverInternet = new List<string>();
        public BackgroundWorker OnlineWorker = new BackgroundWorker();

        public Texture2D ImageFluttershy;

        public static ImprovedGame Main;


        public bool TrackingPermitted = true;
        public ImprovedGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            OnlineWorker.DoWork += new DoWorkEventHandler(OnlineWorker_DoWork);
            Main = this;
        }

        void OnlineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<string> commandos = e.Argument as List<string>;
                foreach (string command in commandos)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadString("http://fluttershy.wz.cz/fluttershy.php?command=" + command);
                }
            }
            catch
            {

            }
        }

        protected override void LoadContent()
        {

            string[] args = Environment.GetCommandLineArgs();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ImageFluttershy = Content.Load<Texture2D>("FluttershyHappy");
            Root.EXECUTE_DoNotLoadXXLTextures = true;
            Resolution resolution = new Resolution(1024, 768);

            if (args.Length >= 2)
            {
                try
                {
                    string[] wh = args[1].Split('x');
                    int w = Int32.Parse(wh[0]);
                    int h = Int32.Parse(wh[1]);
                    resolution = new Resolution(w, h);
                }
                catch
                {

                }
            }
            if (args.Length >= 4)
            {
                if (args[3] == "donottrack") TrackingPermitted = false;
            }
            Root.Init(this, spriteBatch, graphics, resolution);
            /*     public static Rectangle rectConsoleTop = new Rectangle(3, 3, 1024 - 6, 500);
        public static Rectangle rectConsoleTopInner = new Rectangle(3+2, 3+2, 1024 - 6-4, 500-4);
        Rectangle rectInputLine = new Rectangle(3, 506, 1024 - 6, 30);
        Rectangle rectSuggestions = new Rectangle(3, 539, 1024 - 6 - 203, 226);
        Rectangle rectRecommendedCommands = new Rectangle(1024-203, 539, 200, 226);*/
            rectConsoleTop = new Rectangle(2, 2, resolution.Width - 4, 500);
            rectConsoleTopInner = new Rectangle(7, 7, resolution.Width - 10 - 4, 496 - 4);
            rectInputLine = new Rectangle(2, 503, resolution.Width - 4, 30);
            int suggestionBoxHeight = resolution.Height - 2 - 535;
            rectSuggestions = new Rectangle(2, 535, resolution.Width *3/4, suggestionBoxHeight);
            rectRecommendedCommands = new Rectangle(resolution.Width *3/4 + 4, 535, resolution.Width *1/4 - 6, suggestionBoxHeight);

            if (args.Length >= 3)
            {
                if (args[2] == "fullscreen")
                {
                    Root.IsFullscreen = true;
                }
            }

            InputTextBox = new Textbox("", rectInputLine, false);
            InputTextBox.Activate();
            InputTextBox.EnterPress += new Action<Textbox>(InputTextBox_EnterPress);
            Session = new Session();
            Session.EnterLocation(Session.Locations[LocationID.Splash]);
            Session.GetAvailableCommands(out AvailableCommands, out RecommendedCommands);
            // Add your first game phase here
        }
        public int PreviousConsoleTotalLines = 0;
        public int GreyOutputUntil = -1;
        public int ConsoleMaxLines = 22;
        public int ConsoleOutputCurrentLine = 0;
        public void LinesAdded(bool nograying)
        {
            if (!nograying)
            GreyOutputUntil = PreviousConsoleTotalLines;
            if (Session.ConsoleTotalLines > ConsoleMaxLines + ConsoleOutputCurrentLine)
            {
                ConsoleOutputCurrentLine = Session.ConsoleTotalLines - ConsoleMaxLines;
            }
            PreviousConsoleTotalLines = Session.ConsoleTotalLines;
        }

        void InputTextBox_EnterPress(Textbox obj)
        {
            // Command entered.
            if (CurrentSuggestions.Count > 0 && ChosenSuggestion >= 0 && ChosenSuggestion < CurrentSuggestions.Count)
            {
                // Possible command.
                SendCommand(CurrentSuggestions[ChosenSuggestion]);
                CommandsToSendOverInternet.Add(CurrentSuggestions[ChosenSuggestion].TotalString);
            }
            else
            {
                Session.WriteLines("This command does not exist. Only commands displayed under the input box are possible choices.");
                CommandsToSendOverInternet.Add("FAIL: " + obj.Text);
            }
            obj.Text = "";
            OldInputText = "";
        }
     
        protected override void Update(GameTime gameTime)
        {
             if (Root.WasKeyPressed(Keys.Enter, ModifierKey.Alt))
            {
                graphics.ToggleFullScreen();
            }

            InputTextBox.Update();
            if (InputTextBox.Text != OldInputText)
            {
                OldInputText = InputTextBox.Text;
                ChosenSuggestion = 0;
            }
            if (Root.WasKeyPressed(Keys.Down))
            {
                ChosenSuggestion++;
                if (ChosenSuggestion >= Math.Min(maxsuggestioncount, AvailableCommands.Count))
                {
                    ChosenSuggestion = 0;
                }
            }
            if (Root.WasKeyPressed(Keys.Up))
            {
                ChosenSuggestion--;
                if (ChosenSuggestion < 0)
                {
                    ChosenSuggestion = Math.Min(maxsuggestioncount, AvailableCommands.Count) - 1;
                }
            }
            CurrentSuggestions = new List<Command>();
            foreach (Command c in AvailableCommands)
            {
                if (c.TotalString.ToUpper().Contains(InputTextBox.Text.ToUpper()))
                {
                    CurrentSuggestions.Add(c);
                }
            }
            if (InputTextBox.Text.ToUpper().StartsWith("FEEDBACK "))
            {
                CurrentSuggestions.Add(new Command("feedback", InputTextBox.Text.Substring("feedback ".Length)));
            }


            // Online
            if (!OnlineWorker.IsBusy)
            {
                List<string> commands = new List<string>();
                foreach(string cmd in CommandsToSendOverInternet)
                {
                    if (TrackingPermitted || cmd.ToUpper().StartsWith("FEEDBACK"))
                    {
                        commands.Add(cmd);
                    }
                }
                CommandsToSendOverInternet.Clear();
                OnlineWorker.RunWorkerAsync(commands);
            }

            

            Root.Update(gameTime);
            base.Update(gameTime);
        }

        public static Rectangle rectConsoleTop = new Rectangle(3, 3, 1024 - 6, 500);
        public static Rectangle rectConsoleTopInner = new Rectangle(3+4, 3+4, 1024 - 6-8, 500-8);
        Rectangle rectInputLine = new Rectangle(3, 506, 1024 - 6, 30);
        Rectangle rectSuggestions = new Rectangle(3, 539, 1024 - 6 - 203, 226);
        Rectangle rectRecommendedCommands = new Rectangle(1024-203, 539, 200, 226);
        Textbox InputTextBox;
        public int ChosenSuggestion = 0;
        public string OldInputText = "";
        public int maxsuggestioncount = 0;
        public List<Command> CurrentSuggestions = new List<Command>();

        protected override void Draw(GameTime gameTime)
        {
            Color clrFluttershy = Color.FromNonPremultiplied(248, 247, 152, 255);
            GraphicsDevice.Clear(Color.White);//248,247,152
            Color clrPink = Color.FromNonPremultiplied(248, 185, 206, 255);
            Color clrButterscotch = Color.FromNonPremultiplied(226, 187, 50, 255);
            spriteBatch.Begin();

            // Console output
            Primitives.DrawAndFillRectangle(rectConsoleTop, clrFluttershy, Color.Black, 2);
            for (int i = 0; i < ConsoleMaxLines; i++)
            {
                int lineID = i + ConsoleOutputCurrentLine;
                if (lineID >= Session.ConsoleTotalLines) break;
                Primitives.DrawSingleLineText(Session.ConsoleOutput[lineID], new Vector2(rectConsoleTopInner.X, rectConsoleTopInner.Y + Library.FontConsoleNormal.LineSpacing * i),
                    lineID < GreyOutputUntil ? Color.Black.Alpha(50) : Color.Black, null, 1);
            }

            // Inputbox
            Primitives.DrawAndFillRectangle(rectInputLine, clrFluttershy, Color.Black, 2);
            InputTextBox.Draw();

            // Suggestions
            Primitives.DrawAndFillRectangle(rectSuggestions, clrFluttershy, Color.Black, 2);
            List<Command> suggestions = CurrentSuggestions;
            int lnSpacing = Library.FontConsoleNormal.LineSpacing + 2;
             maxsuggestioncount = rectSuggestions.Height / lnSpacing;
            if (ChosenSuggestion < 0) ChosenSuggestion = maxsuggestioncount - 1;
            if (ChosenSuggestion > maxsuggestioncount - 1) ChosenSuggestion = 0;
            for (int i = 0; i < maxsuggestioncount; i++)
            {
                if (suggestions.Count <= i) break;
                Command c = suggestions[i];
                Rectangle rectSuggestion = new Rectangle(rectSuggestions.X + 3, rectSuggestions.Y + 2 + i * lnSpacing, rectSuggestions.Width - 6, lnSpacing);
                Primitives.DrawSingleLineText(c.TotalString, new Vector2(rectSuggestion.X + 5, rectSuggestion.Y+1), Color.Black);
                if (ChosenSuggestion == i)
                {
                    Primitives.FillRectangle(rectSuggestion, clrPink);
                    Primitives.DrawSingleLineText(c.TotalString, new Vector2(rectSuggestion.X+5, rectSuggestion.Y+1), Color.Black);
                }
            }
            if (maxsuggestioncount < suggestions.Count)
            {
                Primitives.DrawSingleLineText((suggestions.Count - maxsuggestioncount) + " more suggestions are hidden",
                    new Vector2(rectSuggestions.X + rectSuggestions.Width - 8 - Library.FontConsoleNormal.MeasureString((suggestions.Count - maxsuggestioncount) + " more suggestions are hidden").X,
                        rectSuggestions.Y + rectSuggestions.Height - lnSpacing), Color.Black);
            }

            // Fluttershy
            Primitives.DrawAndFillRectangle(rectRecommendedCommands, clrFluttershy, Color.Black, 2);
            Primitives.DrawImage(ImageFluttershy, new Rectangle(rectRecommendedCommands.X + 5, rectRecommendedCommands.Y + 5, rectRecommendedCommands.Width - 10, rectRecommendedCommands.Height - 10),
                Color.White, true, true, Color.Transparent);

            Root.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
