using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
        const string BackButtonMessage = "BACK";
        public EntryReadingMenu(ProcessManager manager) : base(manager)
        {
            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;



            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, 0.3f);
                return;
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
