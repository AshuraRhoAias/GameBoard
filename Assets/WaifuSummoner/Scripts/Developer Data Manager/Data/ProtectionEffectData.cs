using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Data
{
    /// <summary>
    /// Tipos de filtro disponibles para Protection.
    /// </summary>
    public enum ProtectionFilterType
    {
        SummonCondition,
        Role,
        Element,
        Reign
    }

    /// <summary>
    /// Opciones de protección según el target seleccionado.
    /// </summary>
    public enum ProtectionOption
    {
        EffectIndestructible,
        CannotBeTargeted,
        BattleIndestructible,
        PreventBattleDamage,
        LibidoDamageImmunity,
        HandManipulationImmunity,
        DeckManipulationImmunity
    }

    /// <summary>
    /// Un único filtro para Protection.
    /// </summary>
    [Serializable]
    public class ProtectionFilterData
    {
        public ProtectionFilterType filterType;
        public SummonCondition summonCondition;
        public Role roleFilter;
        public ElementType elementFilter;
        public Reign reignFilter;
    }

    /// <summary>
    /// Datos para el efecto “Protection”, con flujo condicional y filtros dinámicos.
    /// </summary>
    [Serializable]
    public class ProtectionEffectData
    {
        public ProtectionTarget protectionTarget = ProtectionTarget.None;

        [SerializeField]
        public List<ProtectionOption> options = new List<ProtectionOption>();

        public Target cardTarget = Target.None;
        public TargetSide targetSide = TargetSide.Both;
        public int amount = 1;
        public HighLowOption highLow;
        public StatType situationalStat;
        public TieBreaker tieBreaker;

        public Duration duration = Duration.None;
        public Stages untilStage = Stages.None;
        public int durationTurns = 1;

        [SerializeField]
        public List<ProtectionFilterData> filters = new List<ProtectionFilterData>();

        [SerializeField]
        public List<ProtectionFilterData> waifuFilters = new List<ProtectionFilterData>();
    }
}
