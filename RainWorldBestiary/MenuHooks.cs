using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class MenuHooks
    {
        public static void Init()
        {
            On.Menu.MainMenu.ctor += MainMenu_ctor;
            IL.Menu.MainMenu.AddMainMenuButton += MainMenu_AddMainMenuButton;
            IL.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
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
                if (ID == Main.BestiaryMenu)
                {
                    self.currentMainLoop = new BestiaryMenu(self);
                }
                else if (ID == Main.BestiaryTabMenu)
                {
                    self.currentMainLoop = new BestiaryTabMenu(self);
                }
                else if (ID == Main.EntryReadingTab)
                {
                    self.currentMainLoop = new EntryReadingMenu(self);
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

        private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor original, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            original(self, manager, showRegionSpecificBkg);
            float buttonWidth = 110f;
            Vector2 pos = new Vector2(683f - buttonWidth / 2f, 0f);
            Vector2 size = new Vector2(buttonWidth, 30f);
            self.AddMainMenuButton(new SimpleButton(self, self.pages[0], "BESTIARY", "BESTIARY", pos, size), delegate
            {
                manager.RequestMainProcessSwitch(Main.BestiaryMenu);
                self.PlaySound(SoundID.MENU_Switch_Page_In);
            }, 2);
        }
    }
}
