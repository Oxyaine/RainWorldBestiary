using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryTabMenu : Dialog
    {
        const string BackButtonMessage = "BACK";
        const string EntryPressedID = "Read_Entry_";

        public BestiaryTabMenu(ProcessManager manager) : base(manager)
        {
            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.SubBackground);
            pages[0].subObjects.Add(scene);

            darkSprite = new FSprite("pixel")
            {
                color = new Color(0f, 0f, 0f),
                anchorX = 0f,
                anchorY = 0f,
                scaleX = 1368f,
                scaleY = 770f,
                x = -1f,
                y = -1f,
                alpha = 0.80f
            };
            pages[0].Container.AddChild(darkSprite);

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            CreateEntryButtonsFromTab(BestiaryMenu.CurrentSelectedTab);
            GetTabTitle(BestiaryMenu.CurrentSelectedTab);

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;

            TipLabel = new MenuLabel(this, pages[0], "", new Vector2(Screen.width / 2f, 25), Vector2.one, false);
            pages[0].subObjects.Add(TipLabel);
        }

        public void GetTabTitle(EntriesTab tab)
        {
            if (tab.TitleImage != null && Futile.atlasManager.DoesContainElementWithName(tab.TitleImage.ElementName))
            {
                FSprite sprite = new FSprite(tab.TitleImage.ElementName)
                {
                    scale = 0.3f * tab.TitleImage.Scale,
                    x = Screen.width / 2 + tab.TitleImage.XOffset,
                    y = Screen.height - 50 + tab.TitleImage.YOffset
                };
                pages[0].Container.AddChild(sprite);
            }
            else
            {
                GeneratedFontText fontText = ResourceManager.CustomFonts[0].Generate(tab.Name);
                fontText.X = (Screen.width / 2f) - (fontText.TotalWidth / 2f);
                fontText.Y = Screen.height - 50;

                FSprite[] sprites = fontText.Finalize();
                for (int i = 0; i < sprites.Length; i++)
                    pages[0].Container.AddChild(sprites[i]);
            }
        }

        private const int ButtonSizeX = 155;
        private const int ButtonSizeY = 35;
        private const int ButtonSpacing = 15;
        private const int MaxButtonsPerRow = 8;

        private HSLColor LockedColor = new HSLColor(0f, 0.55f, 0.85f);
        private HSLColor UnlockedColor = new HSLColor(0.4f, 0.6f, 0.9f);

        public void CreateEntryButtonsFromTab(EntriesTab tab)
        {
            int currentX = ButtonSpacing;
            int currentY = Screen.height - 100;

            Vector2 buttonSize = new Vector2(ButtonSizeX, ButtonSizeY);

            for (int i = 0; i < tab.Count; i++)
            {
                if (i % MaxButtonsPerRow == 0)
                {
                    currentY -= ButtonSizeY + ButtonSpacing;
                    currentX = ButtonSpacing;
                }

                bool entryLocked = tab[i].Info.EntryLocked;

                SimpleButton textButton = new SimpleButton(this, pages[0], entryLocked ? "???" : tab[i].Name, string.Concat(EntryPressedID, tab[i].Name), new Vector2(currentX, currentY), buttonSize)
                {
                    rectColor = entryLocked ? LockedColor : UnlockedColor
                };
                pages[0].subObjects.Add(textButton);

                if (Futile.atlasManager.DoesContainElementWithName(tab[i].Info.EntryIcon))
                {
                    FSprite icon = new FSprite(tab[i].Info.EntryIcon)
                    {
                        color = entryLocked ? new Color(0, 0, 0, 255) : Color.white,
                        x = currentX + 5,
                        y = currentY + (ButtonSizeY / 2)
                    };
                    pages[0].Container.AddChild(icon);
                }

                currentX += ButtonSizeX + ButtonSpacing;
            }
        }

        public static Entry CurrentSelectedEntry;

        private readonly MenuLabel TipLabel;
        private float TipLabelAlpha;

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryMenu, Main.Options.MenuFadeTime);
                return;
            }

            if (message.StartsWith(EntryPressedID))
            {
                string msg = message.Substring(EntryPressedID.Length);

                foreach (Entry entry in BestiaryMenu.CurrentSelectedTab)
                {
                    if (entry.Name.Equals(msg))
                    {
                        CurrentSelectedEntry = entry;
                        break;
                    }
                }

                if (CurrentSelectedEntry.Info.EntryLocked)
                {
                    PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);

                    TipLabel.text = CurrentSelectedEntry.Info.LockedText;
                    TipLabelAlpha = 1f;
                }
                else
                {
                    PlaySound(SoundID.MENU_Switch_Page_In);
                    manager.RequestMainProcessSwitch(Main.EntryReadingTab, Main.Options.MenuFadeTime);
                }
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Singal(backObject, BackButtonMessage);

            if (TipLabelAlpha > 0)
            {
                TipLabel.label.color = new Color(1f, 1f, 1f, TipLabelAlpha);
                TipLabelAlpha -= Time.deltaTime / TipLabelAlpha;
            }

            base.Update();
        }
    }
}
