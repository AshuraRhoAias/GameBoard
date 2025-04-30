using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Qué puede filtrar el NegateEffect.
/// </summary>
public enum NegateFilterType
{
    SummonCondition,
    Role,
    Element,
    Reign
}

/// <summary>
/// Un único filtro para NegateEffect.
/// </summary>
[Serializable]
public class NegateFilterData
{
    public NegateFilterType filterType;
    public SummonCondition summonCondition;
    public Role roleFilter;
    public ElementType elementFilter;
    public Reign reignFilter;
}

/// <summary>
/// Datos para el efecto “Negate”:
///  0) Dónde aplica (FieldCardType)
///  1) Cómo seleccionamos el objetivo (Target)
///  2) Cantidad (solo Select/Random/Situational)
///  3) Si Situational: Highest/Lowest + Stat a comparar
///  4) De qué lado (TargetSide)
///  5) Duración (Duration + hasta etapa / turns)
///  6) Filtros dinámicos
/// </summary>
[Serializable]
public class NegateEffectData
{
    // 0) Primera selección
    public FieldCardType fieldCardType = FieldCardType.Any;

    // 1)
    public Target target = Target.None;
    public int amount = 1;

    // 2) Situational extras
    public HighLowOption highLow;
    public StatType statType;

    // 3)
    public TargetSide targetSide = TargetSide.Both;

    // 4) Duración
    public Duration duration = Duration.None;
    public Stages untilStage = Stages.None;
    public int durationTurns = 1;

    // 5) Filtros dinámicos
    [SerializeField]
    public List<NegateFilterData> filters = new List<NegateFilterData>();
}
