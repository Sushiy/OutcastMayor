using OutcastMayor.Dialogue;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Requests
{
    public class Request
    {
        [SerializeField, ReadOnly]
        RequestData requestData;
        public RequestData RequestData => requestData;

        [SerializeField]
        bool isCompleted = false;

        public bool IsCompleted => isCompleted;

        public RequestGoal[] goals;
        public Action<Request> OnRequestUpdated;

        public Request(RequestData _requestData)
        {
            requestData = _requestData;
            
            goals = new RequestGoal[requestData.goals.Count];
            for(int i = 0; i < requestData.goals.Count; i++)
            {
                goals[i] = requestData.goals[i].GetCopy();
                goals[i].Init(requestData.requester.CharacterName, CheckGoals);
            }
        }

        public void CheckGoals()
        {
            bool _isCompleted = true;
            for (int i = 0; i < goals.Length; i++)
            {
                if (!goals[i].CheckGoal())
                {
                    _isCompleted = false;
                }
                else
                {                    
                    Debug.Log($"[Request] Quest: {requestData.title} goal {i} is completed");
                }
            }

            if (_isCompleted)
                RequestCompleted();

            Debug.Log("[Request] Quest: " + requestData.title + " is completed: " + isCompleted);
            OnRequestUpdated?.Invoke(this);
        }

        public void RequestCompleted()
        {
            isCompleted = true;
            for (int i = 0; i < goals.Length; i++)
            {
                goals[i].Clear();
            }
            DialogueSystem.SetDialogueValue(requestData.CompletedDialogueValue, 2);
        }
    }
}