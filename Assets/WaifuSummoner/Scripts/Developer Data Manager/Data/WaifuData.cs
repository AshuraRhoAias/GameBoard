// Assets/Scripts/Data/WaifuData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Waifu Data", fileName = "NewWaifuData")]
public class WaifuData : ScriptableObject
{
    public CardType cardType;
    public Rarity rarity;           // ← Nuevo
    public string waifuName;
    public Reign reign;             // ← Nuevo

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

    // Efectos de la carta:
    public EffectData[] effects;
}
