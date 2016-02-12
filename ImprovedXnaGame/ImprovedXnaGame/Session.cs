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
using System.IO;

namespace ImprovedXnaGame
{
    [Serializable]
    public partial class Session
    {
        private static String actFile;
        static Session()
        {
            actFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            actFile = System.IO.Path.Combine(actFile, "flutters");
            Directory.CreateDirectory(actFile);
            actFile = Path.Combine(actFile, "act.txt");
        }

        private Location locationAmethyst;
        private Location locPuzzle;
        private Location finalRoom;
        private Location locReturn;
        private int[] gates = new int[] { 1, -3, 2, -3, -3 };
        private int yourPosition = 0;
        private String GetPuzzleLocationDescription()
        {
            return locPuzzle.Description + "\n" + GetPuzzleDescription();
        }
        private String GetPuzzleDescription()
        {
            String ttt = "\n";
                for (int i = 0; i < 5; i++)
                {
                    if (yourPosition == i)
                    {
                        if (i == 0)
                        {
                            ttt += "You are in front of the First Gate.\n";
                        }
                        else
                        {
                            ttt += "You are between gates " + (i) + " and " + (i + 1) + ".\n";
                        }
                    }
                    ttt += "Gate " + (i + 1) + " is " + (gates[i] > 0 ? "green" : "red") + " and its pillars bear the inscription \"" + gates[i] + "\".\n";
                }
                if (yourPosition == 5)
                {
                    ttt += "You are behind the last gate. If you feel healthy, you may open the metal door to go further.\n";
                }
            
            string energyStatus = "You feel healthy.";
            if (EnergyLevel < 0)
            {
                energyStatus = "Your energy status is " + EnergyLevel + " (too little).\n\nYou feel very sad and drained of energy. If you leave the room like this, you will be absolutely unable to enjoy Hearth's Warming Eve celebrations. You must stay until you raise it back to zero.";
            }
            if (EnergyLevel > 0)
            {
                energyStatus = "Your energy status is " + EnergyLevel + " (too much).\n\nYou feel super excited but can't think straight. If you leave the room like this, your animals won't be able to understand you, nor you them. You must stay until you reduce it back to zero.";
            }
            return ttt + "\n" + energyStatus;
        }
        private void Act4()
        {

            locReturn = new Location(LocationID.LocReturn, "You are back in front of your cottage at the border of the Everfree, the ancient amulet cracked in your hooves. It no longer works. Perhaps you want to ENTER the COTTAGE?");
            locReturn.BeforeEnter = @"You grab the Orb and set it into the receptacle of the chosen alcove. You feel a wave of happiness wash over you as another blinding flash of light whisks you away to the surface, close to your home.

As you come to your senses, you realize that the Emotion Orb contained powerful magics, including a primed fateseal spell. Fateseal spells are long lost to ponykind, the last conjuror able to cast one was Star Swirl, and the legend says that it was his very last act. 

But now the Emotion Orb is gone and you may return home and see the result of your actions.";
            Item iCottage = new Item("cottage", "You see your home. You wonder what awaits you inside.");
            iCottage.AddCommand("enter", () =>
            {
                EnterLocation(CreateFinalRoom());
            });
            locReturn.Items.Add(iCottage);

            finalRoom = new Location(LocationID.FinalRoom, "You are in a small room with no exits. There is a levitating EMOTION ORB floating in the center of the room. Four ALCOVES are set in the walls, each containing a receptacle for the Orb. You feel that this is what you came here to do. Here your decision will be made, to permanently affect the Hearth's Warming Eve for all time. EXAMINE the alcoves carefully before putting the Orb where you think it belongs.");
            finalRoom.BeforeEnter = @"
ACT 4 BEGINS.
Your progress has been saved to disk. Type ""restore act 4"" at any time to continue from this point.

You open the metal door and walk into the final room.";

            Item west = new Item("western alcove", @"In the western alcove, you see engraved images of happy ponies dancing around a fireplace. 

You feel that if you put the orb in the western alcove, its magic will slightly boost the happy emotions of all ponies across all of Equestria for the duration of a few days around the Hearth's Warming Eve. All ponies will be slightly happier this time of year.

This temporary boost will happen regularly, annually and is irreversible.");
            west.AfterExamination = "western alcove (global happiness)";
            Item north = new Item("northern alcove", @"In the northern alcove, you see engraved images of a few ponies smiling, huddled around a fire in the shape of a heart, but also a few sad ponies, freezing and surrounded by windigos.

You feel that if you put the orb in the northern alcove, then during the Hearth's Warming Eve each year, its magic will judge ponies based on how they acted in their friendships. If they were good friends, then the orb will render them much happier for the holidays; if instead they were bad friends or didn't even have any friends, the Orb will render them depressed and cause them minor physical pain.

This temporary effect will happen regularly, annually and is irreversible.");
            north.AfterExamination = "northern alcove (judgement)";
            Item east = new Item("eastern alcove", @"In the eastern alcove, you see engraved images of formally dressed ponies performing a Hearth's Warming Eve play.

You feel that if you put the orb in the eastern alcove, it will disappear and cause no change upon Equestria. Ponies will enjoy Hearth's Warming Eve as normal, without any extra magical influence messing with their emotions.

This choice is permanent; the Emotion Orb will never return to Equestria.");
            east.AfterExamination = "eastern alcove (no change)";
            Item south = new Item("southern alcove", @"In the southern alcove, you see engraved images of a few sad ponies surrounded by friends. It seems like their frowns are changing into smiles somehow.

You feel that if you put the orb in the southern alcove, its magic will significantly boost the happy emotions of all depressed ponies and ponies with significant negative moods. This will cause them to be happy during Hearth's Warming Eve and while the magic will fade with the festival, it may be enough for some of them to rid themselves of depression completely, or at least for a time.

This temporary boost will happen regularly, annually and is irreversible.");
            south.AfterExamination = "southern alcove (depressed only)";
            Item orb = new Item("Emotion Orb", "This device can be carried to an examined alcove. Doing that will set magical events in motion.");

            finalRoom.Items.AddRange(new[] { west, east, north, south, orb });
            finalRoom.OnEnter = () =>
            {
                ReachedAct(4);
            };
        }

        private Location CreateFinalRoom()
        {
            /*
             * east = no change
             * south = depressed
             * north = judgement
             * west = happy
             */
            string description = "";
            string beforeEnter = "You enter your cottage. ";
            if (Flag_FutureSet.Name.Contains("west"))
            {
                description = @"You scarcely recognize your home. All of the critters are scuttling from here to there, cleaning the room, hanging out additional decorations and packing more presents. They all look very determined. In the midst of them all stands Amethyst Star, a notebook levitating in front of her. When they notice you, all the critters embrace you such that only your face remains uncovered. 

How sweet of them. They were already happy but your activation of the Emotion Orb gave them additional energy to do what they wanted to. And under the guidance of Amethyst, they wanted to get you the best celebration you have ever had. You had an opportunity to improve the lives of all ponies and you took it; nopony should be sad on Hearth's Warming Eve, a day of joy.

You have chosen correctly, for if making ponies happy is not good, then what is?

Amethyst Star offers a shy smile, ""I can imagine your adventure must have been terrifying... I wanted to get you a reward of some kind, so I helped your animals do this. I hope it's enough. Now I suppose... I should return to my place, I think... I should probably start prepping the New Year's fireworks...""";
            }
            if (Flag_FutureSet.Name.Contains("east"))
            {
                description = @"You are at home. The animals all looked up to you and you feel they are greatly relieved that you returned to them, and healthy. They embrace you and soon only your face can be seen through all the critters who surround you. 

You have wrought no change upon Equestria, but that is as it should be. Feelings should be true, not manipulated by drugs or mental magic. Who knows what disaster would it cause if they were made happy against their will?

You have chosen correctly, for you had no right to change everypony.

You see Amethyst Star in the corner of your room, abandoned. ""I take it everything went alright?"" she attempted a smile, ""I suppose... I should now return to my place, I think... I need to start prepping the New Year's fireworks...""";
            }
            if (Flag_FutureSet.Name.Contains("north"))
            {
                description = @"You are at home and you see a whimpering Amethyst Star crouched on the ground, surrounded by animals. She's sobbing and from time to time twitches from pain and sometimes even cries out. ""I'm a bad pony,"" she says to you as you enter your home, ""I'm not really good at friendships, I spend all of my time in my office, working..."" she twitches from another jolt of pain, ""...I deserve this, I promise I will try harder next year...""

It pains you to see Amethyst in a state such as this but you know it was the only feasible choice. Hearth's Warming Eve will return to its roots, when ponies had to be friends or face destruction at the hooves of the Windigos. The punishment of temporary depression and pain is much less severe by comparison. With this new festival, ponies will be more motivated to become friends and the spirit of friendship will soar across Equestria.

You have chosen correctly, for the entire world will get better.

""I... ouch... should probably leave now. I wouldn't want to... to spoil your celebration with my weakness and my whimpers.""";
            }
            if (Flag_FutureSet.Name.Contains("south"))
            {
                description = @"As soon as you close the door behind yourself, already you feel Amethyst Star around your neck, hugging you tightly. ""Thank you, Fluttershy,"" she says, ""I can't even begin to express how grateful I am to you. I know this will not last, but... today is the first time in months that I feel true happiness. I am pretty good at faking happiness in public, but really, my life is a mess and I sometimes wonder why I'm doing anything at all. You're a hero, Fluttershy.""

Amethyst's reaction warms your heart and confirms to you that you were right. While we could all use a little more happiness, to some ponies life is downright unfair. It is they who need, and deserve, our help the most. The Emotion Orb was a powerful artifact and you have used it to help the most vulnerable who need emotional support the most.

You have chosen correctly, for if we turned our back on the ponies who need help, would we deserve to call ourselves ponies at all?

""But now,"" says Amethyst, ""I should probably go. I have used up too much of your generosity. You've already helped me greatly, I think I'll be okay on my own... goodbye...""";
            }
            Item i = new Item("Amethyst", "Amethyst Star looks at you, insecure. It appears nopony awaits her back home and that, like you, she is used to spend the Eve celebration alone.");
            i.AddCommand("please stay,", () => 
            {
                EnterLocation(SpawnEnd(true, Flag_FutureSet));

            });
            i.AddCommand("see you later,", () =>
            {
                EnterLocation(SpawnEnd(false, Flag_FutureSet));

            });
            Location fin = new Location(LocationID.FIN, description);
            fin.BeforeEnter = beforeEnter;
            fin.Items.Add(i);

            return fin;
        }

        private Location SpawnEnd(bool saty, Item alcove)
        {
            /*
             * east = no change
             * south = depressed
             * north = judgement
             * west = happy
             */
            string desc = @"Amethyst tries for a weak smile and exits your cottage. This has been quite the adventure but now it's over, you have no more responsibilities. Everyone around you is now happy, from caterpillars to birds, from Angel to Harry, and all wait for you to join them.

""Let's have fun, everyone!"" you say and they all join in a cheer. Yes, this will be the best Hearth's Warming Eve, you and all your animal friends, sharing kindness together. You notice a bird is speaking to you but you don't listen to her; it reminds you that soon you'll have to start practicing with them for the one thousandth Summer Sun Celebration. It will be quite a big event. 

As you apologize to the bird for not listening, you wonder whether some day, you'll perhaps be sharing the Eve celebration also with other ponies...";
            if (saty)
            {
                desc = @"""Really? You would allow me to share the evening with you?"" 

You nod, ""and the night, too. I... I don't remember the last time I celebrated Hearth's Warming Eve with another pony... I think I would like that...""

Amethyst hugs you in an embrace so warm you've never experienced something like it. And then, together, you, Amethyst and each of your animal friends go on to enjoy your best Eve celebration ever. Amethyst quickly befriends the critters and soon you are all laughing and playing together. You never really appreciated large social gatherings but maybe, maybe friendship isn't only like that, maybe there are different kinds of friendship; and maybe some of them don't require you to stop being shy. You smile and promise you will invite Amethyst again next year.";
                if (alcove.Name.Contains("north"))
                {
                    desc = @"""Really? You would - ouch - allow me to share the evening with you?"" 

You nod, ""and the night, too. I... I don't remember the last time I celebrated Hearth's Warming Eve with another pony... I think I would like that...""

Amethyst hugs you in an embrace so warm you've never experienced something like it. Tears disappear from her eyes and her convulsions stop. Her frown turns to smile and she sighs with relief. And then, together, you, Amethyst and each of your animal friends go on to enjoy your best Eve celebration ever. Amethyst quickly befriends the critters and soon you are all laughing and playing together. You never really appreciated large social gatherings but maybe, maybe friendship isn't only like that, maybe there are different kinds of friendship; and maybe some of them don't require you to stop being shy. You smile and promise you will invite Amethyst again next year.";
                }
            }
            desc += @"

YOU HAVE WON THE GAME! Congratulations!
Thank you for playing the game. I hoped you enjoyed playing it as much as I did making it.

If you wish to see different endings, remember that you can type ""restore act 4"" to get back to the Emotion Orb. Also remember that you can use the feedback command to send information to the game author, for example, ""feedback I hated this game."". If you wish to provide longer feedback, you may write an e-mail to petrhudecek2010@gmail.com. If you liked this game, you may also like ""Star Swirl's Parser-Based Pony Adventure"".";

            Location l = new Location(LocationID.TotalEnd, desc);
            return l;
        }
        private void Act3()
        {
            Act4();
            locPuzzle = new Location(LocationID.Puzzle, "You are in a long corridor that contains five shimmering GATES of light. The gates are strange - each consists of two pillars with a screen of colorful light between them. There is no way around, if you want to proceed, you must go through the gates. In front of the first gate is a RESET DEVICE and beyond the last gate is a METAL DOOR. You may also get a HINT if this puzzle proves too hard for you.");
            locPuzzle.BeforeEnter = @"
ACT 3 BEGINS.
Your progress has been saved to disk. Type ""restore act 3"" at any time to continue from this point.

You climb through the opening into a strange room. It appears to be some kind of a puzzle.";
            Item iHint = new Item("hint", "This hint will reveal how the puzzle works.");
            iHint.AddCommand("get small", () =>
            {
                WriteLines("Use the commands starting with GO to move between the gates. Gates have an effect on you when you pass through them.");
            });
            iHint.AddCommand("get medium", () =>
            {
                WriteLines("To solve this puzzle, you need to get beyond the Fifth gate while having energy status 0. Note that gates have the same effect on you when you go backwards as well!");
            });
            iHint.AddCommand("get big", () =>
            {
                WriteLines("Passing through a +2 gate, in any direction, increases your energy status by 2. What you want to do is to move between gates in such a way that beyond the Second Gate, you have energy status 0, beyond the Third Gate, you have energy status 6, and then you can simply pass the final two gates and end up with the energy status 0.");
            });
            for (int i = 0; i < 5; i++)
            {
                string gatename = "";
                switch (i)
                {
                    case 0: gatename = "First Gate"; break;
                    case 1: gatename = "Second Gate"; break;
                    case 2: gatename = "Third Gate"; break;
                    case 3: gatename = "Fourth Gate"; break;
                    case 4: gatename = "Fifth Gate"; break;
                }
                Item iGate = new Item(gatename, gatename + " is " + (gates[i] > 0 ? "green" : "red") + " and its pillars bear the inscription \"" + gates[i] + "\".");
                iGate.ID = ItemID.Gate;
                iGate.Number = i;
                locPuzzle.Items.Add(iGate);
            }

            Item resetDevice = new Item("Reset Device", "This button bears the text 'reset'. It appears that when you press it, the room, including you, will return to how it was when you first entered.").AddCommand("activate", () =>
            {
                if (yourPosition != 0) {
                    WriteLines("You are not in front of the first gate, and so you cannot reach the Reset Device. Sorry.");
                    return;
                }
                EnergyLevel = 0;
                WriteLines("Your energy status has been reset to zero. You feel healthy now.");
            });
            Item metalDoor = new Item("metal door", "This door leads from this room further into the dungeon.");
            metalDoor.AddCommand("open", () =>
            {
                if (yourPosition != 5)
                {
                    WriteLines("You are not beyond the fifth gate and so you cannot reach the metal door.");
                    return;
                }
                if (EnergyLevel != 0)
                {
                    WriteLines("Your energy status is not zero. You are not feeling healthy enough to be able to leave this room.");
                    return;
                }
                EnterLocation(finalRoom);
            });
            locPuzzle.Items.AddRange(new[] { metalDoor, resetDevice });
            locPuzzle.Items.Add(iHint);
            locPuzzle.OnEnter = () =>
            {
                ReachedAct(3);
                yourPosition = 0;                
            };
        }
        private void Act2()
        {
            Act3();
            locationAmethyst = new Location(LocationID.Amethyst, "You are speaking to AMETHYST STAR, the star organizer of Ponyville, in your cottage near the Everfree. On the table is an AMULET.");
            locationAmethyst.BeforeEnter = @"
ACT 2 BEGINS. 
Your progress has been saved to disk. Type ""restore act 2"" at any time to continue from this point.

Amethyst Star bursts into the room and deposits some kind of amulet on the table, ""disaster! We must act quickly, Fluttershy!""

""What is happening, Amethyst?""

""This is the Narrative Amulet,"" says Amethyst, ""legend says - no, that is not an old mares' tale like the Mare in the Moon - this legend is actually true, the legend says that when the Narrative Amulet is found again, the one who cares most about others must take it and fight, or else ponies will not know any happiness for the interval of a whole moon. I think it means you.""";

            var locCave = new Location(LocationID.Cave, "You are underground in a dark cave, illuminated by phosphorescent fungi on the walls. You smell deep roots that don't grow anywhere except in the Everfree forest. There appears to be no way up. However, you can see an INSCRIPTION on one of the walls, right besides a WOODEN LEVER, a STONE LEVER and a DIAMOND LEVER.");
            locCave.BeforeEnter = @"You reach for the amulet and as soon as you touch it, a flash of brilliant light blinds you. You scream in surprise and feel yourself taken away. Just a moment later, when you reopen your eyes, you see you're in a completely unfamiliar location. You were teleported.";
            locationAmethyst.OnEnter = () =>
            {
                ReachedAct(2);
            };

            Item cInscription = new Item("inscription", @"It reads,

<< On the fields there is a single wooden ball, two stone slabs and four diamond pyramids. The Great Thinker looks at them and thinks. She needs to choose something to bring back home, what will it be? ""If what I bring,"" she reasons, ""doubled and by one increased, is the same as what I leave behind, then I have brought home a good thing. But if instead I bring home something else, then I shall suffer in pain."" >>");

            Item cWood = new Item("wooden lever", "It's lever made off hard wood.").AddCommand("pull", () =>
            {
                if (Flag_Wounded)
                {
                    WriteLines("Are you really sure you want to do this? ...I thought so.");
                    return;
                }
                if (Flag_Injured)
                {
                    WriteLines("You timidly approach the wooden lever, then pull it quickly. Immediately a forceful gust of wind throws you on the ground. You cry out as the scratches on your face burn as if they were on fire. The entire ordeal lasts only for a few seconds and then you feel only a lingering itch. You pick yourself up from the ground. You must be more careful now. You are now Wounded.");
                    Flag_Wounded = true;
                }
                else
                {
                    WriteLines("You pull the wooden lever. Even as you do so, you already feel the choice was not right. When the lever reached its final position, a cloud of dust explodes from a hole in the wall straight into your face. You close your eyes just in time but the specks still hurt you.  You retreat a few steps and rub your face, trying to get the dust specks off you. You mostly succeed but your face is scratched and hurts. You are now Injured.");
                    Flag_Injured = true;
                }
            });
            Item cOpening = new Item("opening", "There's a hole in the wall.");
            cOpening.AddCommand("climb through", () =>
            {
                EnterLocation(locPuzzle);
            });
            Item cStone = new Item("stone lever", "This lever is rather inconspicuous in this cave.").AddCommand("pull", () =>
            {
                if (Flag_Wounded || Flag_Injured)
                {
                    WriteLines("You hesitantly pull the stone lever but you've chosen well this time. An OPENING appears in the wall that you can climb through. Also, a wave of excitement washes over you, suppressing all of your injuries. You are now Healthy.");

                }
                else
                {
                    WriteLines("You confidently pull the stone lever and are not surprised to see you've chosen well. An OPENING appears in the wall that you can climb through. Also, a wave of excitement washes over you, making you curious what you will encounter next.");
                }
                locCave.Items.ForEach(tm => tm.PermittedCommands.Remove("pull"));
                locCave.Items.Add(cOpening);
                locCave.Description += " There's also an OPENING in the wall.";
            });
            Item cDiamond = new Item("diamond lever", "This lever is made of pure diamond. You think it's rather pretty.").AddCommand("pull", () =>
            {
                if (Flag_Wounded)
                {
                    WriteLines("Are you really sure you want to do this? ...I thought so.");
                    return;
                }
                if (Flag_Injured)
                {
                    WriteLines("You timidly approach the diamond lever, then pull it quickly. Immediately a forceful gust of wind throws you on the ground. You cry out as the scratches on your face burn as if they were on fire. The entire ordeal lasts only for a few seconds and then you feel only a lingering itch. You pick yourself up from the ground. You must be more careful now. You are now Wounded.");
                    Flag_Wounded = true;
                }
                else
                {
                    WriteLines("You pull the diamond lever. Even as you do so, you already feel the choice was not right. When the lever reached its final position, a cloud of dust explodes from a hole in the wall straight into your face. You close your eyes just in time but the specks still hurt you.  You retreat a few steps and rub your face, trying to get the dust specks off you. You mostly succeed but your face is scratched and hurts. You are now Injured.");
                    Flag_Injured = true;
                }
            });


            locCave.Items.AddRange(new Item[] { cInscription, cWood, cStone, cDiamond });



            Item amethyst = new Item("Amethyst Star", "The pony across the table has a violet coat and mane and appears very concerned.");
            amethyst.AddCommand("talk to", () =>
            {
                WriteLines(@"""Are you sure it means me, Amethyst?"" you ask, ""I can think of plenty other ponies who care about others. Well, I could if I knew many ponies, that is... you, perhaps...""

""I could be wrong, Fluttershy,"" admits Amethyst, ""but it's unlikely. Believe me, I know the talents of everypony in this town. And while ponies such as Pinkie Pie appear to care a lot, they do it mostly instinctively. Pinkie Pie *enjoys* it, while you, you expend far more time and care even on animals who don't care about you at all. I'm pretty confident the amulet means you.""");
            }).AddCommand("hug", () =>
            {
                WriteLines(@"You jump on Amethyst and hug her. She is very surprised by this but tries to comfort you. ""Why does it have to be me?"" you whimper into her arms.

""I know it's unfair, Fluttershy,"" says Amethyst, ""but know that the amulet will lead you to a decision and that only the most caring of ponies can be trusted to make such a decision.""");
            }).AddCommand("kiss", () =>
            {
                WriteLines(@"You lean over and kiss Amethyst on the lips. To your surprise, she doesn't move away. Not immediately, anyway.

""Uh, um, Flutters, uh,"" she blushes, ""I don't know, I, uh, ...maybe we should take care of the amulet, first?""

Now you berate yourself for making her uncomfortable. However, it does appear she didn't mind your kiss that much, or perhaps even at all?");
            });
            Item amulet = new Item("amulet", "On the ancient amulet is the drawing of a magical orb suspended in mid-air. The amulet is somewhat cracked.");
            amulet.AddCommand("touch", () =>
            {
                EnterLocation(locCave);
               
            });
            locationAmethyst.Items.AddRange(new Item[] { amethyst, amulet });

        }

        private void ReachedAct(int no)
        {
            try
            {
                try
                {
                    String rd = System.IO.File.ReadAllText(actFile);
                    int i = int.Parse(rd);
                    if (i > no) return;
                }
                catch
                {
                }
                System.IO.File.WriteAllText(actFile, no.ToString());
                WriteLines("Your progress has been saved.", true);
            }
            catch
            {
                WriteLines("PROGRAM ERROR. The game attempted to save your progress to disk but the save failed. I am sorry. You will probably need to finish the game in a single session or you'll need to start over from the beginning. Don't worry, though, you're almost at the end.", true);
            }
        }
        public Session()
        {
            Act2();
            Location splash = new Location(LocationID.Splash,
                @"Dear Fluttershy,
I hope you are enjoying yourself during the Hearth's Warming Eve. I'm sure you and your animals must all be very happy. As a gesture of thanks for your work for Ponyville, I prepared a little GIFT for you. You may open it using the command 'open gift'. And if at any time you are unsure of what to do, the commands LOOK AROUND and HELP could be of assistance.

Yours truly,
Mayor Mare");
            Location incottage = new Location(LocationID.Cottage, "You are inside your home. You have put up some meager Hearthswarming decorations and you see critters sit all around the living room. There is a BEAR, some BIRDS, some CATERPILLARS, and of course, ANGEL BUNNY.");
            incottage.BeforeEnter = "You take a final look at the mare imprisoned in the moon, then shake your head and empty your mind of such unpleasant thoughts. You open the door. You need to make sure all of the animals are happy before you can begin celebrating Hearth's Warming Eve together.";
            Item gift = new Item(ItemID.Gift, "gift", "The GIFT is a wooden box engraved with the symbol of the Mare in the Moon. It bears a little note saying 'For Fluttershy from the town of Ponyville; we love you!'.");
            gift.AddCommand("open", () =>
             {
                 WriteLines(@"You open the wooden box left in front of your cottage by the Mayor. Inside is a beautiful silver bell with the engraved signatures of all members of the town council. Your cutie mark is displayed prominently on the top of the bell, just over the text, 'You are the treasure of our town.'. Aww, that's so sweet of them. They... they could have come themselves, though... it's nice to receive a gift but this will be another Hearth's Warming Eve you'll have to celebrate alone with your critters only.

Ah well, time's a-wasting. Angel is bound to be hungry by now. You should probably ENTER your COTTAGE.");
                 gift.PermittedCommands.Remove("open");
                 Item door = new Item(ItemID.CottageDoor, "cottage", "This is your home. It's rather large, considering that you live alone, and it could use some repairs.").AddCommand("enter", () =>
                 {
                     this.EnterLocation(incottage);
                 });
                 splash.Items.Add(door);
             });
            splash.Items.Add(gift);
            Locations.Add(LocationID.Splash, splash);

            Item inDoor = new Item("door", "Behind this door is Amethyst Star, Ponyville's star organizer.");
            inDoor.AddCommand("open", () =>
            {
                EnterLocation(locationAmethyst);
            });

            Item aniBear = new Item(ItemID.Bear, "Harry the bear", "Harry is restless and constantly looks over his shoulder.").AddCommand("talk to", () =>
            {
                if (!Flag_AngelFed)
                {
                    WriteLines("Harry is scared of something but he says you shouldn't worry about him now. First take care of Angel, he says.");
                    return;
                }
                if (!Flag_BirdsCured)
                {
                    WriteLines("Harry does not like something beyond the windows but he says it's probably nothing. First take care of the birds, he says.");
                    return;
                }
                WriteLines(@"""Something is approaching, dear Fluttershy,"" says the bear, ""and I don't like it one bit. I smell a pony approaching from the town, she's a friend but she's frightened. Something bad is happening, I just know it.""

No sooner does he finish talking that you hear several loud knocks on your door. The animals quickly scatter around the room. 

""Fluttershy,"" says a voice beyond the DOOR, ""it's me, Amethyst Star. I must speak with you, immediately!""");
                if (!Flag_HarryTalked)
                {
                    incottage.Items.Add(inDoor);
                    Item inAmethyst = new Item("Amethyst Star", "A pony is waiting outside your door. It would be polite to open.");
                    inAmethyst.AddCommand("open the door for", () =>
                    {
                        EnterLocation(locationAmethyst);
                    });
                    incottage.Items.Add(inAmethyst);
                    incottage.Description += " Amethyst Star is waiting behind the DOOR.";
                    Flag_HarryTalked = true;
                }


            });
            Item kWater = new Item("water", "The water is cool and refreshing.");
            Item kCarrots = new Item("carrots", "Angel likes these veggies the most.");
            Item kTea = new Item("tea", "A steaming hot tea, very useful in winter nights.");
            Item kAnalgetics = new Item("analgetics", "Pain medication, to be used when the Cutie Mark Crusaders try to sing all together.");
            Item aniCaterpillar = new Item(ItemID.Caterpillar, "caterpillars", "The caterpillars seem mostly happy. They don't need your attention now.");
            Item aniBirds = new Item(ItemID.Birds, "birds", "You notice one of the birds has trouble talking.");
            aniBirds.AddCommand("talk to", () =>
            {
                if (Flag_BirdsCured)
                {
                    WriteLines(@"The birds are happily chirping around the room.");
                    return;
                }
                WriteLines(@"""Fluttershy,"" says a bird, ""my friend here somehow lost her voice! One minute she was talking to me how she was singing in the Ponyville sauna, and how she'd never felt such dry air except when leaving for the southern lands, and the next, she begins to speak low and now she can't speak at all. She can't stay like this, you must help, Fluttershy, please!""

In the kitchen, you have WATER, CARROTS, TEA and ANALGETICS.");
                if (!Flag_BirdTalk)
                {
                    Flag_BirdTalk = true;
                    kWater.AddCommand("feed to sick bird:", () =>
                    {
                        Flag_BirdsCured = true;
                        aniBirds.Description = @"The birds are now happily chirping.";
                        WriteLines(@"The bird hastily drinks the cool water you bring her and immediately looks a lot healthier. ""Tha... thank... thank you, F-f-... Flutters,"" she manages to say. ""You are the kindest pony I know. I'm so much looking forward to this evening.""

The birds are now happy.");
                        
                        foreach (Item item in incottage.Items)
                        {
                            item.PermittedCommands.Remove("feed to sick bird:");
                        }
                    });
                    kTea.AddCommand("feed to sick bird:", () =>
                    {
                        WriteLines(@"The bird attempts to drink some of the hot tea but spits it out immediately and retreats. It appears she is now unable to touch anything hot.");
                    });
                    kCarrots.AddCommand("feed to sick bird:", () =>
                        {
                            WriteLines(@"""What are you doing, Fluttershy?"" asks the healthy bird, ""we don't really have teeth...""");
                        });
                    kAnalgetics.AddCommand("feed to sick bird:", () => {
                        WriteLines("The bird flies away when you approach with the medicine. She makes several wing gestures to show you what she thinks of it, you think what she's saying is: medicine, ingest, dance like crazy, drop dead.");
                    });
                }
                if (!Flag_Kitchen) {
                    Flag_Kitchen = true;
                    incottage.Items.AddRange(new[] { kWater, kCarrots, kTea, kAnalgetics });

                }
            });
            Item aniAngel = new Item(ItemID.Angel, "Angel Bunny", "Oh no, Angel looks angry. I should probably take care of him as soon as possible.");
            aniAngel.AddCommand("talk to", () =>
            {
                if (Flag_AngelFed)
                {
                    WriteLines("Angel is still eating his carrots. He appears to be very hungry today.");
                    return;
                }
                WriteLines(@"No wonder Angel is angry. You were outside for an entire hour without giving him a plate of his favourite food. He tries to stomp the ground, demanding your attention.

In the kitchen, you have WATER, CARROTS, TEA and ANALGETICS.");

                if (!Flag_AngelMet)
                {
                    Flag_AngelMet = true;
                    kWater.AddCommand("feed to Angel Bunny:", () =>
                    {
                        WriteLines("Angel takes the cup you offer him and thrusts it back into your face. When Angel heard your scream of surprise and saw your wet mane, he took a step back and mumbled an apology. But he still doesn't want your water.");
                    });
                    kAnalgetics.AddCommand("feed to Angel Bunny:", () =>
                    {
                        WriteLines(@"Angel sniffs the medicine you offer him and then shooks his head. ""You're not making me sleep this early, Flutters,"" he says, ""not on Hearth's Warming Eve.""");
                    });
                    kTea.AddCommand("feed to Angel Bunny:", () =>
                    {
                        WriteLines(@"Angel takes a sip of the tea and looks content, but he still requests his favourite food.");
                    });
                    kCarrots.AddCommand("feed to Angel Bunny:", () =>
                    {
                        aniAngel.Description = "Angel is now happily eating his carrots.";
                        WriteLines(@"Angel hurriedly takes all the carrots, retreats to a corner of the room and begins eating. He is now content.");
                        Flag_AngelFed = true;
                        foreach (Item item in incottage.Items)
                        {
                            item.PermittedCommands.Remove("feed to Angel Bunny:");
                        }
                    });
                }
                if (!Flag_Kitchen)
                {
                    Flag_Kitchen = true;
                    incottage.Items.AddRange(new[] { kWater, kCarrots, kTea, kAnalgetics });

                }
            });


            incottage.Items.AddRange(new[] { aniBear, aniCaterpillar, aniBirds, aniAngel });
        }



        // Utilized commands
        public List<string> UtilizedCommands = new List<string>();
        // Console output
        public List<string> ConsoleOutput = new List<string>();
        public int ConsoleTotalLines = 0;

        // Current state
        public Location CurrentLocation = null;
        public List<Item> Inventory = new List<Item>();
        public Dictionary<LocationID, Location> Locations = new Dictionary<LocationID, Location>();

        // State machine flags
        public bool Flag_SpellbookRead = false;
        private  bool Flag_BirdTalk;
        private  bool Flag_AngelFed;
        private bool Flag_Kitchen;
        private bool Flag_AngelMet;
        private bool Flag_BirdsCured;
        private bool Flag_Injured;
        private bool Flag_Wounded;
        private int EnergyLevel;
        private bool Flag_HarryTalked;

        public string GetLocationDescription(Location loc)
        {
            string output = loc.Description;
            if (loc.ID == LocationID.Puzzle)
            {
                return GetPuzzleLocationDescription();
            }
            return output;
        }
        public void EnterLocation(Location location, bool nograying = false)
        {
            CurrentLocation = location;
            if (location.BeforeEnter != null)
            {
                WriteLines(location.BeforeEnter + "\n", nograying);
                nograying = true;
            }
            if (location.OnEnter != null)
            {
                location.OnEnter();
            }
            string output = GetLocationDescription(CurrentLocation);
            WriteLines(output, nograying);
        }

        public void WriteLines(string lines, bool nograying = false)
        {
            List<string> news = Primitives.GetLines(lines, ImprovedGame.rectConsoleTopInner, Library.FontConsoleNormal);
            ConsoleTotalLines += news.Count;
            ConsoleOutput.AddRange(news);
            ImprovedGame.Main.LinesAdded(nograying);
        }



        public void GetAvailableCommands(out List<Command> availableCommands, out List<Command> recommendedCommands)
        {
            availableCommands = new List<Command>();
            recommendedCommands = new List<Command>();
            Location l = CurrentLocation;
           
            // Location-based
            List<Command> avails; List<Command> recomends;
            l.GetSpecialCommands(this, out avails, out recomends);
            availableCommands.AddRange(avails);

            // Location-based item-based
            foreach (Item item in l.Items)
            {
                List<string> permittedCommands = item.PermittedCommands.Keys.ToList();
                if (item.ID != ItemID.NonExaminable)
                {
                    permittedCommands.Add("examine");
                }
                if (item.ID == ItemID.Gate)
                {
                    if (yourPosition == item.Number)
                    {
                        Command c = new Command("go forwards through the", item);
                        availableCommands.Add(c);
                    }
                    else if (yourPosition == item.Number + 1)
                    {
                        Command c = new Command("go backwards through the", item);
                        availableCommands.Add(c);
                    }
                }
                foreach (string s in permittedCommands)
                {
                    Command c = new Command(s, item);
                    availableCommands.Add(c);
                }
            }

            // Save
            if (l.ID != LocationID.Splash)
            {
                availableCommands.Add(new Command("look around"));
            }
            // Restore
            try
            {
                string s = System.IO.File.ReadAllText(actFile);
                int n = Int32.Parse(s);
                for (int i = 2; i <= n; i++)
                {
                    availableCommands.Add(new Command("restore act " + i));
                }
            }
            catch
            {
            }
            availableCommands.Add(new Command("help"));
            availableCommands.Add(new Command("exit"));
            availableCommands.Add(new Command("toggle fullscreen"));
            availableCommands.Add(new Command("clear"));
            availableCommands.Add(new Command("feedback"));
        }



      
    }
    public enum FutureID
    {
        LockdownTime,
        CallInCelestia,
        EmpowerRadiance,
        ReturnHome,
        None
    }
}
