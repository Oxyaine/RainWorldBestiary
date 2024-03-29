#if DEBUG

using Menu;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class InstructionManualMenu : Dialog
    {
        private List<TabLayout> Tabs = new List<TabLayout>();
        private int currentTabIndex = 0;
        readonly string BackButtonMessage = "BACK";

        public InstructionManualMenu(ProcessManager manager) : base(manager)
        {
            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

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
                alpha = 0.90f
            };
            pages[0].Container.AddChild(darkSprite);

            SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }
    }

    internal class TabLayout
    {
        public List<MenuObject> MenuObjects = new List<MenuObject>();
        public List<FNode> FNodes = new List<FNode>();

        public void Add(params MenuObject[] menuObjects) => MenuObjects.AddRange(menuObjects);
        public void Add(params FNode[] fNodes) => FNodes.AddRange(fNodes);
    }
}

#endif