using System;
using UnityEditor;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EffectDrawerAttribute : Attribute
{
    public EffectType EffectType { get; }
    public string PropertyName { get; }

    /// <param name="effectType">Tipo de efecto al que aplica este Drawer.</param>
    /// <param name="propertyName">Nombre de la propiedad en EffectData (p.ej. "defeatEffect").</param>
    public EffectDrawerAttribute(EffectType effectType, string propertyName)
    {
        EffectType = effectType;
        PropertyName = propertyName;
    }
}
