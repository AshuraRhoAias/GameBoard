using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaifuSummoner.Data
{
    [Serializable]
    public class RecycleEffectData
    {
        public Location sourceLocation = Location.None;
        public RecycleNumber recycleNumber = RecycleNumber.None;
        public int recycleAmount = 1;

        // --- filtros sobre lo reciclado ---
        public enum RecycleFilterType
        {
            CardType,
            Reign,
            Role,
            Element
        }

        [Serializable]
        public class RecycleFilterData
        {
            public RecycleFilterType filterType;
            public CardType cardType;
            public Reign reignFilter;
            public Role roleFilter;
            public ElementType elementFilter;
        }

        [SerializeField]
        public List<RecycleFilterData> recycleFilters = new List<RecycleFilterData>();

        // --- Después del reciclaje: dibujo ---
        public Location drawLocation = Location.None;
        public DrawMode drawMode = DrawMode.SameAsRecycled;
        public int drawAmount = 1;

        [SerializeField]
        public List<RecycleFilterData> drawFilters = new List<RecycleFilterData>();

        public enum DrawMode
        {
            SameAsRecycled,
            FixedAmount
        }

        public enum RecycleNumber
        {
            None,
            Amount,
            All
        }
    }
}
