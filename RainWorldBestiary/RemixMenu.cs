using Menu.Remix.MixedUI;
using UnityEngine;

namespace RainWorldBestiary
{
    public class RemixMenu : OptionInterface
    {
        private Color CheatColor = new Color(1f, .5f, .5f);

        internal RemixMenu()
        {
            UnlockAllEntries = config.Bind("oxyaine_unlock_all_entries", false);

            _MenuFadeTime = config.Bind("oxyaine_menu_fade_time", 4);
        }

        public override void Initialize()
        {
            OpTab def = new OpTab(this, "Default");
            OpTab cheats = new OpTab(this, "Cheats") { colorButton = CheatColor };
            Tabs = new[] { def, cheats };

            UIelement[] uiElements = new UIelement[]
            {
                new OpLabel(20f, 552.5f, "Menu Fade Time", false),
                new OpSlider(_MenuFadeTime, new Vector2(140f, 550f), 150, false) { max = 10 },
            };
            def.AddItems(uiElements);

            UIelement[] uiElements2 = new UIelement[]
            {
                new OpLabel(20f, 552.5f, "Unlock All Entries", false) { color = CheatColor },
                new OpCheckBox(UnlockAllEntries, 140f, 550f) { colorEdge = CheatColor },
            };
            cheats.AddItems(uiElements2);
        }

        public readonly Configurable<bool> UnlockAllEntries;
        private readonly Configurable<int> _MenuFadeTime;
        public float MenuFadeTime => _MenuFadeTime.Value / 10f;
    }
}
