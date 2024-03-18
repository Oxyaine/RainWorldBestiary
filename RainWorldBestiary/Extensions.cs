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
        /// Splits the string into groups the size of <paramref name="groupCount"/>, if the string cannot be split evenly, the last group may contain an unexcpected amount of characters
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
        /// Checks if <paramref name="source"/> contains any of the strings in <paramref name="target"/>
        /// </summary>
        public static bool ContainsAny(this List<string> source, List<string> target)
        {
            foreach (string t in target)
            {
                if (source.Contains(t))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if <paramref name="source"/> contains any of the strings in <paramref name="target"/>
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            foreach (T t in target)
            {
                if (source.Contains(t))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Gets a random item in the collection
        /// </summary>
        public static T GetRandom<T>(this IEnumerable<T> values) => values.ElementAt(Random.Range(0, values.Count()));
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
    }
}
