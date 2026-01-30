using OutcastMayor.Dialogue;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Requests
{
    /// <summary>
    /// Quest Data Class
    /// IMPORTANT: This should never be stateful
    /// </summary>
    [CreateAssetMenu(fileName = "NewRequest", menuName = "ScriptableObjects/Request", order = 1)]
    public class RequestData : ScriptableObject
    {
        public string questID;
        public NPC_Data requester;

        public string title;

        public string description;

        public string reward;

        [SerializeReference]
        public List<RequestGoal> goals;

        public Action<bool> OnRequestCompleted;

        public string CompletedDialogueValue;
    }

    /// <summary>
    /// This class describes a QuestCondition. This is proabably some kind of worldstate?
    /// </summary>
    [System.Serializable]
    public abstract class RequestGoal
    {
        [SerializeField]
        protected string description;
        protected bool isCompleted = false;
        public bool IsCompleted;

        protected string npcName;
        protected System.Action checkGoalCallback;

        public virtual void Init(string _npcName, System.Action _checkGoalCallback)
        {
            isCompleted = false;
            npcName = _npcName;
            checkGoalCallback = _checkGoalCallback;
        }

        public abstract RequestGoal GetCopy();

        public abstract void Clear();

        public abstract bool CheckGoal();

        public virtual string GetGoalDescription()
        {
            return description;
        }
    }
}