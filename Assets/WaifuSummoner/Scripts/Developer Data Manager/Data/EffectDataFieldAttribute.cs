using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class EffectDataFieldAttribute : Attribute
{
    public EffectType EffectType { get; }
    public EffectDataFieldAttribute(EffectType effectType)
    {
        EffectType = effectType;
    }
}