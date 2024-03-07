using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryMenu : Dialog
    {
        private const int ButtonSizeX = 250;
        private const int ButtonSizeY = 40;
        private const int ButtonSpacing = 10;

        const string BUTTON_ID = "Tab_Pressed";
        const string BackButtonMessage = "BACK";

        public BestiaryMenu(ProcessManager manager) : base(manager)
        {
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
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
                alpha = 0.85f
            };
            pages[0].Container.AddChild(darkSprite);

            FSprite bestiaryTitle = new FSprite("illustrations\\Bestiary_Title")
            {
                color = new Color(162f, 157f, 170f),
                scale = 0.6f,
                x = Screen.width / 2,
                y = Screen.height - 100
            };
            pages[0].Container.AddChild(bestiaryTitle);

            int X = Screen.width / 2 - ButtonSizeX / 2;
            int currentButtonY = Screen.height - ButtonSizeY - ButtonSpacing - 200;
            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                if (tab.Name.Equals(Bestiary.DownpourTabName) && !Bestiary.IncludeDownpour)
                    continue;

                AddButton(tab, X, currentButtonY);

                currentButtonY -= ButtonSizeY + ButtonSpacing;
            }

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY));
            pages[0].subObjects.Add(backButton);

            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        void AddButton(EntriesTab tab, in int x, in int y)
        {
            SimpleButton button = new SimpleButton(this, pages[0], tab.Name, string.Concat(BUTTON_ID, tab.Name), new Vector2(x, y), new Vector2(ButtonSizeX, ButtonSizeY));
            pages[0].subObjects.Add(button);
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Singal(backObject, BackButtonMessage);
            }
        }

        public static string CurrentSelectedTab;
        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                return;
            }

            if (message.StartsWith(BUTTON_ID))
            {
                PlaySound(SoundID.MENU_Switch_Page_In);
                CurrentSelectedTab = message.Substring(BUTTON_ID.Length);

                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, 0.3f);
            }
        }
    }
}
