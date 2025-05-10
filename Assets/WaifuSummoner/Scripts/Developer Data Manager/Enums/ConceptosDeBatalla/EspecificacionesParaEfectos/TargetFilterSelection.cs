using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class TargetFilter
{
    public TargetSide side;
    public bool useSummonCondition;
    public SummonCondition summonCondition;

    public bool useTypeFilter;
    public CardType typeFilter;

    public bool useAttributeFilter;
    public ElementType attributeFilter;
}

[System.Serializable]
public class TargetSelection
{
    public Target mode;
    public int quantity;  // For Select/Random
    public TargetFilter filter;

    // Situational only:
    public bool useSituational;
    public bool highest;
    public StatType stat;
    public TieBreaker tieBreaker;
}
