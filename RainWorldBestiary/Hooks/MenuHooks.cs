using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RainWorldBestiary.Plugins;
using RainWorldBestiary.Menus;
using System;
using UnityEngine;

namespace RainWorldBestiary.Hooks
{
    internal class MenuHooks
    {
        public static void Initialize()
        {
            On.Menu.MainMenu.ctor += MainMenu_ctor;
            IL.Menu.MainMenu.AddMainMenuButton += MainMenu_AddMainMenuButton;
            IL.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

            On.Menu.MainMenu.Singal += MainMenu_Singal;

            On.Menu.OptionsMenu.SetCurrentlySelectedOfSeries += OptionsMenu_SetCurrentlySelectedOfSeries;
        }

        private static void OptionsMenu_SetCurrentlySelectedOfSeries(On.Menu.OptionsMenu.orig_SetCurrentlySelectedOfSeries original, OptionsMenu self, string series, int to)
        {
            original(self, series, to);

            switch (series)
            {
                case "Language":
                    Main.CurrentLanguage = self.manager.rainWorld.options.language;
                    ResourceManager.ReloadFonts();
                    break;
                case "SaveSlot":
                    Bestiary.Save();
                    Main.CurrentSaveSlot = self.manager.rainWorld.options.saveSlot;
                    Bestiary.ClearLoadedSaveData();
                    Bestiary.Load();
                    break;
            }
        }

        private static void MainMenu_Singal(On.Menu.MainMenu.orig_Singal original, MainMenu self, MenuObject sender, string message)
        {
            original(self, sender, message);

            if (message.Equals("SHOW_BESTIARY_ERRORS"))
            {
                self.manager.RequestMainProcessSwitch(ErrorManager.BestiaryErrorMenu);
            }
        }

        private static void ProcessManager_PostSwitchMainProcess(ILContext il)
        {
            ILCursor iLCursor = new ILCursor(il);
            try
            {
                if (!iLCursor.TryGotoNext(MoveType.Before, (Instruction i) => i.MatchLdarg(0), (Instruction i) => i.MatchLdfld<ProcessManager>("oldProcess"), (Instruction i) => i.MatchLdarg(0), (Instruction i) => i.MatchLdfld<ProcessManager>("currentMainLoop"), (Instruction i) => i.MatchCallOrCallvirt<MainLoopProcess>("CommunicateWithUpcomingProcess")))
                {
                    throw new Exception("Failed to match IL for ProcessManager_PostSwitchMainProcess!");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception when matching IL for ProcessManager_PostSwitchMainProcess!");
                Debug.LogException(exception);
                Debug.LogError(il);
                throw;
            }

            iLCursor.MoveAfterLabels();
            iLCursor.Emit(OpCodes.Ldarg_0);
            iLCursor.Emit(OpCodes.Ldarg_1);

            iLCursor.EmitDelegate<Action<ProcessManager, ProcessManager.ProcessID>>(delegate (ProcessManager self, ProcessManager.ProcessID ID)
            {
                if (ID == Main.BestiaryTabMenu)
                {
                    self.currentMainLoop = new BestiaryMenu(self);
                }
                else if (ID == ErrorManager.BestiaryErrorMenu)
                {
                    self.currentMainLoop = new ErrorManager(self);
                }
                else if (ID == ProcessManager.ProcessID.MainMenu)
                {
                    MenuResources.Dispose();
                }
            });
        }
        private static void MainMenu_AddMainMenuButton(ILContext il)
        {
            ILCursor iLCursor = new ILCursor(il);
            try
            {
                if (!iLCursor.TryGotoNext(MoveType.After, (Instruction i) => i.MatchLdcI4(8)))
                {
                    throw new Exception("Failed to match IL for MainMenu_ctor1!");
                }

                iLCursor.MoveAfterLabels();
                iLCursor.EmitDelegate<Func<int, int>>((int _) => 12);
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception when matching IL for MainMenu_ctor1!");
                Debug.LogException(exception);
                Debug.LogError(il);
                throw;
            }
        }
        private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor original, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            original(self, manager, showRegionSpecificBkg);

            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

            float buttonWidth = 110f;
            Vector2 pos = new Vector2(683f - buttonWidth / 2f, 0f);
            Vector2 size = new Vector2(buttonWidth, 30f);
            self.AddMainMenuButton(new SimpleButton(self, self.pages[0], Translator.Translate("BESTIARY"), "BESTIARY", pos, size), delegate
            {
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu);
                self.PlaySound(SoundID.MENU_Switch_Page_In);
            }, 2);

            if (ErrorManager.HasErrors())
            {
                SymbolButton button = new SymbolButton(self, self.pages[0], ErrorManager.GetSpriteName(ErrorManager.GetHighestErrorLevel()), "SHOW_BESTIARY_ERRORS", new Vector2(screenSize.x - 40, screenSize.y - 40));
                self.pages[0].subObjects.Add(button);
            }
        }
    }
}
