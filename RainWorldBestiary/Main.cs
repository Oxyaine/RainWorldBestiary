using BepInEx;
using BepInEx.Logging;
using RainWorldBestiary.Hooks;
using RainWorldBestiary.Menus;
using RainWorldBestiary.Plugins;
using System;

namespace RainWorldBestiary
{
    [BepInPlugin(MODID, "Bestiary", "0.0.1")]
    internal sealed class Main : BaseUnityPlugin
    {
        internal const string MODID = "oxyaine.bestiary";
        private bool Initialized;

        internal static new ManualLogSource Logger;

        internal static ProcessManager.ProcessID BestiaryTabMenu => new ProcessManager.ProcessID("Bestiary_Tab_Menu", register: true);

        internal static InGameTranslator.LanguageID CurrentLanguage = null;
        internal static int CurrentSaveSlot = 0;

        internal void Awake()
        {
            try
            {
                Logger = base.Logger;

                On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            }
            catch (Exception data)
            {
                Logger.LogError(data);
            }
        }

        internal void Update()
        {
            HooksUtilities.Update();
            Enumerators.Update();

            BestiaryModManager.UpdatePlugins();
        }
        internal void FixedUpdate() => BestiaryModManager.FixedUpdatePlugins();

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit original, RainWorld self)
        {
            original(self);
            try
            {
                if (!Initialized)
                {
                    Initialized = true;

                    MachineConnector.SetRegisteredOI(MODID, new RemixMenu());

                    CurrentLanguage = self.options.language;
                    CurrentSaveSlot = self.options.saveSlot;

                    Bestiary.Load();

                    BestiaryModManager.Initialize();
                    ResourceManager.Initialize();
                    HooksUtilities.Initialize();
                    AutoCreatureHooks.Initialize();

                    MenuHooks.Initialize();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }
        }
    }
}
