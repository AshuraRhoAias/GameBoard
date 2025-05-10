using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Qué puede filtrar el ModifyStatsEffect.
/// </summary>
public enum ModifyStatsFilterType
{
    SummonCondition,
    Role,
    Element,
    Reign
}

/// <summary>
/// Un único filtro para ModifyStats.
/// </summary>
[Serializable]
public class ModifyStatsFilterData
{
    public ModifyStatsFilterType filterType;
    public SummonCondition summonConditionFilter;
    public Role roleFilter;
    public ElementType elementFilter;
    public Realm reignFilter;
}

/// <summary>
/// Datos para el efecto "Modify Stats":
///  1) Elige Stat (Attack/Ambush/Level)
///  2) Increase/Decrease
///  3) Valor (>=1)
///  4) Target mode (None/All/Select/Random/Situational)
///  5) Cantidad (solo Select/Random/Situational)
///  6) Si Situational: Highest/Lowest + Stat a comparar + TieBreaker
///  7) TargetSide
///  8) Duración (None/ThisTurn/UntilTheNext/ForNumberTurns/ForNumberOfYourTurns/WhileThisCardFaceup/Permanent)
///  9) Si UntilTheNext: Stage y Turns
/// 10) Filtros dinámicos (uno por cada ModifyStatsFilterType)
/// </summary>
[Serializable]
public class ModifyStatsEffectData
{
    public enum StatToModify { Attack, Ambush, Level }

    // 1
    public StatToModify stat = StatToModify.Attack;

    // 2
    public IncreaseDecrease action = IncreaseDecrease.Increase;

    // 3
    public int value = 1;

    // 4
    public Target targetType = Target.None;

    // 5
    public int amount = 1;

    // 6
    public HighLowOption highLow;
    public WaifuStats situationalStat;
    public TieBreaker tieBreaker;

    // 7
    public TargetSide targetSide = TargetSide.Both;

    // 8 — Duración
    public Duration duration = Duration.None;

    // 9 — Campos para cuando duration == UntilTheNext
    public Stages untilStage = Stages.None;
    public int durationTurns = 1;

    // 10
    [SerializeField]
    public List<ModifyStatsFilterData> filters = new List<ModifyStatsFilterData>();
}
