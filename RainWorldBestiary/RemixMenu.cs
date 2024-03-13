using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, 0.5f, 0.5f);

        public RemixMenu()
        {
            BestiarySettings.Default._MenuFadeTime = config.Bind("oxyaine_menu_fade_time", 4);
            BestiarySettings.Default.ShowModuleLockPips = config.Bind("oxyaine_show_module_lock_pips", true);

            BestiarySettings.Cheats.UnlockAllEntries = config.Bind("oxyaine_unlock_all_entries", false);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, "Default");
            //OpTab experimental = new OpTab(this, "Experimental");
            OpTab cheats = new OpTab(this, "Cheats") { colorButton = CheatColor };
            Tabs = new[] { def, cheats };

            List<UIelement> items = new List<UIelement>();
            AddElements(ref items, "Menu Fade Time", BestiarySettings.Default._MenuFadeTime, true);
            AddElements(ref items, "Show Unlock Pips", BestiarySettings.Default.ShowModuleLockPips);
            def.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            AddElements(ref items, "Unlock All Entries", BestiarySettings.Cheats.UnlockAllEntries, CheatColor);
            cheats.AddItems(items.ToArray());
        }

        private void ResetElementPositions()
        {
            currentElementX = 140f;
            currentLabelX = 20f;
            currentY = 550f;
        }
        float currentElementX = 140f, currentLabelX = 20f, currentY = 550f;
        private void AddElements(ref List<UIelement> items, string name, UIelement element, Color? color = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            items.Add(element);

            currentY -= 30f;
        }
        private void AddElements(ref List<UIelement> items, string name, Configurable<int> element, bool asSlider = false, Color? color = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            if (asSlider)
                items.Add(new OpSlider(element, new Vector2(currentElementX, currentY), 200) { colorEdge = color ?? Color.white });
            else
                items.Add(new OpDragger(element, currentElementX, currentY) { colorEdge = color ?? Color.white });

            currentY -= 30f;
        }
        private void AddElements(ref List<UIelement> items, string name, Configurable<bool> element, Color? color = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            items.Add(new OpCheckBox(element, new Vector2(currentElementX, currentY)) { colorEdge = color ?? Color.white });

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
