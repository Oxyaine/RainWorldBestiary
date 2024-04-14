using Menu;
using RainWorldBestiary.Types;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class BestiaryEntryMenu : OverlappingMenu, ISubMenuOwner
    {
        readonly SimpleButton backButton;
        readonly string BackButtonMessage = "BACK";
        readonly string EntryPressedID = "Read_Entry_";

        private bool Closing = false;

        public BestiaryEntryMenu(ProcessManager manager, ISubMenuOwner parentMenu) : base(manager, parentMenu)
        {
            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

            backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, -30f), new Vector2(220f, 30f));
            AddMovingObject(backButton, new Vector2(leftAnchor + 15f, 25f));
            backObject = backButton;

            CreateEntryButtonsFromTab(Bestiary.CurrentSelectedTab, in screenSize, out MenuObject firstEntryButton);

            MenuIllustration[] illustrations = SharedMenuUtilities.GetMenuTitleIllustration(this, pages[0], Bestiary.CurrentSelectedTab, in screenSize);
            foreach (MenuIllustration illustration in illustrations)
                AddMovingObject(illustration, new Vector2(illustration.pos.x, illustration.pos.y + screenSize.y), illustration.pos);

            if (Bestiary.EnteringMenu)
                selectedObject = firstEntryButton;
            else
                backObject.nextSelectable[0] = backButton;

            TipLabel = new MenuLabel(this, pages[0], "", new Vector2(screenSize.x / 2f, 25f), Vector2.one, false);
            pages[0].subObjects.Add(TipLabel);

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        private readonly int ButtonSizeX = 155;
        private readonly int ButtonSizeY = 35;
        private readonly int ButtonSpacing = 15;
        private readonly int MaxButtonsPerRow = 8;

        private readonly HSLColor LockedColor = new HSLColor(0f, 0.55f, 0.85f);

        public void CreateEntryButtonsFromTab(EntriesTab tab, in Vector2 screenSize, out MenuObject firstEntryButton)
        {
            float currentX = ButtonSpacing;
            float currentY = screenSize.y - 100f;

            Vector2 buttonSize = new Vector2(ButtonSizeX, ButtonSizeY);

            firstEntryButton = null;

            for (int i = 0; i < tab.Count; i++)
            {
                if (i % MaxButtonsPerRow == 0)
                {
                    currentY -= ButtonSizeY + ButtonSpacing;
                    currentX = ButtonSpacing;
                }

                bool entryLocked = !tab[i].Info.EntryUnlocked;

                SimpleButton button = new SimpleButton(this, pages[0], entryLocked ? "???" : Translator.Translate(tab[i].Name), string.Concat(EntryPressedID, tab[i].Name), new Vector2(currentX, currentY + screenSize.y), buttonSize)
                {
                    rectColor = entryLocked ? LockedColor : tab[i].Info.EntryColor
                };
                AddMovingObject(button, new Vector2(currentX, currentY));

                if (i == 0)
                    firstEntryButton = button;

                float iconOffset = 0;
                for (int j = 0; j < tab[i].Info.EntryIcons.Length; j++)
                {
                    MenuIllustration illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(tab[i].Info.EntryIcons[j]), Path.GetFileName(tab[i].Info.EntryIcons[j]),
                        new Vector2(currentX + 5f + iconOffset, currentY + (ButtonSizeY / 2f) + screenSize.y), true, true)
                    {
                        color = entryLocked ? Color.black : Color.white
                    };

                    AddMovingObject(illustration, new Vector2(currentX + 5f + iconOffset, currentY + (ButtonSizeY / 2f)));

                    iconOffset += illustration.sprite.width;
                }

                currentX += ButtonSizeX + ButtonSpacing;
            }
        }

        private readonly MenuLabel TipLabel;
        private float TipLabelAlpha;

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Closing = true;

                Bestiary.EnteringMenu = false;

                PlaySound(SoundID.MENU_Switch_Page_Out);
                CloseMenu();
                return;
            }

            if (message.StartsWith(EntryPressedID))
            {
                Bestiary.EnteringMenu = true;

                string msg = message.Substring(EntryPressedID.Length);

                foreach (Entry entry in Bestiary.CurrentSelectedTab)
                {
                    if (entry.Name.Equals(msg))
                    {
                        Bestiary.CurrentSelectedEntry = entry;
                        break;
                    }
                }

                if (Bestiary.CurrentSelectedEntry.Info.EntryUnlocked)
                {
                    PlaySound(SoundID.MENU_Switch_Page_In);

                    InSubMenu = true;
                    backButton.menuLabel.text = string.Empty;
                    manager.ShowDialog(new BestiaryReadingMenu(manager, this));
                }
                else
                {
                    PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);

                    TipLabel.text = Translator.Translate(Bestiary.CurrentSelectedEntry.Info.LockedText);
                    TipLabelAlpha = Mathf.Clamp(TipLabel.text.Length * 0.04f, 1.5f, 5f);
                }
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            TipLabel.RemoveSprites();
        }

        public override void Update()
        {
            if (TipLabelAlpha > 0)
            {
                TipLabelAlpha -= Time.deltaTime / Mathf.Clamp(TipLabelAlpha, 0f, 1f);
                TipLabel.label.color = new Color(1f, 1f, 1f, TipLabelAlpha);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !Closing && !InSubMenu)
                Singal(backObject, BackButtonMessage);

            base.Update();
        }

        private bool InSubMenu = false;
        public void ReturningToThisMenu()
        {
            InSubMenu = false;
            backButton.menuLabel.text = Translator.Translate("BACK");
        }
    }
}
