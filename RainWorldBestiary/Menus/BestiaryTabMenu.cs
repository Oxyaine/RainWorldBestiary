using Menu;
using RainWorldBestiary.Menus.Manual;
using RainWorldBestiary.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class BestiaryTabMenu : Dialog, ISubMenuOwner
    {
        private readonly int ButtonSizeX = 250;
        private readonly int ButtonSizeY = 40;
        private readonly int ButtonSpacing = 10;

        readonly string BUTTON_ID = "Tab_Pressed";
        readonly string BackButtonMessage = "BACK";

        private bool Closing = false;

        private readonly string InstructionManualButtonMessage = "INSTRUCTION_MANUAL";
        public static ProcessManager.ProcessID InstructionManualMenu = new ProcessManager.ProcessID("Bestiary_Instruction_Manual_Menu", true);
        internal readonly Dictionary<InstructionManualPages, int> ManualTopics = new Dictionary<InstructionManualPages, int>
        {
            {
                InstructionManualPages.Introduction,
                1
            },
            {
                InstructionManualPages.Tabs,
                1
            },
            {
                InstructionManualPages.Entries,
                1
            },
            {
                InstructionManualPages.Unlocking,
                2
            }
        };

        public static MenuObject[] TabButtons;
        public static MenuObject BackButton;
        public static MenuObject InstructionManualButton;

        public BestiaryTabMenu(ProcessManager manager) : base(manager)
        {
            if (Bestiary.EnteringMenu && MenuResources.Instance == null)
                MenuResources.Create();

            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
            pages[0].subObjects.Add(scene);

            darkSprite.alpha = 0.5f;

            MenuIllustration menuIllustration = new MenuIllustration(this, pages[0], "illustrations\\bestiary\\titles", "Bestiary_Title", new Vector2(screenSize.x / 2, screenSize.y - 100), false, true)
            {
                color = new Color(162f, 157f, 170f),
                sprite =
                {
                    scale = 0.6f
                }
            };
            pages[0].subObjects.Add(menuIllustration);

            int X = (int)screenSize.x / 2 - ButtonSizeX / 2;
            int currentButtonY = (int)screenSize.y - ButtonSizeY - ButtonSpacing - 200;
            {
                List<MenuObject> tabs = new List<MenuObject>();
                for (int i = 0; i < Bestiary.EntriesTabs.Count; i++)
                {
                    EntriesTab tab = Bestiary.EntriesTabs[i];
                    if (tab.Count > 0 && Main.ActiveMods.ContainsAll(tab.RequiredMods))
                    {
                        tabs.Add(new SimpleButton(this, pages[0], Translator.Translate(tab.Name), string.Concat(BUTTON_ID, tab.Name), new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY)));
                        pages[0].subObjects.Add(tabs[tabs.Count - 1]);

                        currentButtonY -= ButtonSizeY + ButtonSpacing;
                    }
                }
                TabButtons = tabs.ToArray();
            }

            if (BestiarySettings.ShowManualButton.Value)
            {
                InstructionManualButton = new SimpleButton(this, pages[0], "MANUAL", InstructionManualButtonMessage, new Vector2(screenSize.x - 180, screenSize.y - 50), new Vector2(160, 30));
                pages[0].subObjects.Add(InstructionManualButton);
            }

            BackButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(X, currentButtonY), new Vector2(ButtonSizeX, ButtonSizeY));
            pages[0].subObjects.Add(BackButton);

            backObject = BackButton;

            BindButtons();

            if (Bestiary.EnteringMenu)
                selectedObject = TabButtons[0];
            else
                selectedObject = BackButton;

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }
        private static void BindButtons()
        {
            for (int i = 0; i < TabButtons.Length; i++)
            {
                TabButtons[i].nextSelectable[0] = TabButtons[i];

                if (i == 0)
                    TabButtons[i].nextSelectable[1] = BackButton;
                else
                    TabButtons[i].nextSelectable[1] = TabButtons[i - 1];

                if (BestiarySettings.ShowManualButton.Value)
                    TabButtons[i].nextSelectable[2] = InstructionManualButton;
                else
                    TabButtons[i].nextSelectable[2] = TabButtons[i];

                if (i != TabButtons.Length - 1)
                    TabButtons[i].nextSelectable[3] = TabButtons[i + 1];
                else
                    TabButtons[i].nextSelectable[3] = BackButton;
            }

            BackButton.nextSelectable[0] = BackButton;
            BackButton.nextSelectable[1] = TabButtons[TabButtons.Length - 1];
            if (BestiarySettings.ShowManualButton.Value)
                BackButton.nextSelectable[2] = InstructionManualButton;
            else
                BackButton.nextSelectable[2] = BackButton;
            BackButton.nextSelectable[3] = TabButtons[0];

            if (BestiarySettings.ShowManualButton.Value)
            {
                InstructionManualButton.nextSelectable[0] = TabButtons[0];
                InstructionManualButton.nextSelectable[3] = TabButtons[0];
                InstructionManualButton.nextSelectable[1] = BackButton;
                InstructionManualButton.nextSelectable[2] = InstructionManualButton;
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !InSubMenu && !Closing)
                Singal(backObject, BackButtonMessage);

            base.Update();
        }

        internal bool InSubMenu = false;
        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Closing = true;

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
                manager.ShowDialog(new BestiaryEntryMenu(manager, this));
            }
            else if (message.StartsWith(InstructionManualButtonMessage))
            {
                InSubMenu = true;
                PlaySound(SoundID.MENU_Player_Join_Game);
                manager.ShowDialog(new InstructionManualDialog(manager, ManualTopics, this));
            }
        }

        public void ReturningToThisMenu()
        {
            InSubMenu = false;
        }
    }

    internal interface ISubMenuOwner
    {
        void ReturningToThisMenu();
    }

    internal class OverlappingMenu : Dialog
    {
        bool opening = false, closing = false;
        private float targetAlpha;

        public readonly ISubMenuOwner owningMenu;

        protected List<MovingObject> MovingObjects = new List<MovingObject>();

        public OverlappingMenu(ProcessManager manager, ISubMenuOwner parentMenu = null) : base(manager)
        {
            opening = true;
            targetAlpha = 1f;
            owningMenu = parentMenu;
        }

        public virtual void CloseMenu()
        {
            opening = false;
            closing = true;
            targetAlpha = 0f;
        }

        public void AddMovingObject(PositionedMenuObject @object, Vector2 secondPosition)
        {
            pages[0].subObjects.Add(@object);
            MovingObjects.Add(new MovingObject(@object, @object.pos, secondPosition));
        }
        public void AddMovingObject(PositionedMenuObject @object, Vector2 firstPosition, Vector2 secondPosition)
        {
            @object.pos = firstPosition;
            AddMovingObject(@object, secondPosition);
        }

        private float lastAlpha, currentAlpha, uAlpha;
        public override void Update()
        {
            base.Update();

            lastAlpha = currentAlpha;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10);

            if (opening && currentAlpha >= 0.999f)
            {
                opening = false;
            }
            if (closing && Math.Abs(currentAlpha - targetAlpha) < 0.09f)
            {
                manager.StopSideProcess(this);
                closing = false;
            }
        }
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            if (opening || closing)
            {
                uAlpha = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastAlpha, currentAlpha, timeStacker)), 1.5f);
                darkSprite.alpha = uAlpha * 0.8f;
            }

            foreach (MovingObject @object in MovingObjects)
                @object.MenuObject.pos = Vector2.Lerp(@object.FirstPosition, @object.SecondPosition, (uAlpha < 0.999f) ? uAlpha : 1f);
        }

        public class MovingObject
        {
            public PositionedMenuObject MenuObject;
            public Vector2 FirstPosition, SecondPosition;

            public MovingObject(PositionedMenuObject value, Vector2 firstPosition, Vector2 secondPosition)
            {
                MenuObject = value;
                FirstPosition = firstPosition;
                SecondPosition = secondPosition;
            }
        }
    }
}
