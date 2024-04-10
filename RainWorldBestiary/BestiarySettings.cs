namespace RainWorldBestiary
{
    /// <summary>
    /// The main class for all the bestiary mods' settings, including remix menu options
    /// </summary>
    public static class BestiarySettings
    {
        internal static Configurable<int> _MenuFadeTime;
        /// <summary>
        /// The time the bestiary menu's should take to fade between each other, does not affect non bestiary menus
        /// </summary>
        public static float MenuFadeTimeSeconds => _MenuFadeTime.Value / 10f;
        /// <summary>
        /// Whether to show the little pips in the top right while reading an entry, to show how many modules of the bestiary you have unlocked
        /// </summary>
        public static Configurable<bool> ShowModuleLockPips;
        /// <summary>
        /// Whether to perform an animation when going to read an entry
        /// </summary>
        public static Configurable<bool> PerformTextAnimations;
        /// <summary>
        /// Whether to show the "Manual" button in the main bestiary menu
        /// </summary>
        public static Configurable<bool> ShowManualButton;


        /// <summary>
        /// EXPERIMENTAL: Makes the generated title in the menus use smaller spacing between each character, which brings them closer together and closer to real text (still a bit buggy so its in experimental
        /// </summary>
        public static Configurable<bool> MinimizeTitleSpacing;


        /// <summary>
        /// Whether all bestiary entries should be unlocked and completely readable
        /// </summary>
        public static Configurable<bool> UnlockAllEntries;
    }
}
