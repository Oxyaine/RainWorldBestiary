﻿namespace RainWorldBestiary
{
    public static partial class Bestiary
    {
        /// <summary>
        /// The class that holds all the remix menu options and their current values
        /// </summary>
        public static class Settings
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
            /// Makes the generated title in the menus use consistent spacing between each character, which brings them further apart, but makes the spacing more consistent.
            /// </summary>
            public static Configurable<bool> ConsistentTitleSpacing;
            /// <summary>
            /// Determines whether the icons of the entry should appear next to the title (that appears when reading the entry)
            /// </summary>
            public static Configurable<bool> ShowTitleIcons;



            /// <summary>
            /// Whether all bestiary entries should be unlocked and completely readable
            /// </summary>
            public static Configurable<bool> UnlockAllEntries;
        }
    }
}
