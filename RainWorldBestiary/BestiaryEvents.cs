using System;

namespace RainWorldBestiary
{
    /// <summary>
    /// A bunch of events from the bestiary you can hook onto to make your own logic
    /// </summary>
    public static class BestiaryEvents
    {
        /// <summary>
        /// Triggers once before any entries or tabs are loaded
        /// </summary>
        public static event Action BeforeEntriesLoaded;
        internal static void Trigger_BeforeEntriesLoaded()
        {
            if (BeforeEntriesLoaded != null)
                BeforeEntriesLoaded.Invoke();
        }

        /// <summary>
        /// Triggers once after all entries from all active mods have been loaded
        /// </summary>
        public static event Action AfterEntriesLoaded;
        internal static void Trigger_AfterEntriesLoaded()
        {
            if (AfterEntriesLoaded != null)
                AfterEntriesLoaded.Invoke();
        }

        /// <summary>
        /// Triggers whenever a new unlock token gets added to the bestiary
        /// </summary>
        public static event UnlockTokenAdded UnlockTokenAdded;
        internal static void Trigger_UnlockTokenAdded(string creatureId, UnlockTokenType tokenType, bool checkIfThisUnlocksCreature)
        {
            if (UnlockTokenAdded != null)
                UnlockTokenAdded.Invoke(creatureId, tokenType, checkIfThisUnlocksCreature);
        }
    }

    /// <summary>
    /// A delegate type representing the function that gets called when an unlock token gets added to the bestiary
    /// </summary>
    /// <inheritdoc cref="Bestiary.AddOrIncreaseModuleUnlock(string, UnlockTokenType, bool)"/>
    public delegate void UnlockTokenAdded(string creatureId, UnlockTokenType tokenType, bool checkIfThisUnlocksCreature = true);
}
