using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace RainWorldBestiary
{
    [BepInPlugin(GUID, "Bestiary", "0.0.1")]
    internal class Main : BaseUnityPlugin
    {
        internal const string GUID = "oxyaine.bestiary";
        private bool IsInit;

        internal static new ManualLogSource Logger;
        public static RemixMenu Options;

        internal static ProcessManager.ProcessID BestiaryMenu => new ProcessManager.ProcessID("Bestiary", register: true);
        internal static ProcessManager.ProcessID BestiaryTabMenu => new ProcessManager.ProcessID("BestiaryTab", register: true);
        internal static ProcessManager.ProcessID EntryReadingMenu => new ProcessManager.ProcessID("EntryReadingTab", register: true);

        internal void Awake()
        {
            try
            {
                Logger = base.Logger;

                On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;
                On.RainWorld.OnModsInit += RainWorld_OnModsInit;
                On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
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
                MachineConnector.SetRegisteredOI("oxyaine.bestiary", Options = new RemixMenu());

                if (!IsInit)
                {
                    IsInit = true;

                    ResourceManager.Initialize();

                    CreatureHooks.Initialize();

                    MenuHooks.Initialize();
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
    }
}
