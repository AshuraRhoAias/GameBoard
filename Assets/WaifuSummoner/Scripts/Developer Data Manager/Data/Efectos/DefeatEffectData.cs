using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Effects
{
    // Los tipos de filtro disponibles
    public enum DefeatFilterType
    {
        SummonCondition,
        Role,
        Element
    }

    // Cada filtro concreto
    [System.Serializable]
    public class DefeatFilter
    {
        public DefeatFilterType filterType;
        public SummonCondition summonConditionFilter;
        public Role roleFilter;
        public ElementType elementFilter;
    }

    // La clase de datos del efecto, ahora con múltiples filtros
    [System.Serializable]
    public class DefeatEffectData
    {
        public Target targetType;
        public TargetSide targetSide;
        public int amount = 1;                     // Solo para Select o Random
        public List<DefeatFilter> filters =       // ⚠️ Antes era un único filterType + fields
            new List<DefeatFilter>();
        // Para Situational:
        public HighLowOption situationalHighLow;
        public StatType situationalStat;
        public TieBreaker situationalTieBreaker;
    }
}
