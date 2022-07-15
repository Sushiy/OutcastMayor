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
        public float growthTime = 168;

        public GameObject plantPrefab;
        public Item harvestResult;
    }

}
