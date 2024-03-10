using Menu;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
        const int WrapCount = 180;

        const string BackButtonMessage = "BACK";
        public EntryReadingMenu(ProcessManager manager) : base(manager)
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
                alpha = 0.90f
            };
            pages[0].Container.AddChild(darkSprite);

            SimpleButton backButton = new SimpleButton(this, pages[0], Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            DisplayEntryInformation(BestiaryTabMenu.CurrentSelectedEntry);
        }

        public void DisplayEntryInformation(Entry entry)
        {
            float widthOffset;

            if (entry.Info.TitleSprite != null && Futile.atlasManager.DoesContainElementWithName(entry.Info.TitleSprite.ElementName))
            {
                FSprite sprite = new FSprite(entry.Info.TitleSprite.ElementName)
                {
                    scale = 0.3f * entry.Info.TitleSprite.Scale,
                    x = Screen.width / 2 + entry.Info.TitleSprite.XOffset,
                    y = Screen.height - 50 + entry.Info.TitleSprite.YOffset
                };
                pages[0].Container.AddChild(sprite);

                widthOffset = sprite.width / 2f;
            }
            else
            {
                GeneratedFontText fontText = ResourceManager.CustomFonts[0].Generate(entry.Name);

                fontText.X = (Screen.width / 2f) - (fontText.TotalWidth / 2f);
                fontText.Y = Screen.height - 50;

                FSprite[] sprites = fontText.Finalize();
                for (int i = 0; i < sprites.Length; i++)
                    pages[0].Container.AddChild(sprites[i]);

                widthOffset = fontText.TotalWidth / 2f;
            }

            if (entry.Info.IconsNextToTitle && Futile.atlasManager.DoesContainElementWithName(entry.Info.EntryIcon))
            {
                FSprite sprite = new FSprite(entry.Info.EntryIcon)
                {
                    y = Screen.height - 50,
                    x = Screen.width / 2f - (widthOffset + 60),
                    scale = 2
                };
                pages[0].Container.AddChild(sprite);

                FSprite sprite2 = new FSprite(entry.Info.EntryIcon)
                {
                    y = Screen.height - 50,
                    x = Screen.width / 2f + (widthOffset + 10),
                    scale = 2
                };
                pages[0].Container.AddChild(sprite2);
            }

            for (int i = 0; i < entry.Info.Description.Count; i++)
            {
                FSprite pip = new FSprite(entry.Info.Description[i].ModuleLocked ? "illustrations\\unlock_pip" : "illustrations\\unlock_pip_full")
                {
                    x = Screen.width - 20,
                    y = Screen.height - (i * 20) - 20,
                    scale = 1f
                };
                pages[0].Container.AddChild(pip);
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, Main.Options.MenuFadeTime);
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
