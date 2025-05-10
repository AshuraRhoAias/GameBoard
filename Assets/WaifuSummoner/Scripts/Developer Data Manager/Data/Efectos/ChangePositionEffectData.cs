using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Effects
{
    /// <summary>
    /// Tipos de filtro disponibles para ChangePosition.
    /// </summary>
    public enum ChangePositionFilterType
    {
        SummonCondition,
        Role,
        Element,
        Reign
    }

    /// <summary>
    /// Datos de un único filtro de ChangePosition.
    /// </summary>
    [Serializable]
    public class ChangePositionFilterData
    {
        public ChangePositionFilterType filterType;
        public SummonCondition summonCondition;
        public Role roleFilter;
        public ElementType elementFilter;
        public Realm reignFilter;
    }

    /// <summary>
    /// Datos para el efecto "Change Position".
    /// </summary>
    [Serializable]
    public class ChangePositionEffectData
    {
        public WaifuPosition newPosition;
        public Target target = Target.None;
        public int amount = 1;
        public HighLowOption highLow;
        public StatType statType;
        public TieBreaker tieBreaker;
        public TargetSide targetSide = TargetSide.Both;
        public Duration duration = Duration.None;
        public Stages untilStage = Stages.None;
        public int durationTurns = 1;
        [SerializeField]
        public List<ChangePositionFilterData> filters = new List<ChangePositionFilterData>();
    }
}
