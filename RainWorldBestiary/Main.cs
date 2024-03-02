using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace RainWorldBestiary
{
    [BepInPlugin(GUID, "Bestiary", "0.0.1")]
    internal class Main : BaseUnityPlugin
    {
        public const string GUID = "oxyaine.bestiary";
        public static DMSOptions Options;
        bool IsInit;
        bool IsPostInit;
        public static new ManualLogSource Logger;

        public static ProcessManager.ProcessID BestiaryMenu => new ProcessManager.ProcessID("Bestiary", register: true);

        private void Awake()
        {
            try
            {
                Logger = base.Logger;
                Logger.LogInfo("Loading plugin Bestiary");

                ResourceManager.Initialize();

                On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;
                On.RainWorld.OnModsInit += RainWorld_OnModsInit;
                On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;

                Logger.LogInfo("Plugin Bestiary is loaded!");
            }
            catch (Exception data)
            {
                Logger.LogError(data);
            }
        }

        const string MoreSlugCatsID = "moreslugcats";

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled original, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            original(self, newlyDisabledMods);
            CheckBestiaryDependencies();
        }
        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit original, RainWorld self)
        {
            original(self);
            try
            {
                MachineConnector.SetRegisteredOI("bestiary", Options = DMSOptions.Instance);
                if (!IsInit)
                {
                    IsInit = true;

                    On.Menu.MainMenu.ctor += MainMenu_ctor;

                    MenuHooks.Init();
                    CheckBestiaryDependencies();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static void CheckBestiaryDependencies()
        {
            Bestiary.IncludeDownpour = false;
            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (mod == null)
                    continue;

                if (mod.id.Equals(MoreSlugCatsID))
                {
                    Bestiary.IncludeDownpour = true;
                    break;
                }
            }
        }

        private void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled original, RainWorld self, ModManager.Mod[] newlyEnabledMods)
        {
            original(self, newlyEnabledMods);
            CheckBestiaryDependencies();
        }

        private void MainMenu_ctor(On.Menu.MainMenu.orig_ctor original, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            original(self, manager, showRegionSpecificBkg);
            try
            {
                if (!IsPostInit)
                {
                    IsPostInit = true;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
