using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace RainWorldBestiary.Types
{
    // A class to hold the save data for the bestiary
    internal class BestiaryData
    {
        [JsonProperty("module_unlocks")]
        private KeyValuePair<string, List<UnlockToken>>[] _ModuleUnlocks
        {
            get => ModuleUnlocks.ToArray();
            set => ModuleUnlocks = value.ToDictionary(v => v.Key, v => v.Value);
        }
        [JsonIgnore]
        public Dictionary<string, List<UnlockToken>> ModuleUnlocks = new Dictionary<string, List<UnlockToken>>();

        [JsonProperty("creature_unlock_ids")]
        public List<string> CreatureUnlockIDs = new List<string>();

        //[JsonProperty("new_additions_ids")]
        //public List<string> NewAdditionsIDs = new List<string>();

        [JsonConstructor]
        internal BestiaryData() { }
        public BestiaryData(Dictionary<string, List<UnlockToken>> moduleUnlocks, List<string> creatureUnlockIDs)
        {
            ModuleUnlocks = moduleUnlocks;
            CreatureUnlockIDs = creatureUnlockIDs;
        }
    }
}
