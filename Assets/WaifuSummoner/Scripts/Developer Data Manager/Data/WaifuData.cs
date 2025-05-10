// Assets/Scripts/Data/WaifuData.cs
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Cards/Waifu Data", fileName = "NewWaifuData")]
public class WaifuData : ScriptableObject
{
    public CardType cardType;
    public Rarity rarity;           // ← Nuevo
    public string waifuName;
    public Realm reign;             // ← Nuevo

    public SummonType summonType;   // Enums/SummonType.cs
    public Role role;               // Enums/Roles.cs
    public ElementType element;     // Enums/Elements.cs

    public Sprite artwork;

    [Range(0, 5)]
    public int level;

    [Range(0, 999)]
    public int attack;

    [Range(0, 999)]
    public int ambush;

    // Ahora: grupos de efectos independientes
    public List<WaifuEffectGroup> effectGroups = new List<WaifuEffectGroup>();
}

// Define las opciones en el orden del popup: One, Two, Custom, Unlimited
public enum TimesPerTurn
{
    One,       // una vez por turno
    Two,       // dos veces por turno
    Custom,    // número personalizado
    Unlimited  // ilimitado mientras se cumpla el trigger
}

[Serializable]
public class WaifuEffectGroup
{
    // Triggers del grupo
    public List<Trigger> triggers = new List<Trigger> { Trigger.Action };

    // Cuántas veces por turno puede activarse este grupo
    public TimesPerTurn timesPerTurn = TimesPerTurn.One;

    // Usado solo si timesPerTurn == Custom
    public int customTimes = 1;

    // Subefectos en este grupo
    public List<EffectData> effects = new List<EffectData>();

    // Descripción del grupo
    [TextArea(2, 4)]
    public string effectDescription;
}
