using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public class Blackboard : MonoBehaviour
    {
        public static List<SmartObject> smartObjects;
        
        private static Dictionary<string, Requests.RequestPositionMarker> requestPositionMarkers;

        private void Awake()
        {
            smartObjects = new List<SmartObject>();
            requestPositionMarkers = new Dictionary<string, Requests.RequestPositionMarker>();
        }

        public static void AddRequestPositionMarker(string _id, Requests.RequestPositionMarker _requestPositionMarkert)
        {
            requestPositionMarkers.Add(_id, _requestPositionMarkert);
        }

        public static Requests.RequestPositionMarker GetRequestPositionMarker(string _id)
        {
            if(requestPositionMarkers.ContainsKey(_id))
                return requestPositionMarkers[_id];
            else
                return null;
        }
    }
}
