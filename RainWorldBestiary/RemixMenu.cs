using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, 0.5f, 0.5f);
        private Color ExperimentalColor = new Color(0.5f, 0.8f, 0.5f);

        public RemixMenu()
        {
            BestiarySettings.Default._MenuFadeTime = config.Bind("oxyaine_bestiary_menu_fade_time", 4);
            BestiarySettings.Default.ShowModuleLockPips = config.Bind("oxyaine_bestiary_show_module_lock_pips", true);

            BestiarySettings.Experimental.PerformanceMode = config.Bind("oxyaine_bestiary_performance_mode", false);

            BestiarySettings.Cheats.UnlockAllEntries = config.Bind("oxyaine_bestiary_unlock_all_entries", false);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, "Default");
            OpTab experimental = new OpTab(this, "Experimental") { colorButton = ExperimentalColor };
            OpTab cheats = new OpTab(this, "Cheats") { colorButton = CheatColor };
            Tabs = new[] { def, experimental, cheats };

            List<UIelement> items = new List<UIelement>();

            //Default Tab
            AddElements(ref items, "Menu Fade Time", BestiarySettings.Default._MenuFadeTime, asSlider: true, description: "The time the bestiary's sub-menus will take to fade from one menu to the other.");
            AddElements(ref items, "Show Unlock Pips", BestiarySettings.Default.ShowModuleLockPips, description: "Whether to, while reading an entry, show pips in the top right, displaying which parts of the description you have unlocked.");
            def.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Experimental Tab
            AddElements(ref items, "Performance Mode", BestiarySettings.Experimental.PerformanceMode, ExperimentalColor, description: "EXPERIMENTAL: Changes a couple things in the menus that may help increase performance if you're experiencing issues");
            experimental.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Cheats Tab
            AddElements(ref items, "Unlock All Entries", BestiarySettings.Cheats.UnlockAllEntries, color: CheatColor, description: "Unlocks all entries and their full descriptions.");
            cheats.AddItems(items.ToArray());
        }

        

        private void ResetElementPositions()
        {
            currentElementX = 140f;
            currentLabelX = 20f;
            currentY = 550f;
        }
        float currentElementX = 140f, currentLabelX = 20f, currentY = 550f;
        private void AddElements(ref List<UIelement> items, string name, UIelement element, Color? color = null, string description = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white, description = description });
            items.Add(element);

            currentY -= 30f;
        }
        private void AddElements(ref List<UIelement> items, string name, Configurable<int> element, bool asSlider = false, Color? color = null, string description = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            if (asSlider)
                items.Add(new OpSlider(element, new Vector2(currentElementX, currentY), 200) { colorEdge = color ?? Color.white, description = description, max = 10 });
            else
                items.Add(new OpDragger(element, currentElementX, currentY) { colorEdge = color ?? Color.white, description = description });

            currentY -= 30f;
        }
        private void AddElements(ref List<UIelement> items, string name, Configurable<bool> element, Color? color = null, string description = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            items.Add(new OpCheckBox(element, new Vector2(currentElementX, currentY)) { colorEdge = color ?? Color.white, description = description });

            currentY -= 30f;
        }
    }

    /// <summary>
    /// The main class for all the bestiary mods' settings, including remix menu options
    /// </summary>
    public static class BestiarySettings
    {   
        /// <summary>
        /// All settings that are uncategorized, meaning they're not considered, cheats or experimental
        /// </summary>
        public static class Default
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
        }
        /// <summary>
        /// All settings that are currently experimental
        /// </summary>
        public static class Experimental
        {
            /// <summary>
            /// Changes some things about the bestiary for better performance
            /// </summary>
            public static Configurable<bool> PerformanceMode;
        }
        /// <summary>
        /// All settings that are considered cheats
        /// </summary>
        public static class Cheats
        {
            /// <summary>
            /// Whether all bestiary entries should be unlocked and completely readable
            /// </summary>
            public static Configurable<bool> UnlockAllEntries;
        }
    }
}
