using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, 0.5f, 0.5f);
        private Color ExperimentalColor = new Color(0.5f, 0.8f, 0.7f);

        public RemixMenu()
        {
            BestiarySettings._MenuFadeTime = config.Bind("oxyaine_bestiary_menu_fade_time", 4);
            BestiarySettings.ShowModuleLockPips = config.Bind("oxyaine_bestiary_show_module_lock_pips", true);
            BestiarySettings.PerformTextAnimations = config.Bind("oxyaine_bestiary_perform_text_animation", true);
            BestiarySettings.PlayRainSoundsWhileReading = config.Bind("oxyaine_bestiary_play_rain_sounds_while_reading", true);

            BestiarySettings.ShowEntryUnlockPercent = config.Bind("oxyaine_bestiary_show_entry_unlock_percent_EXP", false);
            BestiarySettings.MinimizeTitleSpacing = config.Bind("oxyaine_bestiary_use_characters_spacing_for_names_EXP", false);

            BestiarySettings.UnlockAllEntries = config.Bind("oxyaine_bestiary_unlock_all_entries", false);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, Translate("Default"));
            OpTab experimental = new OpTab(this, Translate("Experimental")) { colorButton = ExperimentalColor };
            OpTab cheats = new OpTab(this, Translate("Cheats")) { colorButton = CheatColor };
            Tabs = new[] { def, experimental, cheats };

            List<UIelement> items = new List<UIelement>();

            //Default Tab
            AddElements(ref items, Translate("Menu Fade Time"), BestiarySettings._MenuFadeTime, asSlider: true, description: Translate("MENU_FADE_TIME_DESCRIPTION"));
            AddElements(ref items, Translate("Show Unlock Pips"), BestiarySettings.ShowModuleLockPips, description: Translate("SHOW_UNLOCK_PIPS_DESCRIPTION"));
            AddElements(ref items, Translate("Perform Text Reveal Animation"), BestiarySettings.PerformTextAnimations, description: Translate("SHOW_TEXT_ANIMATIONS_DESCRIPTION"));
            AddElements(ref items, Translate("Play Rain Sounds While Reading"), BestiarySettings.PlayRainSoundsWhileReading, description: Translate("PLAY_RAIN_SOUNDS_WHILE_READING_DESCRIPTION"));
            def.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Experimental Tab
            AddElements(ref items, Translate("Show Entry Unlock Percent"), BestiarySettings.ShowEntryUnlockPercent, ExperimentalColor, description: Translate("SHOW_ENTRY_UNLOCK_PERCENT_DESCRIPTION"));
            AddElements(ref items, Translate("Minimize Title Character Spacing"), BestiarySettings.MinimizeTitleSpacing, ExperimentalColor, description: Translate("USE_CHARACTER_SPACING_DESCRIPTION"));
            experimental.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Cheats Tab
            AddElements(ref items, Translate("Unlock All Entries"), BestiarySettings.UnlockAllEntries, color: CheatColor, description: Translate("UNLOCK_ALL_ENTRIES_DESCRIPTION"));
            cheats.AddItems(items.ToArray());
        }

        

        private void ResetElementPositions()
        {
            currentLabelX = 20f;
            currentY = 550f;
        }
        float currentLabelX = 20f, currentY = 550f;
        private void AddElements(ref List<UIelement> items, string name, Configurable<int> element, bool asSlider = false, Color? color = null, string description = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            if (asSlider)
                items.Add(new OpSlider(element, new Vector2(currentLabelX + name.Length * 7f, currentY), 200) { colorEdge = color ?? Color.white, description = description, max = 10 });
            else
                items.Add(new OpDragger(element, currentLabelX + name.Length * 7f, currentY) { colorEdge = color ?? Color.white, description = description });

            currentY -= 30f;
        }
        private void AddElements(ref List<UIelement> items, string name, Configurable<bool> element, Color? color = null, string description = null)
        {
            items.Add(new OpLabel(currentLabelX, currentY + 2.5f, name, false) { color = color ?? Color.white });
            items.Add(new OpCheckBox(element, new Vector2(currentLabelX + name.Length * 7f, currentY)) { colorEdge = color ?? Color.white, description = description });

            currentY -= 30f;
        }
    }

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
        /// Whether to play the rain audio, that is present while sleeping, while reading an entry.
        /// </summary>
        public static Configurable<bool> PlayRainSoundsWhileReading;


        /// <summary>
        /// EXPERIMENTAL
        /// </summary>
        public static Configurable<bool> ShowEntryUnlockPercent;
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
