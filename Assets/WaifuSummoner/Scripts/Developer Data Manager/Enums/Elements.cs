using UnityEngine;
using System.Collections.Generic;

namespace WaifuSummoner.Elements
{
    public enum ElementType
    {
        Solar,       // Solar
        Lunar,       // Lunar
        Fire,        // Fuego
        Water,       // Agua
        Wind,        // Viento
        Earth,       // Tierra
        Ice,         // Hielo
        Lightning,   // Trueno
        Eclypse,     // Maldad
        Stellar      // Bien
    }

    public static class ElementTypeDescriptions
    {
        public static readonly Dictionary<ElementType, string> Descriptions = new Dictionary<ElementType, string>
        {
            { ElementType.Solar,      "Shine so bright that even shadows find purpose." },
            { ElementType.Lunar,      "Reflection shapes the world in quiet grace." },
            { ElementType.Stellar,    "The stars call; only the worthy listen." },
            { ElementType.Fire,       "Desire feeds both creation and destruction." },
            { ElementType.Water,      "Become what is needed, as water becomes all things." },
            { ElementType.Wind,       "Freedom belongs to those who cannot be caught." },
            { ElementType.Earth,      "Endure the weight of all things — and remain." },
            { ElementType.Ice,        "Power held back is the sharpest blade." },
            { ElementType.Lightning,  "Doubt ends where the strike begins." },
            { ElementType.Eclypse,    "An Eclipse does not hide the light — it devours hope itself." }
        };

        /// <summary>
        /// Returns the description of the given ElementType.
        /// </summary>
        public static string GetDescription(ElementType element)
        {
            return Descriptions.TryGetValue(element, out var description) ? description : "No description available.";
        }
    }
}
