#if DEBUG
using Menu;

namespace RainWorldBestiary
{
    internal class InstructionManualPages : ExtEnum<InstructionManualPages>
    {
        public static readonly InstructionManualPages Introduction = new InstructionManualPages("Introduction", true);
        public static readonly InstructionManualPages Tabs = new InstructionManualPages("Tabs", true);
        public static readonly InstructionManualPages Entries = new InstructionManualPages("Entries", true);
        public static readonly InstructionManualPages Unlocking = new InstructionManualPages("Unlocking", true);

        public InstructionManualPages(string value, bool register = false)
            : base(value, register)
        {
        }
    }


    internal class BestiaryIntroductionPage : InstructionManualPage
    {
        public BestiaryIntroductionPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "", "manual_jolly_difficulties", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true);
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);
            float startY = AddManualText(menu.Translate("MANUAL_DIFFICULTY_1"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
            string text = menu.Translate("MANUAL_DIFFICULTY_2") + "\n" + menu.Translate("MANUAL_DIFFICULTY_3") + "\n" + menu.Translate("MANUAL_DIFFICULTY_4");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class TabsPage : InstructionManualPage
    {
        public TabsPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "", "manual_jolly_difficulties", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true);
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);
            float startY = AddManualText(menu.Translate("MANUAL_DIFFICULTY_1"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
            string text = menu.Translate("MANUAL_DIFFICULTY_2") + "\n" + menu.Translate("MANUAL_DIFFICULTY_3") + "\n" + menu.Translate("MANUAL_DIFFICULTY_4");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class EntriesPage : InstructionManualPage
    {
        public EntriesPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "", "manual_jolly_difficulties", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true);
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);
            float startY = AddManualText(menu.Translate("MANUAL_DIFFICULTY_1"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
            string text = menu.Translate("MANUAL_DIFFICULTY_2") + "\n" + menu.Translate("MANUAL_DIFFICULTY_3") + "\n" + menu.Translate("MANUAL_DIFFICULTY_4");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
    internal class UnlockingPage : InstructionManualPage
    {
        public UnlockingPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            MenuIllustration menuIllustration = new MenuIllustration(menu, owner, "", "manual_jolly_difficulties", belowHeaderPosCentered - verticalBuffer, crispPixels: true, anchorCenter: true);
            menuIllustration.sprite.SetAnchor(0.5f, 1f);
            subObjects.Add(menuIllustration);
            float startY = AddManualText(menu.Translate("MANUAL_DIFFICULTY_1"), belowHeaderPos.y - menuIllustration.sprite.height - spaceBuffer * 2f);
            string text = menu.Translate("MANUAL_DIFFICULTY_2") + "\n" + menu.Translate("MANUAL_DIFFICULTY_3") + "\n" + menu.Translate("MANUAL_DIFFICULTY_4");
            AddManualText(text, startY, bigText: false, centered: false, rectWidth * 0.6f);
        }
    }
}

#endif