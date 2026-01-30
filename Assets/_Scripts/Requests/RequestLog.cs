using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Requests
{
    public class RequestLog : MonoBehaviour
    {
        public Dictionary<string, Request> activeRequests;

        public Dictionary<string, Request> completedRequests;

        public void AddQuest(Request _request)
        {
            if(activeRequests == null)
            {
                activeRequests = new Dictionary<string, Request>();
            }
            activeRequests.Add(_request.RequestData.questID, _request);
            _request.OnRequestUpdated += OnQuestCompleted;
        }

        void OnQuestCompleted(Request _request)
        {
            activeRequests.Remove(_request.RequestData.questID);
            
            if(completedRequests == null)
                completedRequests = new Dictionary<string, Request>();

            completedRequests.Add(_request.RequestData.questID, _request);
            _request.OnRequestUpdated -= OnQuestCompleted;
        }

        public void CheckActiveQuest(Request _request)
        {
            if(activeRequests.ContainsKey(_request.RequestData.questID))
                activeRequests[_request.RequestData.questID].CheckGoals();
        }
    }
}
