using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    [BepInPlugin(MODID, "Bestiary", "0.0.1")]
    internal class Main : BaseUnityPlugin
    {
        internal const string MODID = "oxyaine.bestiary";
        private bool Initialized;

        internal static new ManualLogSource Logger;

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
            AutoCreatureHooks.Update();
            ProgressEnumerators();
        }

        internal static List<string> ActiveMods = new List<string>();

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled original, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            original(self, newlyDisabledMods);

            foreach (ModManager.Mod mod in newlyDisabledMods)
                ActiveMods.Remove(mod.id);

            ForceCompleteEnumerator(ResourceManager.UnloadingModsEnumerator);
            ResourceManager.UnloadingModsEnumerator = StartEnumerator(ResourceManager.UnloadMods(newlyDisabledMods));
        }


        private static readonly Dictionary<int, IEnumerator> RunningEnumerators = new Dictionary<int, IEnumerator>();
        internal static void ProgressEnumerators()
        {
            int[] keys = new int[RunningEnumerators.Count];
            RunningEnumerators.Keys.CopyTo(keys, 0);
            foreach (int value in keys)
                if (!RunningEnumerators[value].MoveNext())
                    RunningEnumerators.Remove(value);
        }
        internal static void ProgressEnumerators(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
                ProgressEnumerator(ids[i]);
        }
        internal static void ProgressEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            if (RunningEnumerators.TryGetValue(id, out IEnumerator enumerator))
                if (!enumerator.MoveNext())
                    RunningEnumerators.Remove(id);
        }
        internal static void ForceCompleteEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            if (RunningEnumerators.TryGetValue(id, out IEnumerator enumerator))
            {
                while (enumerator.MoveNext()) { }
                RunningEnumerators.Remove(id);
            }
        }
        internal static void ForceCompleteEnumerators(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
                ForceCompleteEnumerator(ids[i]);
        }
        internal static int StartEnumerator(IEnumerator enumerator)
        {
            int id = 0;
            while (RunningEnumerators.ContainsKey(id))
                ++id;

            RunningEnumerators.Add(id, enumerator);
            return id;
        }
        internal static void StopEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            RunningEnumerators.Remove(id);
        }


        [Obsolete]
        internal static Func<IEnumerator, Coroutine> StartCoroutinePtr;
        [Obsolete]
        internal static Action<Coroutine> StopCoroutinePtr;

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit original, RainWorld self)
        {
            original(self);
            try
            {
                MachineConnector.SetRegisteredOI("oxyaine.bestiary", new RemixMenu());

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
                    AutoCreatureHooks.Initialize();
                    ManualCreatureHooks.Initialize();

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

            ForceCompleteEnumerator(ResourceManager.LoadingModsEnumerator);
            ResourceManager.LoadingModsEnumerator = StartEnumerator(ResourceManager.LoadMods(newlyEnabledMods));
        }
    }
}
