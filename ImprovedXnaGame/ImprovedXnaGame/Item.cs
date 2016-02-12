using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImprovedXnaGame
{
    [Serializable]
    public class Item
    {
        public ItemID ID;
        public string Name;
        public Dictionary<string, Action> PermittedCommands;

        public string Description;
        public bool OnTheOtherSide;
        public int Number;
        public string AfterExamination;
        public override string ToString()
        {
            return Name;
        }
        public Item(ItemID id, string name, string description)
        {
            Name = name;
            Description = description;
            ID = id;
            PermittedCommands = new Dictionary<string, Action>();
        }
        public Item( string name, string description)
        {
            Name = name;
            Description = description;
            ID = ItemID.Notimportant;
            PermittedCommands = new Dictionary<string, Action>();
        }
        public Item AddCommand(string name)
        {
            this.PermittedCommands.Add(name, null);
            return this;
        }
        public Item AddCommand(string name, Action action)
        {
            this.PermittedCommands.Add(name, action);
            return this;
        }
    }
    public enum ItemID
    {
        Spellbook,
        UnicorniaDaily,
        VioletFlask,
        BlueFlask,
        OrangeFlask,
        GreenFlask,
        CloverPhase1,
        Writings,
        Diamond1,
        MetalDoor,
        Diamond2,
        Pony_RainbowDash,
        Pony_Fluttershy,
        Pony_Rarity,
        Pony_PinkiePie,
        Pony_Applejack,
        Pony_Twilight,
        Rope,
        Instructions,
        FinalDoor,
        Monitor_Western,
        Monitor_Northern,
        Monitor_Eastern,
        Monitor_Southern,
        Gift,
        CottageDoor,
        Bear,
        Birds,
        Angel,
        Caterpillar,
        Notimportant,
        Gate,
        NonExaminable

    }
}
