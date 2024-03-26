using Menu;
using System;
using System.Collections;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
        readonly int WrapCount = 180;

        readonly string BackButtonMessage = "BACK";
        public EntryReadingMenu(ProcessManager manager) : base(manager)
        {
            try
            {
                Vector2 screenSize = manager.rainWorld.options.ScreenSize;

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

                DisplayEntryInformation(Bestiary.CurrentSelectedEntry, in screenSize);

                mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex);
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();

            if (routine != null)
                Main.StopCoroutinePtr(routine);
        }

        Coroutine routine = null;
        IEnumerator PerformTextAnimation(string text, Vector2 screenSize)
        {
            int characters = text.Length / 100;

            string[] elements = ResourceManager.Characters.GetRandom(characters);
            FSprite[] sprites = new FSprite[characters];

            int position = characters * 16 * 3;

            float currentX = (screenSize.x / 2f) - (position / 2f), currentY = screenSize.y - 175f;
            for (int i = 0; i < elements.Length; i++)
            {
                sprites[i] = new FSprite(elements[i])
                {
                    x = currentX,
                    y = currentY,
                    scale = 3f
                };

                pages[0].Container.AddChild(sprites[i]);

                currentX += sprites[i].width;

                yield return new WaitForSeconds(0.05f);
            }

            elements = null;

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.3f);
                for (int j = 0; j < sprites.Length; j++)
                    sprites[j].RemoveFromContainer();

                yield return new WaitForSeconds(0.2f);
                for (int j = 0; j < sprites.Length; j++)
                    pages[0].Container.AddChild(sprites[j]);
            }

            MenuLabel label = new MenuLabel(this, pages[0], "", new Vector2(screenSize.x / 2f, screenSize.y / 2f), Vector2.one, false);
            pages[0].subObjects.Add(label);

            const int IncreaseAmount = 10;
            int cache = text.Length,
                countBeforeIconRemoval = cache / characters,
                currentSpriteIndex = 0;
            for (int i = 0; i < cache; i += IncreaseAmount)
            {
                if (i + IncreaseAmount >= cache)
                    label.text += text.Substring(i);
                else
                    label.text += text.Substring(i, IncreaseAmount);

                if (i >= (countBeforeIconRemoval * currentSpriteIndex))
                {
                    Main.StartCoroutinePtr(FadeIconAnimation(sprites[currentSpriteIndex]));
                    ++currentSpriteIndex;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        IEnumerator FadeIconAnimation(FSprite sprite)
        {
            PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A);

            while (sprite.alpha > 0)
            {
                sprite.alpha -= Time.deltaTime * 2 / sprite.alpha;
                sprite.scale += 0.05f;

                yield return null;
            }

            sprite.RemoveFromContainer();
        }

        public void DisplayEntryInformation(Entry entry, in Vector2 screenSize)
        {
            float widthOffset, leftSpriteOffset = 60;

            if (BestiarySettings.PerformTextAnimations.Value)
            {
                routine = Main.StartCoroutinePtr(PerformTextAnimation(entry.Info.Description.ToString().WrapText(WrapCount), screenSize));
            }
            else
            {
                MenuLabel label = new MenuLabel(this, pages[0], entry.Info.Description.ToString().WrapText(WrapCount), new Vector2(screenSize.x / 2f, screenSize.y / 2f), Vector2.one, false);
                pages[0].subObjects.Add(label);
            }

            if (entry.Info.TitleSprite != null && Futile.atlasManager.DoesContainElementWithName(entry.Info.TitleSprite.ElementName))
            {
                FSprite sprite = new FSprite(entry.Info.TitleSprite.ElementName)
                {
                    scale = 0.3f * entry.Info.TitleSprite.Scale,
                    x = screenSize.x / 2f + entry.Info.TitleSprite.XOffset,
                    y = screenSize.y - 50f - entry.Info.TitleSprite.YOffset
                };
                pages[0].Container.AddChild(sprite);

                widthOffset = sprite.width / 2f;
            }
            else
            {
                GeneratedFontText fontText = ResourceManager.GetCustomFontByName("rodondo").Generate(OptionInterface.Translate(entry.Name));

                fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
                fontText.Y = screenSize.y - 50f;

                FSprite[] sprites = fontText.Finalize();
                for (int i = 0; i < sprites.Length; i++)
                    pages[0].Container.AddChild(sprites[i]);

                widthOffset = fontText.TotalWidth / 2f;
            }

            if (entry.Info.IconsNextToTitle)
            {
                float iconOffset = 0;
                for (int i = 0; i < entry.Info.EntryIcons.Length; i++)
                {
                    if (entry.Info.IconsNextToTitle && Futile.atlasManager.DoesContainElementWithName(entry.Info.EntryIcons[i]))
                    {
                        FSprite sprite = new FSprite(entry.Info.EntryIcons[i])
                        {
                            y = screenSize.y - 50,
                            x = screenSize.x / 2f - (widthOffset + leftSpriteOffset) - iconOffset,
                            scale = 2
                        };
                        pages[0].Container.AddChild(sprite);

                        FSprite sprite2 = new FSprite(entry.Info.EntryIcons[i])
                        {
                            y = screenSize.y - 50,
                            x = screenSize.x / 2f + (widthOffset + 10) + iconOffset,
                            scale = 2
                        };
                        pages[0].Container.AddChild(sprite2);

                        iconOffset += sprite.width;
                    }
                }
            }

            if (BestiarySettings.ShowModuleLockPips.Value)
            {
                for (int i = 0; i < entry.Info.Description.Count; i++)
                {
                    FSprite pip = new FSprite(entry.Info.Description[i].ModuleUnlocked ? ResourceManager.UnlockPipUnlocked : ResourceManager.UnlockPip)
                    {
                        x = screenSize.x - 20f,
                        y = screenSize.y - (i * 10) - 20f,
                        scale = 1f
                    };
                    pages[0].Container.AddChild(pip);
                }
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Bestiary.EnteringMenu = false;

                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Singal(backObject, BackButtonMessage);

            base.Update();
        }
    }
}
