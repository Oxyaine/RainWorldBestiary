using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    [BepInPlugin(GUID, "Bestiary", "0.0.1")]
    internal class Main : BaseUnityPlugin
    {
        internal const string GUID = "oxyaine.bestiary";
        private bool Initialized;

        internal static new ManualLogSource Logger;
        private static RemixMenu Options;

        internal static ProcessManager.ProcessID BestiaryTabMenu => new ProcessManager.ProcessID("Bestiary_Tab_Menu", register: true);
        internal static ProcessManager.ProcessID BestiaryEntryMenu => new ProcessManager.ProcessID("Bestiary_Entry_Menu", register: true);
        internal static ProcessManager.ProcessID BestiaryReadingMenu => new ProcessManager.ProcessID("Bestiary_Reading_Menu", register: true);

        internal static InGameTranslator.LanguageID CurrentLanguage = null;
        internal static int CurrentSaveSlot = 0;

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

        internal static List<string> ActiveMods = new List<string>();

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled original, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            original(self, newlyDisabledMods);

            foreach (ModManager.Mod mod in newlyDisabledMods)
                ActiveMods.Remove(mod.id);

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

                    foreach (ModManager.Mod mod in ModManager.ActiveMods)
                        ActiveMods.Add(mod.id);

                    CurrentLanguage = self.options.language;
                    CurrentSaveSlot = self.options.saveSlot;

                    Bestiary.Load();

                    ResourceManager.Initialize();
                    CreatureHooks.Initialize();

                    MenuHooks.Initialize();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled original, RainWorld self, ModManager.Mod[] newlyEnabledMods)
        {
            original(self, newlyEnabledMods);

            foreach (ModManager.Mod mod in newlyEnabledMods)
                ActiveMods.Add(mod.id);

            StartCoroutine(ResourceManager.LoadMods(newlyEnabledMods));
        }
    }
}
