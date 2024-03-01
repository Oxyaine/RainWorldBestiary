using Menu;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryMenu : Dialog, SelectOneButton.SelectOneButtonOwner
    {
        public PauseMenu owner;
        public bool InGame => owner != null;

        private readonly float leftAnchor;

        private const int ButtonSizeX = 145;
        private const int ButtonSizeY = 30;
        private const int ButtonSpacing = 8;
        private const int MaxButtonsPerRow = 5;
        private const int TextBoxSize = 600;
        private const int WordWrapChars = 85;

        const string ReadEntryID = "ReadEntry";
        const string BackButtonMessage = "BACK";

        readonly SimpleButton TextDisplay;

        public BestiaryMenu(ProcessManager manager, PauseMenu owner = null)
            : base(manager)
        {
            this.owner = owner;

            leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;
            //float rightAnchor = 1366f - leftAnchor;

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 50f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            int currentButtonPosition = ButtonSpacing;
            int currentButtonRow = 0;

            TextDisplay = new SimpleButton(this, pages[0], "", "NONE", new Vector2(Screen.width - TextBoxSize, 0), new Vector2(TextBoxSize, Screen.height));
            pages[0].subObjects.Add(TextDisplay);

            BasicEntryInfo[] allEntries = Bestiary.GetAllEntriesBasicInfo();

            //for (int i = 0; i < allEntries.Length; i++)
            //{
            //    if (i % MaxButtonsPerRow == 0)
            //    {
            //        currentButtonRow++;
            //        currentButtonPosition = ButtonSpacing;
            //    }

            //    if (allEntries[i].IsLocked)
            //    {
            //        if (!allEntries[i].HiddenIfLocked)
            //            AddButton("???", allEntries[i].Name, allEntries[i].SpriteName, in currentButtonPosition, in currentButtonRow);
            //    }
            //    else
            //        AddButton(allEntries[i].Name, allEntries[i].Name, allEntries[i].SpriteName, in currentButtonPosition, in currentButtonRow);

            //    currentButtonPosition += ButtonSizeX + ButtonSpacing;
            //}
        }

        void AddButton(string displayText, string entryName, string spriteName, in int currentButtonPosition, in int currentButtonRow)
        {
            float y = Screen.height - ButtonSpacing - (currentButtonRow * (ButtonSizeY + ButtonSpacing));

            ButtonTemplate button = new SimpleButton(this, pages[0], displayText, string.Concat(ReadEntryID, entryName), new Vector2(currentButtonPosition, y), new Vector2(ButtonSizeX, ButtonSizeY));

            pages[0].subObjects.Add(button);

            if (spriteName != null)
            {
                button = new SymbolButton(this, pages[0], spriteName, string.Concat(ReadEntryID, entryName), new Vector2(currentButtonPosition - 10, y));
                pages[0].subObjects.Add(button);
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
                if (InGame)
                {
                    manager.StopSideProcess(this);
                }
                else
                {
                    manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                }

                return;
            }

            if (message.StartsWith(ReadEntryID))
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
