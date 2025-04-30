using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnchantFilterType
{
    Role,
    Reign,
    Element
}

[Serializable]
public class EnchantFilterData
{
    public EnchantFilterType filterType;
    public Role roleFilter;
    public Reign reignFilter;
    public ElementType elementFilter;
}

[Serializable]
public class EnchantSummonEffectData
{
    /// <summary>1) De dónde tomo la waifu a invocar.</summary>
    public Location location;

    /// <summary>2) Modo de target (None/All/Select/Random/Situational).</summary>
    public Target target;

    /// <summary>
    /// 3) Cantidad a elegir (solo si target es Select/Random/Situational).
    /// Siempre ≥1.
    /// </summary>
    public int amount = 1;

    /// <summary>
    /// 4) Extras para Situational:
    ///    • Highest/Lowest
    ///    • Stat a comparar
    ///    • Tie Breaker
    /// </summary>
    public HighLowOption highLow;
    public StatType statType;
    public TieBreaker tieBreaker;

    /// <summary>5) A qué lado invoco la carta.</summary>
    public TargetSide targetSide;

    /// <summary>6) En qué posición aparece la waifu invocada.</summary>
    public WaifuPosition waifuPosition;

    /// <summary>
    /// 7) Filtros dinámicos: uno por cada EnchantFilterType.
    ///    Puedes añadir tantos filtros como tipos existan.
    /// </summary>
    [SerializeField]
    public List<EnchantFilterData> filters = new List<EnchantFilterData>();
}
