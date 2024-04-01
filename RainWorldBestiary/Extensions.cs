using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary
{
    /// <summary>
    /// A class with some extensions to speed up certain tasks
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Wrap the text to go along multiple lines (word wrap). If the current letter count exceeds <paramref name="wrapCount"/> it adds a new line (\n) to the string
        /// </summary>
        public static string WrapText(this string text, int wrapCount)
        {
            string result = "";
            int l = 0;

            string[] split = text.Split(' ');
            foreach (string s in split)
            {
                if (s.Contains("\n"))
                    l = 0;

                if (l + s.Length > wrapCount)
                {
                    result += '\n';
                    l = 0;
                }

                result += " " + s;
                l += s.Length;
            }

            return result;
        }

        /// <summary>
        /// Splits the string into groups the size of <paramref name="groupCount"/>, if the string cannot be split evenly, the last group may contain an unexpected amount of characters
        /// </summary>
        public static string[] SplitIntoGroups(this string text, int groupCount)
        {
            int count = Mathf.CeilToInt(text.Length / (float)groupCount);

            string[] result = new string[count];

            for (int i = 0; i < count; i++)
            {
                if (i * groupCount + groupCount > text.Length)
                {
                    result[i] = text.Substring(i * groupCount);
                    break;
                }

                result[i] = text.Substring(i * groupCount, groupCount);
            }

            return result;
        }


        /// <summary>
        /// Checks if <paramref name="source"/> contains any of the strings in <paramref name="values"/>
        /// </summary>
        public static bool ContainsAny(this List<string> source, List<string> values)
        {
            if (values.Count == 0)
                return false;

            foreach (string t in values)
            {
                if (source.Contains(t))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Checks if <paramref name="source"/> contains any of the strings in <paramref name="values"/>
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            if (!values.Any())
                return false;

            foreach (T t in values)
            {
                if (source.Contains(t))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if this list contains all the values in <paramref name="values"/>
        /// </summary>
        /// <returns>True if all items in <paramref name="values"/> are found in this list</returns>
        public static bool ContainsAll<T>(this List<T> list, T[] values)
        {
            if (values.Length == 0)
                return true;

            foreach (T value in values)
            {
                if (!list.Contains(value))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Checks if this list contains all the values in <paramref name="values"/>
        /// </summary>
        /// <returns>True if all items in <paramref name="values"/> are found in this list</returns>
        public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> values)
        {
            if (!values.Any())
                return true;

            foreach (T value in values)
            {
                if (!list.Contains(value))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Gets a random item in the collection
        /// </summary>
        public static T GetRandom<T>(this IEnumerable<T> values) => values.ElementAt(UnityEngine.Random.Range(0, values.Count()));
        /// <summary>
        /// Gets a number of random items from the collection
        /// </summary>
        public static T[] GetRandom<T>(this IEnumerable<T> values, int amountToGet)
        {
            T[] result = new T[amountToGet];

            int max = values.Count();
            System.Random rand = new System.Random();

            for (int i = 0; i < amountToGet; i++)
            {
                result[i] = values.ElementAt(rand.Next(max));
            }

            return result;
        }

        /// <summary>
        /// Converts this <see cref="IntVector2"/> into a <see cref="Vector2"/>
        /// </summary>
        public static Vector2 ToVector2(this IntVector2 intVector) => new Vector2(intVector.x, intVector.y);
        /// <summary>
        /// Gets the x and y of this <see cref="WorldCoordinate"/> and returns it as a Vector2
        /// </summary>
        public static Vector2 ToVector2(this WorldCoordinate coordinate) => new Vector2(coordinate.x, coordinate.y);

        /// <inheritdoc cref="Bestiary.GetCreatureUnlockName(Creature, bool)"/>
        public static string GetCreatureUnlockName(this Creature creature, bool useSpecialIdLogic = true) => Bestiary.GetCreatureUnlockName(creature, useSpecialIdLogic);
        /// <inheritdoc cref="Bestiary.GetCreatureUnlockName(AbstractCreature, bool)"/>
        public static string GetCreatureUnlockName(this AbstractCreature creature, bool useSpecialIdLogic = true) => Bestiary.GetCreatureUnlockName(creature, useSpecialIdLogic);

        /// <summary>
        /// Converts this RGB color to a HSL color
        /// </summary>
        public static HSLColor ToHSL(this Color color)
        {
            float max = Math.Max(color.r, Math.Max(color.g, color.b));
            float min = Math.Min(color.r, Math.Min(color.g, color.b));
            float delta = max - min;

            float Hue;
            if (delta == 0f)
            {
                Hue = 0f;
            }
            else
            {
                if (max.Equals(color.r))
                    Hue = 60f * ((color.g - color.b) / delta % 6f);
                else if (max.Equals(color.g))
                    Hue = 60f * ((color.b - color.r) / delta + 2f);
                else
                    Hue = 60f * ((color.r - color.g) / delta + 4f);
            }

            float Lightness = (max + min) / 2f;
            float Saturation = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * Lightness - 1));

            if (Hue < 0)
                Hue += 360f;
            Hue /= 360f;

            return new HSLColor(Hue, Lightness, Saturation);
        }

        /// <summary>
        /// Converts this hex color string into a Color
        /// </summary>
        public static Color HexToColor(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Color.white;

            string def;
            if (hex.Length > 6)
                def = hex.Substring(hex.Length - 6);
            else
                def = "FFFFFF".Substring(hex.Length) + hex;

            string[] values = def.SplitIntoGroups(2);

            float R = Convert.ToByte(values[0], 16) / 255f;
            float G = Convert.ToByte(values[1], 16) / 255f;
            float B = Convert.ToByte(values[2], 16) / 255f;

            return new Color(R, G, B, 1f);
        }

        /// <summary>
        /// Converts this Color into a hex color string, with 6 characters in the string, representing RGB
        /// </summary>
        public static string RGBToHexString(this Color color)
        {
            return $"{color.r:X2}{color.g:X2}{color.b:X2}";
        }
        /// <summary>
        /// Converts this Color into a hex color string, with 8 characters in the string, representing ARGB
        /// </summary>
        public static string ARGBToHexString(this Color color)
        {
            return $"{color.a:X2}{color.r:X2}{color.g:X2}{color.b:X2}";
        }

        /// <inheritdoc cref="string.IndexOf(char, int)"/>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="ignoreWithinQuotes">Whether to ignore if the character is found within double quotes</param>
        public static int IndexOf(this string text, char value, int startIndex, bool ignoreWithinQuotes)
        {
            if (!ignoreWithinQuotes)
                return text.IndexOf(value, startIndex);

            bool inQuotes = false;

            int length = text.Length;
            for (int i = startIndex; i < length; ++i)
            {
                if (!inQuotes && text[i].Equals(value))
                    return i;
                else if (text[i].Equals('\"'))
                    inQuotes = !inQuotes;
            }

            return -1;
        }
    }

    /// <summary>
    /// A class for custom translation behaviour
    /// </summary>
    public static class Translator
    {
        /// <summary>
        /// Translates this text using the short strings dictionary
        /// </summary>
        public static string Translate(string text) => OptionInterface.Translate(text);
    }

    //class Ref<T>
    //{

    //}
}