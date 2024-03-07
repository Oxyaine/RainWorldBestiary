using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
        const int WrapCount = 120;

        const string BackButtonMessage = "BACK";
        public EntryReadingMenu(ProcessManager manager) : base(manager)
        {
            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                if (tab.Name.Equals(BestiaryMenu.CurrentSelectedTab))
                {
                    foreach (Entry entry in tab)
                    {
                        if (entry.Name.Equals(BestiaryTabMenu.CurrentSelectedEntry))
                        {
                            DisplayEntryInformation(entry);

                            break;
                        }
                    }

                    break;
                }
            }
        }

        public void DisplayEntryInformation(Entry entry)
        {
            MenuLabel label = new MenuLabel(this, pages[0], entry.Info.Description.ToString().WrapText(WrapCount), new Vector2(Screen.width / 3.5f, Screen.height / 2), new Vector2(1, 1), false);
            pages[0].subObjects.Add(label);

            FLabel label2 = new FLabel("DisplayFont", entry.Name)
            {
                scale = 3f,
                x = Screen.width / 2,
                y = Screen.height - 50
            };
            pages[0].Container.AddChild(label2);
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, 0.3f);
                return;
            }
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Singal(backObject, BackButtonMessage);
            }
        }
    }
}
