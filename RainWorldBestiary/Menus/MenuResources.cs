namespace RainWorldBestiary.Menus
{
    internal class MenuResources
    {
        // All the resources the menus need to properly operate, this is always null when not in the menus
        public static MenuResources Instance { get; internal set; } = null;
        public static void Create() => Instance = new MenuResources();
        public static void Dispose() => Instance = null;
        public MenuResources() { }
        //~MenuResources() { }

        public readonly string IllustrationsIconsPath = "illustrations\\bestiary\\icons";
        public readonly string IllustrationsEntryIconsPath = "illustrations\\bestiary\\entry_icons";
        public readonly string IllustrationsTitlesPath = "illustrations\\bestiary\\titles";

        public readonly string UnlockPipUnlockedName = "unlock_pip_full";
        public readonly string UnlockPipName = "unlock_pip";

        public readonly string[] Characters = new string[]
        {
            "illustrations\\bestiary\\icons\\Char_1",
            "illustrations\\bestiary\\icons\\Char_2",
            "illustrations\\bestiary\\icons\\Char_3",
            "illustrations\\bestiary\\icons\\Char_4",
            "illustrations\\bestiary\\icons\\Char_5",
            "illustrations\\bestiary\\icons\\Char_6",
            "illustrations\\bestiary\\icons\\Char_7",
            "illustrations\\bestiary\\icons\\Char_8",
            "illustrations\\bestiary\\icons\\Char_9",
            "illustrations\\bestiary\\icons\\Char_10",
            "illustrations\\bestiary\\icons\\Char_11",
            "illustrations\\bestiary\\icons\\Char_12",
            "illustrations\\bestiary\\icons\\Char_13",
            "illustrations\\bestiary\\icons\\Char_14",
            "illustrations\\bestiary\\icons\\Char_15",
            "illustrations\\bestiary\\icons\\Char_16",
            "illustrations\\bestiary\\icons\\Char_17",
            "illustrations\\bestiary\\icons\\Char_18",
            "illustrations\\bestiary\\icons\\Char_19",
            "illustrations\\bestiary\\icons\\Char_20",
            "illustrations\\bestiary\\icons\\Char_21",
            "illustrations\\bestiary\\icons\\Char_22",
            "illustrations\\bestiary\\icons\\Char_23",
            "illustrations\\bestiary\\icons\\Char_24",
            "illustrations\\bestiary\\icons\\Char_25",
            "illustrations\\bestiary\\icons\\Char_26",
            "illustrations\\bestiary\\icons\\Char_27"
        };
    }
}
