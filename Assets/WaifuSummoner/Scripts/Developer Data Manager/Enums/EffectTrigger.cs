using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Enums
{
    public enum Trigger
    {
        Action,
        Ambush,
        Strike,
        Arrival,
        Accessory,
        Battle,
        Victory,
        Disenchant,
        Mindlash,
        Revenge,
        Attack
    }

    public static class TriggerDescriptions
    {
        private static readonly Dictionary<Trigger, string> descriptions = new Dictionary<Trigger, string>
        {
            { Trigger.Action,      "Can be activated on your Summon Stage." },
            { Trigger.Ambush,      "Active on enemy attack declaration." },
            { Trigger.Strike,      "Active during the Strike Phase." },
            { Trigger.Arrival,     "Active when the card is summoned." },
            { Trigger.Accessory,   "Active when attached to a waifu." },
            { Trigger.Battle,      "Active during the battle phase." },
            { Trigger.Victory,     "Active when your waifu wins a battle." },
            { Trigger.Disenchant,  "Active in response to an enchantment." },
            { Trigger.Mindlash,    "Active in response to a waifu's effect." },
            { Trigger.Revenge,     "Active when your waifu is defeated." },
            { Trigger.Attack,      "Active when your waifu declares an attack." }
        };

        public static string GetDescription(Trigger trigger)
        {
            return descriptions.TryGetValue(trigger, out var desc) ? desc : "No description available.";
        }
    }
}
