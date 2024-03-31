#if DEBUG
using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class InstructionManualPages : ExtEnum<InstructionManualPages>
    {
        public static readonly InstructionManualPages Introduction = new InstructionManualPages("Introduction", true);
        public static readonly InstructionManualPages Tabs = new InstructionManualPages("Tabs", true);
        public static readonly InstructionManualPages Entries = new InstructionManualPages("Entries", true);
        public static readonly InstructionManualPages Unlocking = new InstructionManualPages("Unlocking", true);

        public InstructionManualPages(string value, bool register = false)
            : base(value, register) { }
    }

    internal class BestiaryIntroductionPage : InstructionManualPage
    {
        public BestiaryIntroductionPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_INTRODUCTION_TITLE"), belowHeaderPos.y - spaceBuffer * 2f);
            string text = menu.Translate("BESTIARY_MANUAL_INTRODUCTION_1") + "\n" + menu.Translate("BESTIARY_MANUAL_INTRODUCTION_2") + "\n" + menu.Translate("BESTIARY_MANUAL_INTRODUCTION_3");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class TabsPage : InstructionManualPage
    {
        public TabsPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            //MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "", "", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true);
            //menuIllustration.sprite.SetAnchor(0.5f, 1f);
            //subObjects.Add(menuIllustration);

            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_TABS_TITLE"), belowHeaderPos.y /*- menuIllustration.sprite.height*/ - spaceBuffer * 2f);
            string text = menu.Translate("BESTIARY_MANUAL_TABS_1") + "\n" + menu.Translate("BESTIARY_MANUAL_TABS_2") + "\n" + menu.Translate("BESTIARY_MANUAL_TABS_3");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class EntriesPage : InstructionManualPage
    {
        public EntriesPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "illustrations\\bestiary\\titles", "The_Survivor_Title", belowHeaderPosCentered - verticalBuffer, crispPixels: false, anchorCenter: true)
            {
                sprite =
                {
                    scale = 0.5f
                }
            };
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);
            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_ENTIRES_TITLE"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
            string text = menu.Translate("BESTIARY_MANUAL_ENTRIES_1") + "\n" + menu.Translate("BESTIARY_MANUAL_ENTRIES_2") + "\n" + menu.Translate("BESTIARY_MANUAL_ENTRIES_3");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class UnlockingPage : InstructionManualPage
    {
        float currentPipY;
        float pipX;
        readonly System.Random random = new System.Random();

        public UnlockingPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            currentPipY = belowHeaderPosCentered.y - 50;
            pipX = belowHeaderPosCentered.x - 270;

            AddPips(menu, 1, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Appearance), true);
            AddPips(menu, 4, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Hunting), true);
            AddPips(menu, 5, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Utility), true);
            AddPips(menu, 5, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Behaviour), true);

            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_UNLOCKING_TITLE"), belowHeaderPos.y - spaceBuffer * 2f);
            string text = menu.Translate("BESTIARY_MANUAL_UNLOCKING_1") + "\n" + menu.Translate("BESTIARY_MANUAL_UNLOCKING_2") + "\n" + menu.Translate("BESTIARY_MANUAL_UNLOCKING_3");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }

        private void AddPips(Menu.Menu menu, int count, Color color, bool includeTitlePip = false)
        {
            if (includeTitlePip)
            {
                AddPip(menu, Color.white);
                currentPipY -= 20;
            }

            for (int i = 0; i < count; i++)
            {
                AddPip(menu, color);
                currentPipY -= 20;
            }
        }

        private void AddPip(Menu.Menu menu, Color color)
        {
            MenuIllustration pip = new MenuIllustration(menu, this, "illustrations\\bestiary\\icons", random.NextDouble() >= 0.5 ? ResourceManager.UnlockPipUnlockedName : ResourceManager.UnlockPipName,
                                new Vector2(pipX, currentPipY), true, true)
            {
                color = color,
                sprite =
                    {
                        scale = 2f
                    }
            };
            subObjects.Add(pip);
        }
    }
}

#endif