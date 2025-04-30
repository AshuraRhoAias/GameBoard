using System;
using System.Collections.Generic;
using UnityEngine;



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
    public Reign reignFilter;
}

/// <summary>
/// Datos para el efecto “Control”, con flujo condicional y filtros dinámicos.
/// </summary>
[Serializable]
public class ControlEffectData
{
    /// <summary>1) Tipo de acción de Control (None/…)</summary>
    public Control control = Control.None;

    /// <summary>2) Cómo elegimos el objetivo (None/All/…)</summary>
    public Target target = Target.None;

    /// <summary>3) Cantidad (solo para Select/Random/Situational; siempre ≥1)</summary>
    public int amount = 1;

    /// <summary>4) Extras si target == Situational</summary>
    public HighLowOption highLow;
    public StatType statType;

    /// <summary>5) De qué lado elegimos</summary>
    public TargetSide targetSide;

    /// <summary>6) Duración del control</summary>
    public Duration duration = Duration.None;

    /// <summary>
    /// 7a) Para Duration == UntilTheNext: etapa objetivo
    /// 7b) Para ForNumberTurns / ForNumberOfYourTurns: cuántos turnos
    /// </summary>
    public Stages untilStage = Stages.None;
    public int durationTurns = 1;

    /// <summary>8) Filtros dinámicos</summary>
    [SerializeField]
    public List<ControlFilterData> filters = new List<ControlFilterData>();
}
