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
            "Char_1",
            "Char_2",
            "Char_3",
            "Char_4",
            "Char_5",
            "Char_6",
            "Char_7",
            "Char_8",
            "Char_9",
            "Char_10",
            "Char_11",
            "Char_12",
            "Char_13",
            "Char_14",
            "Char_15",
            "Char_16",
            "Char_17",
            "Char_18",
            "Char_19",
            "Char_20",
            "Char_21",
            "Char_22",
            "Char_23",
            "Char_24",
            "Char_25",
            "Char_26",
            "Char_27"
        };
    }
}
