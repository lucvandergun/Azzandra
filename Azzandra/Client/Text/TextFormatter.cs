using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class TextFormatter
    {
        /// <summary>
        /// Draws a string with supplied text format at given position. (Referring method.)
        /// </summary>
        public static Tuple<Vector2, Color> DrawString(Vector2 pos, string str, TextFormat format, Color? startColor = null, Color? blendColor = null, float blendAmount = 1.0f)
        {
            return DrawString(pos, str, format.DefaultColor, format.Font, format.Alignment, format.IsShadow, startColor, blendColor, blendAmount);
        }


        /// <summary>
        /// Draws a string with supplied text format at given position.
        /// </summary>
        /// <returns>The new position and the resulting draw color.</returns>
        public static Tuple<Vector2, Color> DrawString(Vector2 pos, string str, Color defaultColor, SpriteFont font, Alignment alignment, bool isShadow = false, Color? startColor = null, Color? blendColor = null, float blendAmount = 1.0f)
        {
            var drawColor = startColor ?? defaultColor;

            // Apply initial color blend color if requested
            if (blendColor != null)
                drawColor = drawColor.BlendWith(blendColor.Value, blendAmount);

            // Separate string into sections
            var sections = FormatString(str);

            // Handle sections
            foreach (var s in sections)
            {
                if (IsFormatCode(s))
                {
                    // Apply code format (color)
                    var color = GetColor(s);

                    // Set or reset color value;
                    if (color == null) drawColor = defaultColor;
                    else drawColor = color.Value;

                    // Blend color if requested
                    if (blendColor != null)
                    {
                        drawColor = drawColor.BlendWith(blendColor.Value, blendAmount);
                    }
                }
                else
                {
                    switch (alignment)
                    {
                        default: Display.DrawString(pos, s, font, drawColor, isShadow); break;
                        case Alignment.Centered: Display.DrawStringCentered(pos, s, font, drawColor, isShadow); break;
                        case Alignment.VCentered: Display.DrawStringVCentered(pos, s, font, drawColor, isShadow); break;
                        case Alignment.VCenteredRight: Display.DrawStringVCenteredRight(pos, s, font, drawColor, isShadow); break;
                    }

                    pos.X += Util.GetStringWidth(s, font);
                } 
            }

            return Tuple.Create(pos, drawColor);
        }


        public static Tuple<Vector2, Color> DrawMultiLineString(Vector2 pos, string[] str, TextFormat format, Color? startColor = null, Color? blendColor = null, float blendAmount = 1.0f)
        {
            return DrawMultiLineString(pos, str, format.DefaultColor, format.Font, format.Alignment, format.IsShadow, startColor, blendColor, blendAmount);
        }
        public static Tuple<Vector2, Color> DrawMultiLineString(Vector2 pos, string[] str, Color defaultColor, SpriteFont font, Alignment alignment, bool isShadow = false, Color? startColor = null, Color? blendColor = null, float blendAmount = 1.0f)
        {
            if (str == null || str.Length <= 0) return null;
            pos.Y -= (str.Length - 1) * 16 / 2;
            foreach (var s in str)
            {
                var result = DrawString(pos, s, defaultColor, font, alignment, isShadow, startColor, blendColor, blendAmount);
                startColor = result.Item2;
                pos.Y += 16;
            }
            return Tuple.Create(pos, startColor.Value);
        }


        /// <summary>
        /// This method separates supplied string into segments of draw strings and formatint string codes.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] FormatString(string str)
        {
            if (str == null)
            {
                return new string[0];
            }

            var sections = new List<string>();
            bool isFormat = false;
            bool end = true;

            foreach (char c in str)
            {
                if (c == '<' && !isFormat)
                {
                    //start format section
                    sections.Add("<");
                    isFormat = true;
                    if (end) end = false;
                }
                else if (c == '>' && isFormat)
                {
                    //end format section
                    sections[sections.Count - 1] += c;
                    end = true;
                    isFormat = false;
                }
                else
                {
                    //for regular characters - add to current section
                    if (end)
                    {
                        end = false;
                        sections.Add("");
                    }

                    sections[sections.Count - 1] += c;
                }
            }

            return sections.ToArray();
        }

        public static bool IsFormatCode(string str)
        {
            //return (str != null) ? (str.First() == '<') ? true : false : false;
            return (str != null) && ((str.First() == '<'));
        }


        /// <summary>
        /// This method returns a Color based on supplied formatting code.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color? GetColor(string str)
        {
            if (str == null) return null;
            if (str.Length < 1) return null;

            //remove format signifiers
            var code = "";
            for (int i = 1; i < str.Length - 1; i++) code += str[i];

            switch (code)
            {
                default:
                case "r": return null;
                
                // Grays
                case "white": return Color.White;
                case "ltgray": return new Color(191, 191, 191);
                case "gray": return new Color(127, 127, 127);
                case "dkgray": return new Color(63, 63, 63);
                case "black": return Color.Black;

                // Blues
                case "aqua": return Color.Aqua;
                case "ltblue": return Color.LightBlue;
                case "blue": return Color.Blue;
                case "dkblue": return Color.DarkBlue;
                case "cyan": return Color.DarkCyan;
                case "medblue": return Color.CornflowerBlue;
                case "slate": return Color.LightSlateGray;
                case "dkslate": return Color.LightSlateGray.ChangeBrightness(-0.5f);
                case "azure": return Color.Azure;

                // Reds/yellows
                case "red": return Color.Red;
                case "dkred": case "maroon": return Color.Maroon;
                case "rose": return Color.Salmon;
                case "orange": return Color.Orange;
                case "dkorange": return Color.OrangeRed;
                case "yellow": return Color.Yellow;
                case "purple": return Color.Orchid;
                case "lavender": return Color.MediumPurple;
                case "chiffon": return Color.LemonChiffon;
                case "vred": return Color.MediumVioletRed;
                case "fuchsia": return Color.Fuchsia;

                // Greens
                case "acid": return Color.YellowGreen;
                case "green": return Color.Green;
                case "lime": return Color.Lime;
                case "spring": return Color.SpringGreen;
                case "dkgreen": return Color.DarkGreen;
                
                // Other
                case "beige": case "tan": return Color.Wheat; 
                case "brown": return Color.Sienna;
                case "gold": return Color.Goldenrod;
                case "azgreen": return new Color(43, 104, 69);
                case "azyellow": return new Color(255, 216, 0);

            }
        }

        public static string GetValueRatioColor(int v1, int v2)
        {
            return v1 <= v2 / 6 ? "<red>"
                : v1 <= v2 / 3 ? "<orange>"
                : v1 >= v2 - v2 / 3 ? "<lime>"
                : "<yellow>";
        }


        //public static string[] SplitString(string msg, int maxLength, SpriteFont font)
        //{
        //    if (msg == null) return null;
            
        //    // Split string if drawn string length exceeds maxLength
        //    var segments = FormatString(msg);
        //    string lastFormat = null;

        //    int currentLength = 0;
        //    var lines = new List<string>();
        //    lines.Add("");
        //    foreach (var segment in segments)
        //    {
        //        if (segment?.Length > 1)
        //        {
        //            if (segment.First() == '<')
        //            {
        //                lastFormat = segment;
        //                lines[lines.Count - 1] += segment;
        //            }
        //            else
        //            {
        //                while (true)
        //                {
        //                    var splits = Util.SeparateString(segment, font, maxLength - currentLength);
        //                    if (splits?.Length > 0)
        //                    {
        //                        // Add initial split to current line.
        //                        lines[lines.Count - 1] += splits[0];
        //                    }
                            
        //                }
                        
        //                foreach (var split in splits)
        //                {
        //                    lines[lines.Count - 1]
        //                }
        //            }
        //        }
                    

        //        // paste last formatting code to the front of a non-starting line.
        //        var newLine = line;
        //        if (lastFormat != null)
        //            newLine = lastFormat + newLine;

        //        if (lines.Length > 1)
        //            lastFormat = TextFormatter.FormatString(newLine).FirstOrDefault(f => f.First() == '<');
        //    }
    }
}
