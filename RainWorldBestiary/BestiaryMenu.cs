using Menu;
using System;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryMenu : Dialog
    {
        private const int ButtonSizeX = 150;
        private const int ButtonSizeY = 40;
        private const int ButtonSpacing = 10;

        const string ReadEntryID = "ReadEntry";
        const string ViewTabID = "ViewTab";
        const string BackButtonMessage = "BACK";

        public BestiaryMenu(ProcessManager manager)
            : base(manager)
        {
            int X = Screen.width / 2 - ButtonSizeX / 2;
            int currentButtonY = Screen.height - ButtonSizeY - ButtonSpacing;
            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                AddButton(tab, pages[0], X, currentButtonY);

                currentButtonY -= ButtonSizeY + ButtonSpacing;
            }

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY));
            pages[0].subObjects.Add(backButton);

            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;
        }

        void AddButton(EntriesTab tab, Page page, in int x, in int y)
        {
            SimpleButton button = new SimpleButton(this, page, tab.Name, string.Concat(ReadEntryID, tab.Name), new Vector2(x, y), new Vector2(ButtonSizeX, ButtonSizeY));
            page.subObjects.Add(button);
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Singal(backObject, BackButtonMessage);
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                return;
            }

            if (message.StartsWith(ViewTabID))
            {
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);

                string id = message.Substring(ViewTabID.Length);
                pages[0].subObjects.RemoveRange(Bestiary.EntriesTabs.Count, pages[0].subObjects.Count - Bestiary.EntriesTabs.Count);
                foreach (EntriesTab v in Bestiary.EntriesTabs)
                {
                    if (v.Name.Equals(id))
                    {

                        break;
                    }
                }
            }
            else if (message.StartsWith(ReadEntryID))
            {
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);

                string entryName = message.Substring(ReadEntryID.Length);
                //Entry entry = Bestiary.GetEntryByName(entryName);

                //if (entry.IsLocked)
                //    TextDisplay.menuLabel.text = WrapText(entry.LockedText, WordWrapChars);
                //else
                //    TextDisplay.menuLabel.text = WrapText(entry.FullDescription, WordWrapChars);
            }
        }
        static string WrapText(string text, int wrapCount)
        {
            string result = "";
            int l = 0;

            string[] split = text.Split(' ');
            foreach (string s in split)
            {
                if (s.Contains("\n"))
                    l = 0;

                if (l + s.Length > wrapCount)
                {
                    result += '\n';
                    l = 0;
                }

                result += " " + s;
                l += s.Length;
            }

            return result;
        }
    }
}
