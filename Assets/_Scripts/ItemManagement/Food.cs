using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Items
{
    [CreateAssetMenu(fileName = "NewFood", menuName = "ScriptableObjects/Food", order = 1)]
    public class Food : Item
    {
        public float nourishment = .5f;
        public Taste primaryTaste = Taste.None;
        public Taste secondaryTaste = Taste.None;
        public Trait trait = Trait.None;
        public enum Taste
        {
            None,
            Sweet,
            Savoury,
            Sour,
            Salty,
            Spicy
        }

        [System.Flags]
        public enum Trait
        {
            None = 0,
            Creamy = 1,
            Fresh = 2,
            Healthy = 4
        }
    }
}
