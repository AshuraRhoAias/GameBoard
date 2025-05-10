using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectData
{
    // 1) La lista de triggers que usa el editor
    public List<Trigger> triggers = new List<Trigger>();

    // 2) El tipo de efecto que elige el usuario
    public EffectType effectType;

    // 3) La descripción que aparece debajo de cada drawer
    [TextArea(3, 5)]
    public string effectDescription;

    // 4) Aquí van tus datos concretos, con el mismo nombre que pasan al atributo
    [EffectDataField(EffectType.Defeat)]
    public DefeatEffectData defeatEffect;

    [EffectDataField(EffectType.SendWaifu)]
    public SendWaifuEffectData sendWaifuEffect;

    [EffectDataField(EffectType.SendHand)]
    public SendHandEffectData sendHandEffect;

    [EffectDataField(EffectType.HealBurn)]
    public HealBurnEffectData healBurnEffect;

    [EffectDataField(EffectType.ModifyStats)]
    public ModifyStatsEffectData modifyStatsEffect;

    [EffectDataField(EffectType.EnchantSummon)]
    public EnchantSummonEffectData enchantSummonEffect;

    [EffectDataField(EffectType.DrawSearch)]
    public DrawSearchEffectData drawSearchEffect;

    [EffectDataField(EffectType.Stun)]
    public StunEffectData stunEffect;

    [EffectDataField(EffectType.Control)]
    public ControlEffectData controlEffect;

    [EffectDataField(EffectType.NegateEffect)]
    public NegateEffectData negateEffect;

    [EffectDataField(EffectType.MultiAttacks)]
    public MultiAttacksEffectData multiAttacksEffect;


    [EffectDataField(EffectType.ChangePosition)]
    public ChangePositionEffectData changePositionEffect;

    [EffectDataField(EffectType.Protection)]
    public ProtectionEffectData protectionEffect;

    [EffectDataField(EffectType.Recycle)]
    public RecycleEffectData recycleEffect;

    [EffectDataField(EffectType.SummonAid)]
    public SummonAidEffectData summonAidEffect;

    [EffectDataField(EffectType.DestroySend)]
    public DestroySendEffectData destroySendEffect;

}
