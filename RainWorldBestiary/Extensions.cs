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
    }
}
