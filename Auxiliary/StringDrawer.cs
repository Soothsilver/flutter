using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Specialized;

namespace Auxiliary
{
    public static partial class Primitives
    {
        private static List<MultilineString> MultilineStringCache = new List<MultilineString>();
        public static void DrawMultiLineText(string text, Rectangle rect, Color color, SpriteFont font = null, TextAlignment alignment = TextAlignment.TopLeft)
        {
            if (font == null) font = Library.FontConsoleNormal;
            Rectangle empty = Rectangle.Empty;
            MultilineString ms = new MultilineString(text, rect, alignment, font, Vector2.Zero, true, null);
            foreach (MultilineString msCache in MultilineStringCache)
            {
                if (ms.Equals(msCache))
                {
                    foreach (MultilineLine line in msCache.CachedLines)
                    {
                        SpriteBatch.DrawString(font, line.Text, line.Position, color);
                    }
                    return;
                }
            }
                List<MultilineLine> cachedLines = new List<MultilineLine>();
                DrawMultiLineTextDetailedParameters(SpriteBatch, font, text, rect, color, alignment, true, new Vector2(0, 0), out empty, out cachedLines);
                MultilineStringCache.Add(new MultilineString(text, rect, alignment, font, Vector2.Zero, true, cachedLines));
            
        }
        public static Rectangle GetMultiLineTextBounds(string text, Rectangle rect, SpriteFont font = null)
        {
            if (font == null) font = Library.FontConsoleNormal;
            Rectangle bounds = Rectangle.Empty;
            List<MultilineLine> empty;
            DrawMultiLineTextDetailedParameters(SpriteBatch, font, text, rect, Color.Black, TextAlignment.TopLeft, true, new Vector2(0, 0), out bounds, out empty, onlyGetBounds: true);
            return bounds;
        }
        public enum TextAlignment
        {
            Top,
            Left,
            Middle,
            Right,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public static List<string> GetLines(string text, Rectangle r, SpriteFont fnt)
        {
            StringCollection lines = new StringCollection();
            lines.AddRange(text.Split(new string[] { "\\n", Environment.NewLine, "\n" }, StringSplitOptions.None));

            // calc the size of the rect for all the text
            Rectangle tmprect = ProcessLines(fnt, r, true, lines);
            List<string> outp = new List<string>();
            foreach (string s in lines)
            {
                outp.Add(s);
            }
            return outp;
        }

        /// <summary>
        /// Draws a multi-string. 
        /// WARNING! This grows more CPU-intensive as the number of words grow (only if word wrap enabled).
        /// </summary>
        /// <param name="sb">A reference to a SpriteBatch object that will draw the text.</param>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>

        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="performWordWrap">If true the words within the text will be aranged to rey and
        /// fit within the bounds of the specified screen rectangle.</param>
        /// <param name="offsett">Draws the text at a specified offset relative to the screen
        /// rectangles top left position. </param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bouds of
        /// the text that was drawn.</param>
        public static void DrawMultiLineTextDetailedParameters(SpriteBatch sb, SpriteFont fnt, string text, Rectangle r,
        Color col, TextAlignment align, bool performWordWrap, Vector2 offsett, out Rectangle textBounds, out List<MultilineLine> cachedLines, bool onlyGetBounds = false)
        {
            // check if there is text to draw
            textBounds = r;
            cachedLines = new List<MultilineLine>();
            if (text == null) return;
            if (text == string.Empty) return;

            StringCollection lines = new StringCollection();
            lines.AddRange(text.Split(new string[] { "\\n", Environment.NewLine, "\n" }, StringSplitOptions.None));

            // calc the size of the rect for all the text
            Rectangle tmprect = ProcessLines(fnt, r, performWordWrap, lines);
            if (onlyGetBounds) { textBounds = tmprect; return; }

            // setup the position where drawing will start
            Vector2 pos = new Vector2(r.X, r.Y);
            int aStyle = 0;

            switch (align)
            {
                case TextAlignment.Bottom:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 1;
                    break;
                case TextAlignment.BottomLeft:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 0;
                    break;
                case TextAlignment.BottomRight:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 2;
                    break;
                case TextAlignment.Left:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 0;
                    break;
                case TextAlignment.Middle:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 1;
                    break;
                case TextAlignment.Right:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 2;
                    break;
                case TextAlignment.Top:
                    aStyle = 1;
                    break;
                case TextAlignment.TopLeft:
                    aStyle = 0;
                    break;
                case TextAlignment.TopRight:
                    aStyle = 2;
                    break;
            }

            // draw text
            for (int idx = 0; idx < lines.Count; idx++)
            {
                string txt = lines[idx];
                Vector2 size = fnt.MeasureString(txt);
                switch (aStyle)
                {
                    case 0:
                        pos.X = r.X;
                        break;
                    case 1:
                        pos.X = r.X + ((r.Width / 2) - (size.X / 2));
                        break;
                    case 2:
                        pos.X = r.Right - size.X;
                        break;
                }
                if (pos.Y + fnt.LineSpacing > r.Y + r.Height) { break; }
                // draw the line of text
                pos = new Vector2((int)pos.X, (int)pos.Y);
                sb.DrawString(fnt, txt, pos + offsett, col);
                cachedLines.Add(new MultilineLine(txt, pos + offsett));
                pos.Y += fnt.LineSpacing;
                
            }

            textBounds = tmprect;
        }

        internal static Rectangle ProcessLines(SpriteFont fnt, Rectangle r, bool performWordWrap, StringCollection lines)
        {
            // loop through each line in the collection
            Rectangle bounds = r;
            bounds.Width = 0;
            bounds.Height = 0;
            int index = 0;
            float Width = 0;
            bool lineInserted = false;
            while (index < lines.Count)
            {
                // get a line of text
                string linetext = lines[index];
                //measure the line of text
                Vector2 size = fnt.MeasureString(linetext);

                // check if the line of text is geater then then the rect we want to draw it inside of
                if (performWordWrap && size.X > r.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (linetext.EndsWith(" "))
                    {
                        endspace = " ";
                        linetext = linetext.TrimEnd();
                    }

                    // get the index of the last space character
                    int i = linetext.LastIndexOf(" ");
                    if (i != -1)
                    {

                        // if there was a space grab the last word in the line
                        string lastword = linetext.Substring(i + 1);
                        // move word to next line
                        if (index == lines.Count - 1)
                        {
                            lines.Add(lastword);
                            lineInserted = true;
                        }
                        else
                        {
                            // prepend last word to begining of next line
                            if (lineInserted)
                            {
                                lines[index + 1] = lastword + endspace + lines[index + 1];
                            }
                            else
                            {
                                lines.Insert(index + 1, lastword);
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed

                        lines[index] = linetext.Substring(0, i + 1);

                    }
                    else
                    {

                        // there appear to be no space characters on this line s move to the next line

                        lineInserted = false;
                        size = fnt.MeasureString(lines[index]);
                        if (size.X > bounds.Width) Width = size.X;
                        bounds.Height += fnt.LineSpacing;// size.Y - 1;
                        index++;
                    }
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;

                    size = fnt.MeasureString(lines[index]);
                    if (size.X > bounds.Width) bounds.Width = (int)size.X;
                    bounds.Height += fnt.LineSpacing;//size.Y - 1;
                    index++;
                }
            }

            // returns the size of the text
            return bounds;
        }


        public class MultilineLine
        {
            public string Text;
            public Vector2 Position;
            public MultilineLine(string text, Vector2 position)
            {
                Text = text;
                Position = position;
            }
        }
        private class MultilineString
        {
            public string Text;
            public Rectangle Rectangle;
            public TextAlignment TextAlignment;
            public SpriteFont Font;
            public Vector2 Offset;
            public bool WordWrap;
            public List<MultilineLine> CachedLines;
            
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                MultilineString ms = (MultilineString)obj;
                return Offset == ms.Offset && WordWrap == ms.WordWrap && Text == ms.Text && Rectangle == ms.Rectangle && TextAlignment == ms.TextAlignment && Font == ms.Font;
            }

            public override int GetHashCode()
            {
                return (Text.GetHashCode() + Rectangle.GetHashCode() + TextAlignment.GetHashCode() + Font.GetHashCode() + Offset.GetHashCode() + WordWrap.GetHashCode());
            }
            public MultilineString(string text, Rectangle rect, TextAlignment alignment, SpriteFont font, Vector2 offset, bool wordWrap, List<MultilineLine> cachedLines)
            {
                CachedLines = cachedLines;
                Text = text;
                Rectangle = rect;
                TextAlignment = alignment;
                Font = font;
                Offset = offset;
                WordWrap = wordWrap;
            }
        }
    }
}
