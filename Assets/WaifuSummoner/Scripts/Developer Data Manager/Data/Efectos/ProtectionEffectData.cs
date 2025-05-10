// Assets/WaifuSummoner/Scripts/Developer Data Manager/Data/ProtectionEffectData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

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
    // Para “AnyCard” y “Enchantment/Mood”
    EffectIndestructible,
    CannotBeTargeted,

    // Sólo para “Waifu”
    BattleIndestructible,
    PreventBattleDamage,

    // Sólo para “Opponent/User/…”
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
    public Realm reignFilter;
}

/// <summary>
/// Datos para el efecto “Protection”, con flujo condicional y filtros dinámicos.
/// </summary>
[Serializable]
public class ProtectionEffectData
{
    /// <summary>1) Qué vamos a proteger.</summary>
    public ProtectionTarget protectionTarget = ProtectionTarget.None;

    /// <summary>2) Opciones de protección (una lista de ProtectionOption).</summary>
    [SerializeField]
    public List<ProtectionOption> options = new List<ProtectionOption>();

    // — A partir de aquí, solo si estamos protegiendo cartas —

    /// <summary>3) Cómo elegimos las cartas a proteger.</summary>
    public Target cardTarget = Target.None;

    /// <summary>4) De qué lado elegimos (Both/Enemy/User).</summary>
    public TargetSide targetSide = TargetSide.Both;

    /// <summary>5) Cantidad (solo para Select/Random/Situational; siempre ≥1).</summary>
    public int amount = 1;

    /// <summary>6) Extras situational (solo si cardTarget == Situational).</summary>
    public HighLowOption highLow;
    public StatType situationalStat;
    public TieBreaker tieBreaker;

    // — Duración del efecto (aplica siempre) —

    /// <summary>7) Duración del efecto.</summary>
    public Duration duration = Duration.None;

    /// <summary>8) Si duration == UntilTheNext, hasta qué etapa.</summary>
    public Stages untilStage = Stages.None;

    /// <summary>9) Número de turnos (para ForNumberTurns, ForNumberOfYourTurns o UntilTheNext).</summary>
    public int durationTurns = 1;

    // — Filtros dinámicos (solo para cartas) —

    /// <summary>10) Filtros adicionales (uno por cada ProtectionFilterType).</summary>
    [SerializeField]
    public List<ProtectionFilterData> filters = new List<ProtectionFilterData>();

    // — Filtros específicos para Waifu (solo si protectionTarget == Waifu) —

    /// <summary>11) Filtros de SummonCondition, Role, Element y Reign para Waifu.</summary>
    [SerializeField]
    public List<ProtectionFilterData> waifuFilters = new List<ProtectionFilterData>();
}
