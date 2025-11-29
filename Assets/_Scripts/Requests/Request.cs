using Sirenix.OdinInspector;
using System;
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

        public Action<Request> OnRequestCompleted;

        public Request(RequestData _requestData)
        {
            requestData = _requestData;
            requestData.OnRequestCompleted += RequestCompleted;

            requestData.Init();
        }

        public void CheckGoal()
        {
            requestData.CheckGoals();
        }

        public void RequestCompleted(bool _completed)
        {
            if(!_completed) return;
            isCompleted = true;
            OnRequestCompleted?.Invoke(this);
        }
    }
}