// Assets/WaifuSummoner/Scripts/Developer Data Manager/Data/DestroySendEffectData.cs
using System;
using UnityEngine;

[Serializable]
public enum DestroySendAction
{
    Destroy,    // send to void
    Send        // send to chosen location
}

[Serializable]
public class DestroySendEffectData
{
    public Target targetType;            // None/All/Select/Random/Situational/Self
    public CardType cardType;            // Waifu/Enchantment/Mood/Accessory
    public DestroySendAction actionType; // Destroy vs Send

    // — Solo si actionType == Send —
    public Location location;            // chosen location

    // — Filtro extra para encantamientos que destruyes —
    public EnchantmentPosition enchantmentPosition = EnchantmentPosition.Any;

    // — Duración —
    public Duration duration;            // enum Duration
    public int durationTurns;            // para ForNumberTurns / ForNumberOfYourTurns

    // → Para UntilTheNext:
    public Stages untilStage = Stages.None;  // etapa de turno
    public int stageCount = 1;               // cuántas de esas etapas
}
