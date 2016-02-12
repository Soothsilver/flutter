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
    public class GamePhase
    {
        public List<UIElement> UIElements = new List<UIElement>();
        public MessageBoxResult ReturnedMessageBoxResult = MessageBoxResult.Awaiting;
        public GamePhaseID ID { get; set; }
        public bool InTransition { get; set; }
        public bool ScheduledForElimination { get; set; }

        public void AddUIElement(UIElement ui)
        {
            UIElements.Add(ui);
        }
        public virtual void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw();
            }
        }
        public virtual void Update(Game game, float elapsedSeconds)
        {
            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Update();
            }
        }
        public virtual void Initialize(Game game)
        {

        }
        public virtual void Destruct(Game game)
        {
            this.ScheduledForElimination = true;
        }

        public GamePhase(GamePhaseID id)
        {
            ID = id;
        }
        public GamePhase()
        {
            ID = GamePhaseID.Null;
        }
     
    }
    /// <summary>
    /// Identifies the type of game phase. Feel free to add your proper IDs using Refactor options.
    /// </summary>
    public enum GamePhaseID
    {
        Null,
        Intro,
        MainMenu,
        Credits,
        Ingame,
        LoadingScreen
    }
}
