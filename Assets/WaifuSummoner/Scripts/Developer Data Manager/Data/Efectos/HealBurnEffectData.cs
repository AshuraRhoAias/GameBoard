// Assets/Scripts/Data/HealBurnEffectData.cs
using System;

[Serializable]
public class HealBurnEffectData
{
    public IncreaseDecrease action = IncreaseDecrease.None;
    public TargetSide targetSide = TargetSide.User;
    public int amount = 0;
}
