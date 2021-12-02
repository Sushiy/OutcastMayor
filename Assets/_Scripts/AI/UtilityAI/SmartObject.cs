using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class SmartObject : MonoBehaviour
    {
        /// <summary>
        /// The advertisements this object can post
        /// </summary>
        public Advertisement[] advertisements;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    [System.Serializable]
    public class Advertisement
    {
        public Action advertisedOption;

    }

}
