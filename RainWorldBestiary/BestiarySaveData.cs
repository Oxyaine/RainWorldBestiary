﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace RainWorldBestiary
{
    internal class BestiarySaveData
    {
        [JsonProperty("module_unlocks")]
        public KeyValuePair<string, List<UnlockToken>>[] ModuleUnlocks = new KeyValuePair<string, List<UnlockToken>>[0];
        [JsonProperty("creature_unlock_ids")]
        public List<string> CreatureUnlockIDs = new List<string>();

        public BestiarySaveData(KeyValuePair<string, List<UnlockToken>>[] moduleUnlocks, List<string> creatureUnlockIDs)
        {
            ModuleUnlocks = moduleUnlocks;
            CreatureUnlockIDs = creatureUnlockIDs;
        }
        public BestiarySaveData(IEnumerable<KeyValuePair<string, List<UnlockToken>>> moduleUnlocks, List<string> creatureUnlockIDs)
        {
            ModuleUnlocks = moduleUnlocks.ToArray();
            CreatureUnlockIDs = creatureUnlockIDs;
        }
    }
}