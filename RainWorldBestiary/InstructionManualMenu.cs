﻿#if DEBUG

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

            MakeTabs(in screenSize);
            DisplayTabButtons(in screenSize);
            DisplayTab(Tabs[currentTabIndex]);
        }

        private void MakeTabs(in Vector2 screenSize)
        {
            TabLayout general = new TabLayout("Bestiary");
            Tabs.Add(general);



            TabLayout tabs = new TabLayout("Tabs");
            Tabs.Add(tabs);



            TabLayout entries = new TabLayout("Entries");
            Tabs.Add(entries);



            TabLayout unlocking = new TabLayout("Unlocking");
            Tabs.Add(unlocking);
        }

        private void DisplayTabButtons(in Vector2 screenSize)
        {
            float width = screenSize.x / Tabs.Count;

            for (int i = 0; i < Tabs.Count; i++)
            {
                SimpleButton button = new SimpleButton(this, pages[0], Tabs[i].Name, "TAB_" + i, new Vector2(i * width, screenSize.y - 50f), new Vector2(width, 30f));
                pages[0].subObjects.Add(button);
            }
        }

        private void Un_DisplayTab(TabLayout tab)
        {
            foreach (var v in tab.MenuObjects)
                pages[0].subObjects.Remove(v);

            foreach (FNode node in tab.FNodes)
                pages[0].Container.RemoveChild(node);
        }
        private void DisplayTab(TabLayout tab)
        {
            pages[0].subObjects.AddRange(tab.MenuObjects);

            foreach (FNode node in tab.FNodes)
                pages[0].Container.AddChild(node);
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
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
                return;
            }
        }
    }

    internal class TabLayout
    {
        public string Name = "";
        public List<MenuObject> MenuObjects = new List<MenuObject>();
        public List<FNode> FNodes = new List<FNode>();

        public void Add(params MenuObject[] menuObjects) => MenuObjects.AddRange(menuObjects);
        public void Add(params FNode[] fNodes) => FNodes.AddRange(fNodes);

        public TabLayout(string name)
        {
            Name = name;
        }
    }
}

#endif