using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using UnityEngine;

namespace RainWorldBestiary
{
    [BepInPlugin(GUID, "Bestiary", "0.0.1")]
    internal class Main : BaseUnityPlugin
    {
        internal const string GUID = "oxyaine.bestiary";
        private bool Initialized;

        internal static new ManualLogSource Logger;
        public static RemixMenu Options;

        internal static ProcessManager.ProcessID BestiaryMenu => new ProcessManager.ProcessID("Bestiary", register: true);
        internal static ProcessManager.ProcessID BestiaryTabMenu => new ProcessManager.ProcessID("BestiaryTab", register: true);
        internal static ProcessManager.ProcessID EntryReadingMenu => new ProcessManager.ProcessID("EntryReadingTab", register: true);

        internal static InGameTranslator.LanguageID CurrentLanguage = null;

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

        internal void Update()
        {
            CreatureHooks.Update();
        }

        const string MoreSlugCatsID = "moreslugcats";

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled original, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            original(self, newlyDisabledMods);
            CheckBestiaryDependencies();
            StartCoroutine(ResourceManager.UnloadMods(newlyDisabledMods));
        }

        public static Func<IEnumerator, Coroutine> StartCoroutinePtr; 
        public static Action<Coroutine> StopCoroutinePtr;
        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit original, RainWorld self)
        {
            original(self);
            try
            {
                MachineConnector.SetRegisteredOI("oxyaine.bestiary", Options = new RemixMenu());

                StartCoroutinePtr = StartCoroutine;
                StopCoroutinePtr = StopCoroutine;

                if (!Initialized)
                {
                    Initialized = true;

                    CurrentLanguage = self.options.language;

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
            StartCoroutine(ResourceManager.LoadMods(newlyEnabledMods));
        }
    }
}
