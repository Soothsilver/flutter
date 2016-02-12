using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImprovedXnaGame
{
    [Serializable]
    public class Location
    {
        public LocationID ID;
        public string Description;
        public List<Item> Items = new List<Item>();
        public Action OnEnter;
        public string BeforeEnter;
 
        public Location(LocationID id, string description)
        {
            ID = id;
            Description = description;
        }

        internal void GetSpecialCommands(Session session, out List<Command> avails, out List<Command> recomends)
        {
            avails = new List<Command>();
            recomends = new List<Command>();
        }
    }

    [Serializable]
    public enum LocationID
    {
        Splash,
        Cottage,
        Amethyst,
        Cave,
        Puzzle,
        FinalRoom,
        LocReturn,
        FIN,
        TotalEnd
    }
}
