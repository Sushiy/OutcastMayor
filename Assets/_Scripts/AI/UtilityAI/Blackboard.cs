using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public class Blackboard : MonoBehaviour
    {
        public static List<SmartObject> smartObjects;

        private void Awake()
        {
            smartObjects = new List<SmartObject>();
        }
    }
}
