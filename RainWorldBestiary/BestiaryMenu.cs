using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryMenu : Dialog
    {
        private readonly int ButtonSizeX = 250;
        private readonly int ButtonSizeY = 40;
        private readonly int ButtonSpacing = 10;

        readonly string BUTTON_ID = "Tab_Pressed";
        readonly string BackButtonMessage = "BACK";

        public BestiaryMenu(ProcessManager manager) : base(manager)
        {
            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

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
                alpha = 0.5f
            };
            pages[0].Container.AddChild(darkSprite);

            FSprite bestiaryTitle = new FSprite(ResourceManager.BestiaryTitle)
            {
                color = new Color(162f, 157f, 170f),
                scale = 0.6f,
                x = screenSize.x / 2,
                y = screenSize.y - 100
            };
            pages[0].Container.AddChild(bestiaryTitle);

            int X = (int)screenSize.x / 2 - ButtonSizeX / 2;
            int currentButtonY = (int)screenSize.y - ButtonSizeY - ButtonSpacing - 200;
            MenuObject firstTabButton = null;
            foreach (EntriesTab tab in Bestiary.EntriesTabs)
            {
                if (tab.Count > 0 && Main.ActiveMods.ContainsAll(tab.RequiredMods))
                {
                    SimpleButton button = new SimpleButton(this, pages[0], Translator.Translate(tab.Name), string.Concat(BUTTON_ID, tab.Name), new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY));
                    pages[0].subObjects.Add(button);

                    if (firstTabButton == null)
                        firstTabButton = button;

                    currentButtonY -= ButtonSizeY + ButtonSpacing;
                }
            }

            SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY));
            pages[0].subObjects.Add(backButton);

            backObject = backButton;

            if (Bestiary.EnteringMenu)
                selectedObject = firstTabButton;
            else
                backObject.nextSelectable[0] = backButton;

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Singal(backObject, BackButtonMessage);

            base.Update();
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Bestiary.EnteringMenu = false;
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                return;
            }

            if (message.StartsWith(BUTTON_ID))
            {
                Bestiary.EnteringMenu = true;

                string msg = message.Substring(BUTTON_ID.Length);

                foreach (EntriesTab tab in Bestiary.EntriesTabs)
                {
                    if (tab.Name.Equals(msg))
                    {
                        Bestiary.CurrentSelectedTab = tab;
                        break;
                    }
                }

                PlaySound(SoundID.MENU_Switch_Page_In);
                manager.RequestMainProcessSwitch(Bestiary.CurrentSelectedTab.TabMenuProcessID, BestiarySettings.MenuFadeTimeSeconds);
            }
        }
    }
}
