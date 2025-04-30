using System;
using System.Collections.Generic;
using UnityEngine;

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
    public Reign reignFilter;
}

/// <summary>
/// Datos para el efecto “Change Position”:
///   0) Nueva posición (WaifuPosition)
///   1) Target selection (Target)
///   2) Amount (solo para Select/Random/Situational)
///   3) Situational extras (HighLowOption, StatType, TieBreaker)
///   4) Target Side
///   5) Duration (Duration + Stages/Turns)
///   6) Filtros dinámicos
/// </summary>
[Serializable]
public class ChangePositionEffectData
{
    /// <summary>0) La posición a la que cambiamos la waifu.</summary>
    public WaifuPosition newPosition;

    /// <summary>1) Cómo seleccionamos objetivos.</summary>
    public Target target = Target.None;

    /// <summary>2) Cuántos (solo Select/Random/Situational; siempre ≥1).</summary>
    public int amount = 1;

    /// <summary>3) Extras situational (solo si target == Situational).</summary>
    public HighLowOption highLow;
    public StatType statType;
    public TieBreaker tieBreaker;

    /// <summary>4) De qué lado elegimos (Both/Enemy/User).</summary>
    public TargetSide targetSide = TargetSide.Both;

    /// <summary>5) Duración del cambio de posición.</summary>
    public Duration duration = Duration.None;
    public Stages untilStage = Stages.None;
    public int durationTurns = 1;

    /// <summary>
    /// 6) Filtros dinámicos (uno por cada ChangePositionFilterType).
    /// </summary>
    [SerializeField]
    public List<ChangePositionFilterData> filters = new List<ChangePositionFilterData>();
}
