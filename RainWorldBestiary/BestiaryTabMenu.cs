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

            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                if (tab.Name.Equals(BestiaryMenu.CurrentSelectedTab))
                {
                    CreateEntryButtonsFromTab(tab);
                    GetTabTitle(tab);

                    break;
                }
            }

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
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
                FLabel label2 = new FLabel("DisplayFont", tab.Name)
                {
                    scale = 3f,
                    x = Screen.width / 2,
                    y = Screen.height - 50
                };
                pages[0].Container.AddChild(label2);
            }
        }

        private const int ButtonSizeX = 155;
        private const int ButtonSizeY = 35;
        private const int ButtonSpacing = 15;
        private const int MaxButtonsPerRow = 8;
        public void CreateEntryButtonsFromTab(EntriesTab tab)
        {
            int currentX = ButtonSpacing;
            int currentY = Screen.height - 100;

            for (int i = 0; i < tab.Count; i++)
            {
                if (i % MaxButtonsPerRow == 0)
                {
                    currentY -= ButtonSizeY + ButtonSpacing;
                    currentX = ButtonSpacing;
                }

                SimpleButton textButton = new SimpleButton(this, pages[0], tab[i].Name, string.Concat(EntryPressedID, tab[i].Name), new Vector2(currentX, currentY), new Vector2(ButtonSizeX, ButtonSizeY));
                pages[0].subObjects.Add(textButton);

                if (Futile.atlasManager.DoesContainElementWithName(tab[i].Info.EntryIcon))
                {
                    FSprite icon = new FSprite(tab[i].Info.EntryIcon)
                    {
                        x = currentX + 5,
                        y = currentY + (ButtonSizeY / 2)
                    };
                    pages[0].Container.AddChild(icon);
                }

                currentX += ButtonSizeX + ButtonSpacing;
            }
        }

        public static string CurrentSelectedEntry = string.Empty;
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
                PlaySound(SoundID.MENU_Switch_Page_In);
                CurrentSelectedEntry = message.Substring(EntryPressedID.Length);

                manager.RequestMainProcessSwitch(Main.EntryReadingTab, Main.Options.MenuFadeTime);
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
