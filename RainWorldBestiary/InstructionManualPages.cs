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
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.8f);
        }
    }
    internal class TabsPage : InstructionManualPage
    {
        public TabsPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_TABS_TITLE"), belowHeaderPos.y - spaceBuffer * 2f);
            string text = menu.Translate("BESTIARY_MANUAL_TABS_1") + "\n" + menu.Translate("BESTIARY_MANUAL_TABS_2") + "\n" + menu.Translate("BESTIARY_MANUAL_TABS_3");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.8f);
        }
    }
    internal class EntriesPage : InstructionManualPage
    {
        public EntriesPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, Bestiary.MenuResources.IllustrationsEntryIconsPath, "Survivor_head", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true)
            {
                sprite =
                {
                    scale = 4f
                }
            };
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);

            AddManualText(menu.Translate("BESTIARY_MANUAL_ENTIRES_TITLE"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
        }
    }

    internal class UnlockingPageFirst : InstructionManualPage
    {
        readonly float PipY;
        float PipX;
        readonly System.Random random = new System.Random();

        public UnlockingPageFirst(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            PipY = belowHeaderPosCentered.y - 60;
            PipX = belowHeaderPosCentered.x - 200;

            AddPips(menu, 1, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.AlwaysAvailable), false);
            AddPips(menu, 1, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Appearance), true);
            AddPips(menu, 4, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Hunting), true);
            AddPips(menu, 5, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Utility), true);
            AddPips(menu, 5, DescriptionModule.GetColorFromPredefined(DescriptionModule.PredefinedColors.Behaviour), true);

            float startY = AddManualText(menu.Translate("BESTIARY_MANUAL_UNLOCKING_TITLE"), belowHeaderPos.y - 50 - spaceBuffer * 2f);
            AddManualText(menu.Translate("BESTIARY_MANUAL_UNLOCKING_1"), startY - 20, bigText: true);
        }

        private void AddPips(Menu.Menu menu, int count, Color color, bool includeTitlePip = false)
        {
            float PX = PipX;
            PipX += 20;
            bool unl = false;

            for (int i = 0; i < count; i++)
            {
                AddRandomPip(menu, color, out bool unlocked);
                if (unlocked) unl = true;
                PipX += 20;
            }

            if (includeTitlePip)
            {
                MenuIllustration pip = new MenuIllustration(menu, this, Bestiary.MenuResources.IllustrationsIconsPath, unl ? Bestiary.MenuResources.UnlockPipUnlockedName : Bestiary.MenuResources.UnlockPipName, new Vector2(PX, PipY), true, true)
                {
                    sprite =
                    {
                        scale = 2f
                    }
                };
                subObjects.Add(pip);
            }
        }

        private void AddRandomPip(Menu.Menu menu, Color color, out bool unlocked)
        {
            unlocked = random.NextDouble() >= 0.5;
            MenuIllustration pip = new MenuIllustration(menu, this, Bestiary.MenuResources.IllustrationsIconsPath, unlocked ? Bestiary.MenuResources.UnlockPipUnlockedName : Bestiary.MenuResources.UnlockPipName, new Vector2(PipX, PipY), true, true)
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
    internal class UnlockingPageSecond : InstructionManualPage
    {
        public UnlockingPageSecond(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            float currentPipY = belowHeaderPosCentered.y - 100;
            float currentPipX = belowHeaderPosCentered.x - 150;
            const float PipDistanceX = 300;
            const float PipDistanceY = 70;

            MenuIllustration pip = new MenuIllustration(menu, this, Bestiary.MenuResources.IllustrationsIconsPath, Bestiary.MenuResources.UnlockPipName, new Vector2(currentPipX, currentPipY - 10), true, true)
            {
                sprite = { scale = 3f }
            };
            subObjects.Add(pip);

            currentPipX += PipDistanceX;

            pip = new MenuIllustration(menu, this, Bestiary.MenuResources.IllustrationsIconsPath, Bestiary.MenuResources.UnlockPipUnlockedName, new Vector2(currentPipX, currentPipY - 10), true, true)
            {
                sprite = { scale = 3f }
            };
            subObjects.Add(pip);

            AddManualText(menu.Translate("BESTIARY_MANUAL_UNLOCKING_TITLE_P2"), belowHeaderPos.y - 20);
            AddManualText(menu.Translate("BESTIARY_MANUAL_TWO_PIP_TYPES"), belowHeaderPos.y - 55, false);

            MenuLabel label = new MenuLabel(menu, this, menu.Translate("A Locked Pip"), new Vector2(currentPipX - PipDistanceX, currentPipY - 35), Vector2.one, false);
            subObjects.Add(label);

            label = new MenuLabel(menu, this, menu.Translate("An Unlocked Pip"), new Vector2(currentPipX, currentPipY - 35), Vector2.one, false);
            subObjects.Add(label);


            AddManualText(menu.Translate("BESTIARY_MANUAL_UNLOCK_PIP_TYPES"), currentPipY - 60, false);


            bool affectY = false;
            currentPipX -= PipDistanceX;
            currentPipY -= PipDistanceY + 60;
            (DescriptionModule.PredefinedColors color, string title)[] predefinedColors = new (DescriptionModule.PredefinedColors color, string title)[]
            {
                (DescriptionModule.PredefinedColors.None, "BSTRY_PREDEFINED_COLOR_NONE"),
                (DescriptionModule.PredefinedColors.AlwaysAvailable, "BSTRY_PREDEFINED_COLOR_ALWAYSAVAILABLE"),
                (DescriptionModule.PredefinedColors.Combat, "BSTRY_PREDEFINED_COLOR_COMBAT"),
                (DescriptionModule.PredefinedColors.Appearance, "BSTRY_PREDEFINED_COLOR_APPEARANCE"),
                (DescriptionModule.PredefinedColors.Hunting, "BSTRY_PREDEFINED_COLOR_HUNTING"),
                (DescriptionModule.PredefinedColors.Utility, "BSTRY_PREDEFINED_COLOR_UTILITY"),
                (DescriptionModule.PredefinedColors.Behaviour, "BSTRY_PREDEFINED_COLOR_BEHAVIOUR"),
            };
            foreach ((DescriptionModule.PredefinedColors color, string title) color in predefinedColors)
            {
                Color c = DescriptionModule.GetColorFromPredefined(color.color);
                pip = new MenuIllustration(menu, this, Bestiary.MenuResources.IllustrationsIconsPath, Bestiary.MenuResources.UnlockPipUnlockedName, new Vector2(currentPipX, currentPipY), true, true)
                {
                    sprite = { scale = 3f },
                    color = c
                };
                subObjects.Add(pip);

                label = new MenuLabel(menu, this, menu.Translate(color.title).WrapText(50), new Vector2(currentPipX, currentPipY - 25), Vector2.one, false);
                subObjects.Add(label);

                if (affectY) {
                    currentPipX -= PipDistanceX;
                    currentPipY -= PipDistanceY;
                    affectY = false;
                }
                else {
                    currentPipX += PipDistanceX;
                    affectY = true;
                }
            }
        }
    }
}