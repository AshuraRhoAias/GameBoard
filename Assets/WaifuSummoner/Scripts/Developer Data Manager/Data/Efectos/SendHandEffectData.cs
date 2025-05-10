// Assets/Scripts/Data/SendHandEffectData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SendHandEffectData
{
    /// <summary>1) Primero escogemos el Target (None/All/Select/Random/Situational)</summary>
    public Target selectionType = Target.None;

    /// <summary>2) Lado al que va la carta (Both/Enemy/User)</summary>
    public TargetSide targetSide;

    /// <summary>3) Cantidad mínima a enviar (solo para Select/Random/Situational, siempre ≥1)</summary>
    public int amount = 1;

    /// <summary>4) Ahora elegimos el Location</summary>
    public Location location;

    /// <summary>
    /// 5) Filtros dinámicos: cada elemento es un CardType.
    /// Puedes añadir uno por cada CardType disponible.
    /// </summary>
    [SerializeField]
    public List<CardType> filterCardTypes = new List<CardType>();
}
