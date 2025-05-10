// Assets/WaifuSummoner/Scripts/Developer Data Manager/Data/DrawSearchEffectData.cs
using System;
using UnityEngine;

namespace WaifuSummoner.Effects
{
    [Serializable]
    public class DrawSearchEffectData
    {
        /// <summary>1) Draw or Search mode</summary>
        public DrawSearch mode = DrawSearch.None;
        /// <summary>
        /// 2) If mode == Search, where to look.
        /// </summary>
        public Search searchSource = Search.None;
        /// <summary>3) How many cards (always ≥1)</summary>
        public int amount = 1;
        /// <summary>
        /// 4) Where to send the drawn/searched cards.
        /// Default is UserHand.
        /// </summary>
        public Location destination = Location.UserHand;
    }
}