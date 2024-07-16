using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Farming
{
    [CreateAssetMenu(fileName = "NewCrop", menuName = "ScriptableObjects/Crop", order = 1)]
    public class Crop : ScriptableObject
    {
        public string Name= "Crop";

        /// <summary>
        /// Growth time in ingame Hours (?)
        /// </summary>
        public float growthTime = 168f;

        /// <summary>
        /// How much water will the crop use per hour
        /// </summary>
        public int waterNeed = 1;

        public CropInstance plantPrefab;
        public Item harvestResult;

        public int baseYield = 2;
    }

}
