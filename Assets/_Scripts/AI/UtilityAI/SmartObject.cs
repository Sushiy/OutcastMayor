using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public class SmartObject : MonoBehaviour
    {
        /// <summary>
        /// The advertisements this object can post
        /// </summary>
        public Advertisement[] advertisements;

        public bool isOccupied;

        // Start is called before the first frame update
        void Start()
        {
            Blackboard.smartObjects.Add(this);
        }

        private void OnDisable()
        {
            Blackboard.smartObjects.Remove(this);
        }
    }

    [System.Serializable]
    public class Advertisement
    {
        public Action advertisedAction;

    }

}
