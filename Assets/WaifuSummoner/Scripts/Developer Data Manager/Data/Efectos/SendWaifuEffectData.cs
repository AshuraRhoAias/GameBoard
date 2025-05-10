using System.Collections.Generic;
using UnityEngine;

public enum SendFilterType
{
    SummonCondition,
    Role,
    Element
}

[System.Serializable]
public class SendFilter
{
    public SendFilterType filterType;
    public SummonCondition summonCondition;
    public Role roleFilter;
    public ElementType elementFilter;
}

[System.Serializable]
public class SendWaifuEffectData
{
    public Location location;
    public Target selectionType;
    public TargetSide targetSide;
    public int amount = 1;
    public List<SendFilter> filters = new List<SendFilter>();
    public HighLowOption highLow;
    public WaifuStats waifuStats;
    public TieBreaker tieBreaker;
}
