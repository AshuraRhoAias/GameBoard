using System;

public enum SummonKind
{
    None,
    TurnSummon,
    SeductionSummon
}

public enum ExtraOption
{
    None,
    Custom
}

public enum RemoveAfter
{
    CustomNumberOfSummons,
    CustomNumberOfTurns,
    Permanent
}

[Serializable]
public class SummonAidEffectData
{
    public SummonKind summonKind = SummonKind.None;

    // Turn Summon options
    public ExtraOption extraTurnSummons = ExtraOption.None;
    public int customExtraTurnSummons = 1;
    public int reduceLevelBy = 1;
    public int numberOfSummons = 1;

    // Seduction Summon options
    public ExtraOption extraSeductionSummons = ExtraOption.None;
    public int customExtraSeductionSummons = 1;
    public int reduceWaitingTurns = 1;
    public RemoveAfter removeAfter = RemoveAfter.Permanent;
    public int customRemoveCount = 1;
    public int customRemoveTurns = 1;
}
