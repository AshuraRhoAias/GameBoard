using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class CardTargetFilter
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
public class TargetSelectionData
{
    public Target mode;
    public int quantity;
    public CardTargetFilter filter;
    public bool useSituational;
    public bool highest;
    public StatType stat;
    public TieBreaker tieBreaker;
}
