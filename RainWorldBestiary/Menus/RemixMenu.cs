using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, 0.5f, 0.5f);
        //private Color ExperimentalColor = new Color(0.5f, 0.8f, 0.7f);
        private Color DangerColor = new Color(1f, 0.2f, 0.2f);

        public RemixMenu()
        {
            Bestiary.Settings._MenuFadeTime = config.Bind("bestiary_menu_fade_time", 4);
            Bestiary.Settings.ShowModuleLockPips = config.Bind("bestiary_show_module_lock_pips", true);
            Bestiary.Settings.PerformTextAnimations = config.Bind("bestiary_perform_text_animation", true);
            Bestiary.Settings.ShowManualButton = config.Bind("bestiary_show_manual_button", true);
            Bestiary.Settings.ConsistentTitleSpacing = config.Bind("bestiary_use_character_spacing", false);

            Bestiary.Settings.UnlockAllEntries = config.Bind("bestiary_unlock_all_entries", false);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, Translator.Translate("Default"));
            //OpTab experimental = new OpTab(this, Translator.Translate("Experimental")) { colorButton = ExperimentalColor };
            OpTab cheats = new OpTab(this, Translator.Translate("Cheats")) { colorButton = CheatColor };
            OpTab danger = new OpTab(this, Translator.Translate("Danger Zone")) { colorButton = DangerColor };
            Tabs = new[] { def, /*experimental,*/ cheats, danger };

            List<UIelement> items = new List<UIelement>();

            //Default Tab
            AddElements(ref items, Translator.Translate("Menu Fade Time"), Bestiary.Settings._MenuFadeTime, asSlider: true, description: Translator.Translate("OXY.BESTIARY.MENU_FADE_TIME_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Show Unlock Pips"), Bestiary.Settings.ShowModuleLockPips, description: Translator.Translate("OXY.BESTIARY.SHOW_UNLOCK_PIPS_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Perform Text Reveal Animation"), Bestiary.Settings.PerformTextAnimations, description: Translator.Translate("OXY.BESTIARY.SHOW_TEXT_ANIMATIONS_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Show Manual Button"), Bestiary.Settings.ShowManualButton, description: Translator.Translate("OXY.BESTIARY.SHOW_MANUAL_BUTTON_DESCRIPTION"));
            AddElements(ref items, Translator.Translate("Use Consistent Title Character Spacing"), Bestiary.Settings.ConsistentTitleSpacing, description: Translator.Translate("OXY.BESTIARY.USE_CHARACTER_SPACING_DESCRIPTION"));
            def.AddItems(items.ToArray());

            items.Clear();
            ResetElementPositions();

            // Experimental Tab

            //experimental.AddItems(items.ToArray());

            //items.Clear();
            //ResetElementPositions();

            // Cheats Tab
            AddElements(ref items, Translator.Translate("Unlock All Entries"), Bestiary.Settings.UnlockAllEntries, color: CheatColor, description: Translator.Translate("OXY.BESTIARY.UNLOCK_ALL_ENTRIES_DESCRIPTION"));
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
}
