using Menu;
using System;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryMenu : Dialog
    {
        private const int ButtonSizeX = 145;
        private const int ButtonSizeY = 25;
        private const int ButtonSpacing = 8;
        private const int MaxButtonsPerRow = 5;
        private const int TextBoxSize = 600;
        private const int WordWrapChars = 85;

        const string ReadEntryID = "ReadEntry";
        const string ViewTabID = "ViewTab";
        const string BackButtonMessage = "BACK";

        public BestiaryMenu(ProcessManager manager)
            : base(manager)
        {
            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 50f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            BuildMenu();
        }

        void BuildMenu()
        {
            int i = 0;
            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                int currentButtonPosition = ButtonSpacing;
                int currentButtonRow = 0;

                AddButton(tab, pages[0], in currentButtonPosition, in currentButtonRow);

                i++;
            }
        }

        void AddButton(EntriesTab tab, Page page, in int currentButtonPosition, in int currentButtonRow)
        {
            float y = Screen.height - ButtonSpacing - (currentButtonRow * (ButtonSizeY + ButtonSpacing));

            SimpleButton button = new SimpleButton(this, page, tab.Name, string.Concat(ReadEntryID, tab.Name), new Vector2(currentButtonPosition, y), new Vector2(ButtonSizeX, ButtonSizeY));
            page.subObjects.Add(button);
        }

        /// <summary>
        /// Gets all the first chars after a space or any other white space character
        /// </summary>
        string GetInitials(string s)
        {
            string result = string.Empty;

            bool previousWasWhiteSpace = true;

            foreach (char c in s)
            {
                if (char.IsWhiteSpace(c))
                {
                    previousWasWhiteSpace = true;
                }
                else
                {
                    if (previousWasWhiteSpace)
                    {
                        previousWasWhiteSpace = false;
                        result += c;
                    }
                }
            }

            return result;
        }


        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Singal(backObject, BackButtonMessage);
            }
        }

        public int GetCurrentlySelectedOfSeries(string series)
        {
            throw new NotImplementedException();
        }
        public void SetCurrentlySelectedOfSeries(string series, int to)
        {
            throw new NotImplementedException();
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
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
