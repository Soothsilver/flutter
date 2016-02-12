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
    public class Listbox<T> : UIElement
    {
    
        public event Action<Listbox<T>, object> ItemSelected;
        protected virtual void OnItemSelected(Listbox<T> listbox, object item)
        {
            if (ItemSelected != null)
                ItemSelected(listbox, item);
        }
        public event Action<Listbox<T>, object> ItemConfirmed;
        protected virtual void OnItemConfirmed(Listbox<T> listbox, object item)
        {
            if (ItemConfirmed != null)
                ItemConfirmed(listbox, item);
        }
        public List<T> Items = new List<T>();
        private int _selectedIndex = -1;
        private int MouseOverIndex = -1;
        private int _topOfList = 0;
        public int SelectedIndex
        {
            get { if (_selectedIndex >= Items.Count) return -1; else return _selectedIndex; }
            set
            {
                if (value >= Items.Count || value < -1)
                    _selectedIndex = -1;
                else
                {
                    _selectedIndex = value;
                    OnItemSelected(this, SelectedItem);
                }
            }
        }
        public object SelectedItem
        {
            get { if (_selectedIndex == -1) return null; else return Items[_selectedIndex]; }
            set
            {
                T input;
                if (value is T)
                {
                    input = (T)value;
                    if (Items.Contains(input))
                    {
                        SelectedIndex = Items.IndexOf(input);
                    }
                    else throw new Exception("This item is not in the listbox.");
                }
                else throw new Exception("This item is not of the type accepted by this Listbox<T>.");
            }
        }
        public override void Update()
        {
            if (Root.WasMouseLeftClick)
            {
                if (Root.IsMouseOver(Rectangle))
                {
                    Root.ConsumeLeftClick();
                    if (MouseOverIndex != -1)
                    {
                        if (MouseOverIndex == SelectedIndex)
                            OnItemConfirmed(this, SelectedItem);
                        else
                        {
                            SelectedIndex = MouseOverIndex;
                        }
                    }
                    else SelectedIndex = -1;
                }
            }
            if (Root.WasKeyPressed(Keys.Down))
                if (SelectedIndex < Items.Count - 1) SelectedIndex++;
            if (Root.WasKeyPressed(Keys.Up))
                if (SelectedIndex > 0) SelectedIndex--;
            if (Root.WasKeyPressed(Keys.Home))
                if (Items.Count > 0) SelectedIndex = 0;
            if (Root.WasKeyPressed(Keys.End))
                SelectedIndex = Items.Count - 1;
            if (Root.WasKeyPressed(Keys.Enter))
            {
                if (SelectedIndex != -1) OnItemConfirmed(this, SelectedItem);
            }
            base.Update();
        }

        public override void Draw()
        {
            MouseOverIndex = -1;
            Color outerBorderColor = Skin.OuterBorderColor;
            Color innerBorderColor = Skin.InnerBorderColor;
            Color innerButtonColor = Skin.WhiteBackgroundColor;
            Color textColor = Skin.TextColor;
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            //  Primitives.DrawMultiLineText(txt + (Root.SecondsSinceStartInt % 2 == 0 ? "|" : ""), new Rectangle(InnerRectangle.X + 8, InnerRectangle.Y + 3, InnerRectangle.Width - 10, InnerRectangle.Height - 4), textColor, Skin.Font, Primitives.TextAlignment.TopLeft);
            for (int i = _topOfList; i < Items.Count; i++)
            {
                Rectangle rectItem = new Rectangle(InnerRectangle.X + 1, InnerRectangle.Y + Skin.ListItemHeight * (i - _topOfList) + 1, InnerRectangle.Width - 2, Skin.ListItemHeight);
                if (Root.IsMouseOver(rectItem))
                    MouseOverIndex = i;

                if (_selectedIndex == i)
                    Primitives.FillRectangle(rectItem, Skin.ItemSelectedBackgroundColor);
                else if (MouseOverIndex == i)
                    Primitives.FillRectangle(rectItem, Skin.ItemMouseOverBackgroundColor);
                Primitives.DrawSingleLineText(Items[i].ToString(), new Vector2(InnerRectangle.X + 5, InnerRectangle.Y + 2 + Skin.ListItemHeight * (i - _topOfList)), Skin.TextColor, Skin.Font);
                Primitives.DrawLine(new Vector2(InnerRectangle.X, InnerRectangle.Y + Skin.ListItemHeight * (i - _topOfList + 1)),
                                    new Vector2(InnerRectangle.Right, InnerRectangle.Y + Skin.ListItemHeight * (i - _topOfList + 1)),
                                    outerBorderColor, Skin.OuterBorderThickness);
            }
        }
        public Listbox(Rectangle rect)
        {
            Rectangle = rect;
        }
    }
}