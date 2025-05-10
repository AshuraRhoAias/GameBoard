using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipos de filtro disponibles para Stun.
/// </summary>
public enum StunFilterType
{
    SummonCondition,
    Role,
    Element,
    Reign
}

/// <summary>
/// Datos de un único filtro de Stun.
/// </summary>
[Serializable]
public class StunFilterData
{
    public StunFilterType filterType;
    public SummonCondition summonCondition;
    public Role roleFilter;
    public ElementType elementFilter;
    public Realm reignFilter;
}

/// <summary>
/// Datos para el efecto “Stun”, con flujo condicional y filtros dinámicos.
/// </summary>
[Serializable]
public class StunEffectData
{
    // 1) Cómo seleccionamos el objetivo (None/All/Select/Random/Situational)
    public Target target = Target.None;

    // 2) Cantidad (solo para Select/Random/Situational; siempre ≥1)
    public int amount = 1;

    // 3) Extras si target == Situational:
    //    • Highest/Lowest  
    //    • Stat a comparar (Attack/Level/Ambush)
    public HighLowOption highLow;
    public StatType statType;

    // 4) De qué lado elegimos
    public TargetSide targetSide;

    // 5) Duración del stun
    public Duration duration = Duration.None;

    // 6a) Para UntilTheNext: etapa objetivo
    public Stages untilStage = Stages.None;
    // 6b) Para ForNumberTurns / ForNumberOfYourTurns: cuántos turnos
    public int durationTurns = 1;

    // 7) Filtros dinámicos (uno por cada StunFilterType)
    [SerializeField]
    public List<StunFilterData> filters = new List<StunFilterData>();
}
