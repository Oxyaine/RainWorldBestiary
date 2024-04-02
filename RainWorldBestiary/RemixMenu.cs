using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, 0.5f, 0.5f);
        private Color ExperimentalColor = new Color(0.5f, 0.8f, 0.7f);
        private Color DangerColor = new Color(1f, 0.2f, 0.2f);

        public RemixMenu()
        {
            BestiarySettings._MenuFadeTime = config.Bind("bestiary_menu_fade_time", 4);
            BestiarySettings.ShowModuleLockPips = config.Bind("bestiary_show_module_lock_pips", true);
            BestiarySettings.PerformTextAnimations = config.Bind("bestiary_perform_text_animation", true);
            BestiarySettings.ShowManualButton = config.Bind("bestiary_show_manual_button", true);

            BestiarySettings.MinimizeTitleSpacing = config.Bind("bestiary_use_characters_spacing_for_names_EXP", false);

            BestiarySettings.UnlockAllEntries = config.Bind("bestiary_unlock_all_entries", false);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, Translator.Translate("Default"));
            OpTab experimental = new OpTab(this, Translator.Translate("Experimental")) { colorButton = ExperimentalColor };
            OpTab cheats = new OpTab(this, Translator.Translate("Cheats")) { colorButton = CheatColor };
            OpTab danger = new OpTab(this, Translator.Translate("Danger Zone")) { colorButton = DangerColor };
            Tabs = new[] { def, experimental, cheats, danger };

            List<UIelement> items = new List<UIelement>();

            //Default Tab
            AddElements(ref items, Translator.Translate("Menu Fade Time"), BestiarySettings._MenuFadeTime, asSlider: true, description: Translator.Translate("OXY.BESTIARY.MENU_FADE_TIME_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Show Unlock Pips"), BestiarySettings.ShowModuleLockPips, description: Translator.Translate("OXY.BESTIARY.SHOW_UNLOCK_PIPS_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Perform Text Reveal Animation"), BestiarySettings.PerformTextAnimations, description: Translator.Translate("OXY.BESTIARY.SHOW_TEXT_ANIMATIONS_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Show Manual Button"), BestiarySettings.ShowManualButton, description: Translator.Translate("OXY.BESTIARY.SHOW_MANUAL_BUTTON_DESCRIPTION"));
            def.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Experimental Tab
            AddElements(ref items, Translator.Translate("Minimize Title Character Spacing"), BestiarySettings.MinimizeTitleSpacing, ExperimentalColor, description: Translator.Translate("OXY.BESTIARY.USE_CHARACTER_SPACING_DESCRIPTION"));
            experimental.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Cheats Tab
            AddElements(ref items, Translator.Translate("Unlock All Entries"), BestiarySettings.UnlockAllEntries, color: CheatColor, description: Translator.Translate("OXY.BESTIARY.UNLOCK_ALL_ENTRIES_DESCRIPTION"));
            cheats.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Danger Zone Tab
            OpHoldButton DeleteSavedDataButton = new OpHoldButton(new Vector2(40f, 400f), 75, Translator.Translate("Delete Unlock Data"), 500) { colorFill = DangerColor, colorEdge = DangerColor, description = Translator.Translate("OXY.BESTIARY.DELETE_UNLOCK_DATA_DESCRIPTION") };
            DeleteSavedDataButton.OnHeld += DeleteSavedDataButton_OnHeld;
            DeleteSavedDataButton.OnPressDone += DeleteSavedDataButton_OnPressDone;
            OpLabel label = new OpLabel(20f, 570f, Translator.Translate("Current Save Slot:") + " " + (Main.CurrentSaveSlot + 1), false);
            WarningLabel = new OpLabel(20f, 350f, Translator.Translate("OXY.BESTIARY.DELETE_UNLOCK_DATA_WARNING").WrapText(80), false) { color = DangerColor, alpha = 0f };
            danger.AddItems(DeleteSavedDataButton, label, WarningLabel);
        }

        private void DeleteSavedDataButton_OnHeld(bool held)
        {
            if (held)
            {
                fadeOutWarningLabel = false;
                WarningLabel.alpha = 1.5f;
            }
            else
                fadeOutWarningLabel = true;
        }

        OpLabel WarningLabel = null;

        private void DeleteSavedDataButton_OnPressDone(UIfocusable trigger) => Bestiary.DELETESaveDataInSlot();

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

        bool fadeOutWarningLabel = false;
        public override void Update()
        {
            base.Update();

            if (fadeOutWarningLabel)
            {
                WarningLabel.alpha -= Time.deltaTime / WarningLabel.alpha;

                if (WarningLabel.alpha <= 0)
                    fadeOutWarningLabel = false;
            }
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
