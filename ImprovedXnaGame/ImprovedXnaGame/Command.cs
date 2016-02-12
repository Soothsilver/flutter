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
namespace ImprovedXnaGame
{
    public partial class Session
    {
        private Item Flag_FutureSet;
        public void UseCommand(Command c)
        {
            Item target = c.CommandObject as Item;
            if (!UtilizedCommands.Contains(c.ToString()))
            {
                UtilizedCommands.Add(c.ToString());
            }
            switch (c.CommandWord)
            {
                case "clear":
                    Clear();
                    break;
                case "toggle fullscreen":
                    Root.IsFullscreen = !Root.IsFullscreen;
                    break;
                case "help":
                    @WriteLines(@"This is a parser-based text adventure game. You may not be familiar with this type of game because they are mostly extinct for some twenty years. The principle is that you roleplay an adventure by typing commands and then pressing Enter. You will not need your mouse at all. 

To use a command, type in several letters from it (for example, start typing ""examine"" or ""door""). The suggestion system will automatically find appropriate commands for you to use. Choose among them with arrow keys and then press Enter to confirm your choice. It is impossible to lose the game or become stuck.

Most useful commands:
CLEAR - Clears the screen.
HELP - Shows this screen.
LOOK AROUND - Gives you a description of the room you're in usually with names of items you can interact with.
RESTORE ACT [X] - Loads the game from a specified point. You must have reached that point previously, though.
EXAMINE - Gives a detailed description of an item or pony.
FEEDBACK [Your message here] - Sends feedback over internet to the game creator. 

If you don't know what exactly you can do with an item, just type the item's name and a list of options will present itself to you."); 
                    break;
                case "exit":
                    ImprovedGame.Main.Exit();
                    break;
                case "look around":
                    WriteLines(GetLocationDescription(CurrentLocation));
                    break;
                case "feedback":
                    if (c.CommandObject == null)
                    {
                        WriteLines("Feedback was not sent.\nTo submit textual feedback, you must type 'feedback (text to submit)', for example 'feedback I hated this game.'.");
                    }
                    else if ((string)c.CommandObject == "" || (string)c.CommandObject == " ")
                    {
                        WriteLines("Feedback was not sent.\nTo submit textual feedback, you must type 'feedback (text to submit)', for example 'feedback I hated this game.'.");
                 
                    }
                    else
                    {
                        WriteLines("Feedback was sent. Thank you very much.\nYour feedback was: \"" + c.CommandObject + "\".");
                    }
                    break;
                case "examine":
                    WriteLines(target.Description);
                    if (target.AfterExamination != null)
                    {
                        if (target.Name != target.AfterExamination)
                        {
                            target.Name = target.AfterExamination;
                            target.PermittedCommands.Add("move orb to", () =>
                            {
                                Flag_FutureSet = target;
                                EnterLocation(locReturn);
                            });
                        }
                    }
                    break;
                case "restore act 2":
                    ImprovedGame.Main.Session = new Session();
                    ImprovedGame.Main.PreviousConsoleTotalLines = 0;
                    ImprovedGame.Main.GreyOutputUntil = -1;
                    ImprovedGame.Main.ConsoleOutputCurrentLine = 0;
                    ImprovedGame.Main.Session.Clear();
                    ImprovedGame.Main.Session.WriteLines("Loading act 2.");
                    ImprovedGame.Main.Session.EnterLocation(ImprovedGame.Main.Session.locationAmethyst);
                    break;
                case "restore act 3":
                    ImprovedGame.Main.Session = new Session();
                    ImprovedGame.Main.Session.Clear();
                    ImprovedGame.Main.Session.WriteLines("Loading act 3.");
                    ImprovedGame.Main.Session.EnterLocation(ImprovedGame.Main.Session.locPuzzle);
                    break;
                case "restore act 4":
                    ImprovedGame.Main.Session = new Session();
                    ImprovedGame.Main.Session.Clear();
                    ImprovedGame.Main.Session.WriteLines("Loading act 4.");
                    ImprovedGame.Main.Session.EnterLocation(ImprovedGame.Main.Session.finalRoom);
                    break;
                case "go forwards through the":
                    yourPosition++;
                    EnergyLevel += gates[target.Number];
                    WriteLines(GetPuzzleDescription());
                    break;
                case "go backwards through the":
                    yourPosition--;
                    EnergyLevel += gates[target.Number];
                    WriteLines(GetPuzzleDescription());
                    break;
                
                //----------------------------- GENERIC COMMANDS --------------------------------//
          
                
                default:
                    var kvp = target.PermittedCommands[c.CommandWord];
                    if (kvp != null)
                    {
                        kvp();
                    }
                    break;

            }
        }

        private void Clear()
        {
            ConsoleOutput = new List<string>();
            ConsoleTotalLines = 0;
            ImprovedGame.Main.GreyOutputUntil = -1;
            ImprovedGame.Main.ConsoleOutputCurrentLine = 0;
            ImprovedGame.Main.PreviousConsoleTotalLines = 0;
        }

   
    }
    public partial class ImprovedGame : Microsoft.Xna.Framework.Game
    {
        public List<Command> AvailableCommands = new List<Command>();
        public List<Command> RecommendedCommands = new List<Command>();
        public void SendCommand(Command c)
        {
            Session.UseCommand(c);
            Session.GetAvailableCommands(out AvailableCommands, out RecommendedCommands);
        }
    }
    public class Command
    {
        public string CommandWord;
        public object CommandObject;
        public string TotalString;

        public override string ToString()
        {
            return TotalString;
        }

        public Command(string word, object target = null)
        {
            CommandWord = word;
            CommandObject = target;
            TotalString = CommandWord + (target != null ? " " + CommandObject.ToString() : "");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Command other = obj as Command;
            return (other != null && other.TotalString == this.TotalString);
        }
    }
}
