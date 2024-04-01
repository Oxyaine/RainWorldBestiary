using Menu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class InstructionManualDialog : ManualDialog
    {
        public InstructionManualDialog(ProcessManager manager, Dictionary<InstructionManualPages, int> topics)
            : this(manager, topics.ToDictionary(v => v.Key.value, v => v.Value)) { }
        public InstructionManualDialog(ProcessManager manager, Dictionary<string, int> topics) : base(manager, topics)
        {
            currentTopic = base.topics.Keys.ElementAt(0);
            pageNumber = 0;
            GetManualPage(currentTopic, pageNumber);
            try
            {
                Update();
                GrafUpdate(0f);
            }
            catch
            {
            }
        }
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !closing)
                Singal(backObject, "CLOSE");

            base.Update();
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals("CLOSE"))
            {
                PlaySound(SoundID.MENU_Player_Unjoin_Game);
                BestiaryTabMenu.ManualOpen = false;
            }

            base.Singal(sender, message);
        }

        public override void GetManualPage(string topicString, int pageNumber)
        {
            InstructionManualPages jollyManualPages = new InstructionManualPages(topicString);
            if (currentTopicPage != null)
            {
                currentTopicPage.RemoveSprites();
                pages[1].RemoveSubObject(currentTopicPage);
            }

            if (jollyManualPages == InstructionManualPages.Introduction)
            {
                currentTopicPage = new BestiaryIntroductionPage(this, pages[1]);
            }

            if (jollyManualPages == InstructionManualPages.Tabs)
            {
                currentTopicPage = new TabsPage(this, pages[1]);
            }

            if (jollyManualPages == InstructionManualPages.Entries)
            {
                currentTopicPage = new EntriesPage(this, pages[1]);
            }

            if (jollyManualPages == InstructionManualPages.Unlocking)
            {
                switch (pageNumber)
                {
                    case 0:
                        currentTopicPage = new UnlockingPageFirst(this, pages[1]);
                        break;
                    case 1:
                        currentTopicPage = new UnlockingPageSecond(this, pages[1]);
                        break;
                }
            }

            pages[1].subObjects.Add(currentTopicPage);
        }

        public override string TopicName(string topic)
        {
            return topic.ToString().Replace("_", " ").ToUpper();
        }
    }
}