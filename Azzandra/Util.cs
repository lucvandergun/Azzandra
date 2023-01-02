using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class Util
    {
        public static Random Random { get; private set; }
        public static void NewRandom(int seed)
        {
            Random = new Random(seed);
        }

        public static int NextUpperHalf(int n, Random random = null)
        {
            var r = random ?? Random;
            return r.Next((int)Math.Floor(n / 2d) + 1) + (int)Math.Ceiling(n / 2d);
        }

        /// <summary>
        /// Boundarize (i.e. max & min) by lower and upper (both included) bounds.
        /// </summary>
        public static int Boundarize(this int n, int lower, int upper)
        {
            return Math.Max(lower, Math.Min(n, upper));
        }

        public static int Round(this float n)
        {
            return (int)Math.Round(n, MidpointRounding.AwayFromZero);
        }

        //public static rdg.Dir Opposite(this rdg.Dir dir) => (rdg.Dir)(((int)dir + 2) % 4);

        /// <summary>
        /// Shuffles the list in-place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="random"></param>
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            if (random == null) random = Random;
            
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T PickRandom<T>(this IList<T> list, Random random = null)
        {
            if (random == null) random = Random;
            int c = list == null ? 0 : list.Count;

            return c == 0
                ? default(T)
                : list[random.Next(c)];
        }

        public static T PickAndRemoveRandom<T>(this IList<T> list, Random random = null)
        {
            if (random == null) random = Random;
            int c = list == null ? 0 : list.Count;
            if (c == 0) return default(T);

            int n = random.Next(c);
            T item = list[n];

            list.RemoveAt(n);
            return item;
        }

        public static string CapFirst(this string input)
        {
            switch (input)
            {
                case null:  return "Null";// throw new ArgumentNullException(nameof(input)); //
                case "": return input;
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        /// <summary>
        /// Creates a string representation of format: "3k", "4m" or "1b".
        /// </summary>
        /// <param name="num">The integer to convert.</param>
        /// <returns></returns>
        public static string StringifyNumber(this int num)
        {
            if (num >= 1000000000)
            {
                return (num / 1000000000).ToString() + "b";
            }
            else if (num >= 1000000)
            {
                return (num / 1000000).ToString() + "m";
            }
            else if (num >= 10000)
            {
                return (num / 1000).ToString() + "k";
            }
            else return num.ToString();
        }

        /// <summary>
        /// Creates a string representation of format: "3k", "4m" or "1b".
        /// </summary>
        /// <param name="num">The unsigned integer to convert.</param>
        /// <returns></returns>
        public static string StringifyNumber(this uint num)
        {
            if (num >= 1000000000)
            {
                return (num / 1000000000).ToString() + "b";
            }
            else if (num >= 1000000)
            {
                return (num / 1000000).ToString() + "m";
            }
            else if (num >= 10000)
            {
                return (num / 1000).ToString() + "k";
            }
            else return num.ToString();
        }

        /// <summary>
        /// Creates a string representation of format: "10.000". It places a "." every 3 numbers from the left.
        /// </summary>
        /// <param name="num">The integer to convert.</param>
        /// <returns></returns>
        public static string StringifyNumber2(this int num)
        {
            return num.ToString("N0");
        }

        /// <summary>
        /// Creates a string representation of format: "10.000". It places a "." every 3 numbers from the left.
        /// </summary>
        /// <param name="num">The unsigned integer to convert.</param>
        /// <returns></returns>
        public static string StringifyNumber2(this uint num)
        {
            return num.ToString("N0");
        }

        /// <summary>
        /// Creates a roman numeral string representation of number.
        /// Numers below 0 will be "<0" and above 4000 "MMMM+".
        /// </summary>
        /// <param name="num">The integer to convert.</param>
        /// <returns></returns>
        public static string ToRoman(this int number)
        {
            if (number < 0) return "<0";
            if (number > 4000) return "MMMM+";

            if (number == 0) return "";
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);

            return "Error";
        }

        /// <summary>
        /// Converts integer to roman numeral string. Returns nothing if lower than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToRomanHideOne(this int number)
        {
            if (number <= 1) return string.Empty;
            else return ToRoman(number);
        }


        public static string GetSignString(this int n)
        {
            return n >= 0 ? "+" + n : n.ToString();
        }


        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }
        public static void Populate<T>(this T[,] arr, T value)
        {
            if (arr == null)
                return;
            
            int width = arr.GetLength(0);
            int height = arr.GetLength(1);

            for (int i, j = 0; j < height; j++)
            {
                for (i = 0; i < width; i++)
                {
                    arr[i, j] = value;
                }
            }
        }

        public static T[] CreateCopy<T>(this T[] arr)
        {
            T[] newArr = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = arr[i];
            }

            return newArr;
        }

        public static T[,] CreateCopy<T>(this T[,] arr)
        {
            int w, h;
            w = arr.GetLength(0);
            h = arr.GetLength(1);
            T[,] newArr = new T[w, h];

            for (int i, j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                {
                    newArr[i, j] = arr[i, j];
                }
            }

            return newArr;
        }

        public static List<T> CreateCopy<T>(this List<T> list)
        {
            List<T> newList = new List<T>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(list[i]);
            }

            return newList;
        }

        /// <summary>
        /// Adds either 'a' or 'an' in front of the string, depending on correct grammar.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddArticle(string str)
        {
            if (str == null) return "";
            else if (str == "") return "";

            char first = str.First().ToString().ToLower().First();
            if (first == 'i' || first == 'e' || first == 'a' || first == 'o' || first == 'u')
            {
                return "an " + str;
            }
            else
            {
                return "a " + str;
            }
        }

        public static Keys IntToKey(int n)
        {
            if (n < 1 || n > 9) return Keys.D0;
            else return (Keys)(n + 48);
        }

        public static Keys IntToFKey(int n)
        {
            if (n < 1 || n > 12) return Keys.Escape;
            else return (Keys)(n + 111);
        }

        public static int GetStringWidth(string str, SpriteFont font)
        {
            return (int)(font.MeasureString(str).X);
        }

        public static int GetStringHeight(string str, SpriteFont font)
        {
            return (int)(font.MeasureString(str).Y);
        }

        public static int GetFontHeight(SpriteFont font)
        {
            return (int)(font.MeasureString("A").Y - 1);
        }

        public static int GetMaxStringWidth(string[] text, SpriteFont font)
        {
            if (text == null) return 0;
            else if (text.Length == 0) return 0;

            int width = 0;
            foreach (var str in text)
            {
                var w = GetStringWidth(str, font);
                if (w > width) width = w;
            }

            return width;
        }

        public static int GetMaxStringWidth(List<string> text, SpriteFont font)
        {
            if (text == null) return 0;
            else if (text.Count == 0) return 0;

            int width = 0;
            foreach (var str in text)
            {
                var w = GetStringWidth(str, font);
                if (w > width) width = w;
            }

            return width;
        }

        public static string[] SplitWords(string str)
        {
            //This method removes and splits words at every space.
            //Excess spaces are removed.

            var words = new List<string>();
            bool end = true;

            foreach (char c in str)
            {
                if (c == ' ')
                {
                    if (end != true) end = true;
                }
                else
                {
                    if (end)
                    {
                        words.Add("");
                        end = false;
                    }

                    words[words.Count - 1] += c;
                }
            }

            return words.ToArray();
        }

        /// <summary>
        /// Changes the brightness of a color, where 1.0f is fullly white, and -1.0f is fully black.
        /// Alpha channels remain unchanged.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="correctionFactor"></param>
        /// <returns></returns>
        public static Color ChangeBrightness(this Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return new Color((int)red, (int)green, (int)blue, color.A);
        }

        public static string CreateStatsString(List<string> stats)
        {
            if (stats == null || stats.Count <= 0)
                return null;

            var str = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (i != 0) str += "\n";
                str += stats[i];
            }

            return str; ;
        }

        /// <summary>
        /// Separates the string at the maximum length, or any \n character.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="font">The font to use when comparing length.</param>
        /// <param name="maxLength">The maximum length in pixels to split at.</param>
        /// <returns>A string array of all split parts.</returns>
        public static string[] SeparateString(string str, SpriteFont font, int maxLength)
        {
            // Nullcheck
            if (string.IsNullOrEmpty(str))
                return new string[0];
            
            // Create list of lines
            var lines = new List<string>();
             
            string tempStr = "";
            int tempStrLength = 0;
            int lastSpace = -1;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == ' ') lastSpace = i;
                tempStr += c;
                tempStrLength += Util.GetStringWidth(c.ToString(), font);

                // Add new line if conditions are met
                if (tempStrLength > maxLength || c == '\n')
                {
                    if (c == '\n') // Cut at line break
                    {
                        tempStr = tempStr.Remove(i);
                        str = str.Remove(0, i + 1);
                    }
                    else if (lastSpace != -1)  // Cut at last space
                    {
                        tempStr = tempStr.Remove(lastSpace);
                        str = str.Remove(0, lastSpace + 1);
                    }
                    else // Cut at current position
                    {
                        tempStr = tempStr.Remove(i);
                        str = str.Remove(0, i);
                    }

                    // Add temporary string to lines
                    lines.Add(tempStr);

                    // Cut the total string to new start & reset the for-loop
                    tempStr = "";
                    tempStrLength = 0;
                    i = -1;
                }
            }

            // Add the final line
            lines.Add(str);

            return lines.ToArray();
        }

        public static Vector ToInt(this Vector2 v)
        {
            return new Vector((int)v.X, (int)v.Y);
        }

        public static int ClosestToZero(int a, int b)
        {
            return Math.Abs(a) <= Math.Abs(b) ? a : b;
        }

        public static int FurthestFromZero(int a, int b)
        {
            return Math.Abs(a) > Math.Abs(b) ? a : b;
        }

        /// <summary>
        /// Computes a list of absolute nodes that occupy the line space between two points.
        /// </summary>
        /// <param name="start">The starting node.</param>
        /// <param name="end">The ending node.</param>
        /// <param name="includeStart">Whether to include the start.</param>
        /// <param name="includeEnd">Whether to include the end.</param>
        /// <returns></returns>
        public static IEnumerable<Vector> CastRay(this Vector start, Vector end, bool includeStart, bool includeEnd)
        {
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;

            int N = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float divN = (N == 0) ? 0.0f : 1.0f / N;
            float xstep = dx * divN;
            float ystep = dy * divN;
            float x = start.X;
            float y = start.Y;

            int s = includeStart ? 0 : 1;
            int e = includeEnd ? N : N - 1;
            for (int step = 0; step <= N; step++, x += xstep, y += ystep)
            {
                var v = new Vector((int)Math.Round(x), (int)Math.Round(y));
                if ((v != start || includeStart) && (v != end || includeEnd))
                    yield return v;
            }
        }

        /// <summary>
        /// Computes a list of absolute nodes that occupy the line space between two points.
        /// </summary>
        /// <param name="start">The starting node.</param>
        /// <param name="end">The ending node.</param>
        /// <param name="includeStart">Whether to include the start.</param>
        /// <param name="includeEnd">Whether to include the end.</param>
        /// <returns></returns>
        public static IEnumerable<Vector> CastRay(IEnumerable<Vector> origin, IEnumerable<Vector> target, bool includeStart, bool includeEnd)
        {
            var closestNodes = origin.GetClosestNodes(target);
            var list = CastRay(closestNodes.Item1, closestNodes.Item2, false, false);
            if (includeStart)
                list = list.Concat(origin);
            if (includeEnd)
                list = list.Concat(target);
            // These last concatenations dont seem to be happening...
            return list;
        }

        public static Tuple<Vector, Vector> GetClosestNodes(this IEnumerable<Vector> origin, IEnumerable<Vector> target)
        {
            // For all tiles in 1 to all tiles in 2, select min distance:
            Tuple<Vector, Vector> closest = null;

            foreach (var t1 in origin)
            {
                foreach (var t2 in target)
                {
                    if (closest == null || (t2 - t1).ChebyshevLength() < (closest.Item2 - closest.Item2).ChebyshevLength())
                        closest = Tuple.Create(t1, t2);
                }
            }

            return closest;
        }

        //public static List<Vector> CastRayFull(this Vector start, Vector end, bool includeStart, bool includeEnd)
        //{
        //    var dx = end.X - start.X;
        //    var dy = end.Y - start.Y;

        //    var nx = Math.Abs(dx);
        //    var ny = Math.Abs(dy);
        //    var sign_x = dx > 0 ? 1 : -1;
        //    var sign_y = dy > 0 ? 1 : -1;

        //    var p = start;
        //    var points = new List<Vector>(nx + ny);

        //    //add start
        //    if (includeStart) points.Add(p);

        //    for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        //    {
        //        // if the current point + 0.5 is exactly on the line:
        //        if ((0.5 + ix) / nx == (0.5 + iy) / ny)
        //        {
        //            // next step is diagonal
        //            points.Add(new Vector(p.X + sign_x, p.Y));
        //            points.Add(new Vector(p.X, p.Y + sign_y));

        //            p.X += sign_x;
        //            p.Y += sign_y;
        //            ix++;
        //            iy++;
        //        }

        //        // if the current point + 0.5 is more oriented towards y
        //        else if ((0.5 + ix) / nx < (0.5 + iy) / ny)
        //        {
        //            // next step is horizontal
        //            p.X += sign_x;
        //            ix++;
        //        }
        //        else
        //        {
        //            // next step is vertical
        //            p.Y += sign_y;
        //            iy++;
        //        }

        //        if (p != end || includeEnd)
        //            points.Add(new Vector(p.X, p.Y));
        //    }
        //    return points;
        //}

        /// <summary>
        /// Creates a string representation of format: "[a, b, c, ]".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Stringify<T>(this IEnumerable<T> list, Func<T, string> func = null)
        {
            if (list == null)
                return "null";
            
            var str = "[";
            foreach (var i in list)
            {
                if (func == null)
                    str += i;
                else
                    str += func(i);
                str += ", ";
            }

            return str + "]";
        }

        /// <summary>
        /// Creates a string representation of format: "a, b and c".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Stringify2<T>(this IEnumerable<T> list, Func<T, string> func = null)
        {
            if (list == null)
                return "None";

            var str = "";
            int index = 0, amt = list.Count();
            foreach (var i in list)
            {
                if (func == null)
                    str += i;
                else
                    str += func(i);

                index++;
                str += index < amt - 1 ? ", " : index == amt - 1 ? " and " : "";
            }

            return str;
        }

        /// <summary>
        /// Creates a string representation of format: "a b c".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Stringify3<T>(this IEnumerable<T> list, Func<T, string> func = null)
        {
            if (list == null)
                return "None";

            var str = "";
            int index = 0, amt = list.Count();
            foreach (var i in list)
            {
                if (func == null)
                    str += i;
                else
                    str += func(i);

                index++;
                if (index < amt) str += " ";
            }

            return str;
        }

        public static string Stringify<T>(this T[,] grid)
        {
            if (grid == null)
                return "null";

            int h = grid.GetLength(0);
            int w = grid.GetLength(1);
            
            var str = "";
            for (int i, j = 0; j < h; j++)
            {
                str += "[";
                for (i = 0; i < w; i++) str += grid[i, j] + ", ";
                str += "]";
                if (j != h - 1)
                    str += "\n";
            }

            return str + "";
        }

        public static bool RollAgainst(this int a, int b)
        {
            var negativeValueOffset = (a < 0 || b < 0) ? -Math.Min(a, b) : 0;

            
            return Random.Next(a + b + negativeValueOffset) - negativeValueOffset < a;
        }

        public static Color BlendWith(this Color baseColor, Color blendColor, float amount)
        {
            baseColor.Deconstruct(out float r1, out float g1, out float b1);
            blendColor.Deconstruct(out float r2, out float g2, out float b2);

            return new Color
                ((int)r1.LerpTo(r2, amount),
                (int)g1.LerpTo(g2, amount),
                (int)b1.LerpTo(b2, amount));
        }

        /// <summary>
        /// Pick the float svalue that is exactly at the "amount" ratio between the "min" and "max" values
        /// </summary>
        /// <param name="min">Float value between 0 and 1.0</param>
        /// <param name="max">Float value between 0 and 1.0</param>
        /// <param name="amount">Float value between 0 and 1.0</param>
        /// <returns></returns>
        public static float LerpTo(this float min, float max, float amount)
        {
            return min + (max - min) * amount;
        }

        /// <summary>
        /// This function picks an index based on its value/weight and a random roll value
        /// </summary>
        public static int PickItemByWeight(List<int> list, Random random)
        {
            if (list != null && list.Count > 0)
            {
                // Add up weights to a total
                int tableWeight = 0;
                foreach (var item in list)
                    tableWeight += item;

                // Roll a weight index
                int roll = random.Next(tableWeight);

                // Find that index' drop
                for (int i = 0, j = 0; i < list.Count; i++)
                {
                    j += list[i];
                    if (j > roll)
                        return i;
                }
            }

            return -1;
        }

        ///// <summary>
        ///// This function picks an item in the list based on its weight and a random roll value
        ///// </summary>
        //public static T PickItemByWeight<T>(List<Tuple<int, T>> list, Random random)
        //{
        //    if (list != null && list.Count > 0)
        //    {
        //        // Add up weights to a total
        //        int tableWeight = 0;
        //        foreach (var item in list)
        //            tableWeight += item.Item1;

        //        // Roll a weight index
        //        int roll = random.Next(tableWeight);

        //        // Find that index' drop
        //        for (int i = 0, j = 0; i < list.Count; i++)
        //        {
        //            j += list[i].Item1;
        //            if (j > roll)
        //                return list[i].Item2;
        //        }
        //    }

        //    return default(T);
        //}

        /// <summary>
        /// This function picks an item in the list based on its weight and a random roll value
        /// </summary>
        public static T PickItemByWeight<T>(IEnumerable<Tuple<int, T>> list, Random random)
        {
            if (random == null)
                random = Random;
            
            if (list != null)
            {
                // Add up weights to a total
                int totalWeight = 0;
                foreach (var item in list)
                    totalWeight += item.Item1;

                // Roll a random value of total weight
                int roll = random.Next(totalWeight);

                // Find the corresponding drop
                int j = 0;
                foreach (var item in list)
                {
                    j += item.Item1;
                    if (j > roll)
                        return item.Item2;
                }
            }

            return default;
        }

        /// <summary>
        /// This function picks an item in the list based on its weight and a random roll value
        /// </summary>
        public static T PickItemByWeight<T>(IEnumerable<Tuple<float, T>> list, Random random)
        {
            if (random == null)
                random = Random;

            if (list != null)
            {
                // Add up weights to a total
                float totalWeight = 0f;
                foreach (var item in list)
                    totalWeight += item.Item1;

                // Roll a random value of total weight
                float roll = (float)random.NextDouble() * totalWeight;

                // Find the corresponding drop
                float j = 0;
                foreach (var item in list)
                {
                    j += item.Item1;
                    if (j > roll)
                        return item.Item2;
                }
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when successfully inserted. false when subbuffer exceeded main buffer bounds</returns>
        public static bool Insert(this byte[] buffer, int pos, byte[] bytes)
        {
            // Buffer overflow protection:
            if (pos + bytes.Length - 1 > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            var length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                buffer[pos + i] = bytes[i];
            }
            return true;
        }


        public static string ToUnderscore(this string name)
        {
            // replace capitals with '_' + lowercase character equivalent.
            var underscored = string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
            return underscored.ToLower();
        }

        public static string ToCamelCase(this string name)
        {
            // Convert id to TitleCase
            name = name.CapFirst();
            int length = name.Length;
            for (int i = 0; i < length - 1; i++)
            {
                var c = name[i];
                if (c == '_')
                {
                    name = name.Substring(0, i) + name[i + 1].ToString().ToUpper() + name.Substring(i + 2);
                    length--;
                }
            }

            return name;
        }

        public static List<Vector> SortAround(this List<Vector> list, Vector center)
        {
            var newList = list.CreateCopy();
            newList.Sort(comparison: (n1, n2) => (n1 - center).ChebyshevLength() - (n2 - center).ChebyshevLength());
            return newList;
        }

        /// <summary>
        /// Creates a new list with all inner sums of the items of the original list.
        /// </summary>
        public static List<Vector> InnerSum(this IEnumerable<Vector> list)
        {
            var ip = new List<Vector>();
            foreach (var a in list)
                foreach (var b in list)
                    ip.Add(a + b);
            return ip;
        }

        /// <summary>
        /// Method to remove predicate matches from the given list reference (in-list!).
        /// Perhaps more inefficient this way, but is in-list.
        /// </summary>
        public static void FilterOut<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (predicate == null) return;
            var toRemove = list.Where(t => !predicate.Invoke(t)).ToArray();
            foreach (var r in toRemove)
                list.Remove(r);
        }

        public static bool RecursiveValidator<T>(T start, Func<T, T> selector, Func<T, bool> validator)
        {
            if (start == null)
                return false;
            
            if (validator.Invoke(start)) return true;
            var next = selector.Invoke(start);
            
            if (next == null)
                return false;

            return RecursiveValidator(next, selector, validator);
        }

        public static void SetNodes(this int[,] map, IEnumerable<Vector> nodes, int value)
        {
            if (map == null) return;
            var size = new Vector(map.GetLength(0), map.GetLength(1));
            foreach (var node in nodes)
            {
                if (node >= Vector.Zero && node < size)
                    map[node.X, node.Y] = value;
            }
        }
    }
}
