using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Requests
{
    public class RequestLog : MonoBehaviour
    {
        public List<Request> activeRequests;

        public List<Request> completedRequests;

        public void AddQuest(Request q)
        {
            activeRequests.Add(q);
        }

        public void CheckActiveQuests()
        {
            for (int i = 0; i < activeRequests.Count; i++)
            {
                //Check each active quest if it is completed/completeable

            }
        }
    }
}
