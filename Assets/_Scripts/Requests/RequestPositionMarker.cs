using System.Collections;
using System.Collections.Generic;
using OutcastMayor.UtilityAI;
using UnityEngine;

namespace OutcastMayor.Requests
{
    public class RequestPositionMarker : MonoBehaviour
    {
        [SerializeField]
        string _id = "questID_posID";
        void Start()
        {
            Blackboard.AddRequestPositionMarker(_id, this);
        }
    }
    
}
