using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Effects
{
    /// <summary>
    /// Tipos de filtro disponibles para Control.
    /// </summary>
    public enum ControlFilterType
    {
        SummonCondition,
        Role,
        Element,
        Reign
    }

    /// <summary>
    /// Datos de un único filtro de Control.
    /// </summary>
    [Serializable]
    public class ControlFilterData
    {
        public ControlFilterType filterType;
        public SummonCondition summonCondition;
        public Role roleFilter;
        public ElementType elementFilter;
        public Realm reignFilter;
    }

    /// <summary>
    /// Datos para el efecto "Control", con flujo condicional y filtros dinámicos.
    /// </summary>
    [Serializable]
    public class ControlEffectData
    {
        public Control control = Control.None;
        public Target target = Target.None;
        public int amount = 1;
        public HighLowOption highLow;
        public StatType statType;
        public TargetSide targetSide;
        public Duration duration = Duration.None;
        public Stages untilStage = Stages.None;
        public int durationTurns = 1;
        [SerializeField]
        public List<ControlFilterData> filters = new List<ControlFilterData>();
    }
}
