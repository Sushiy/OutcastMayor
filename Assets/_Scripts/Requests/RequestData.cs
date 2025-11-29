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

        [Button]
        public void AddValidRoomGoal()
        {
            if(goals == null)
            {
                goals = new List<RequestGoal>();
            }
            goals.Add(new ValidRoomGoal());
        }
        [Button]
        public void AddValidPathGoal()
        {
            if(goals == null)
            {
                goals = new List<RequestGoal>();
            }
            goals.Add(new ValidRoomGoal());
        }

        public Action<bool> OnRequestCompleted;

        public string CompletedDialogueValue;

        public void Init()
        {
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].Init(requester.CharacterName, CheckGoals);
            }
            CheckGoals();
        }

        public void CheckGoals()
        {
            bool isCompleted = true;
            for (int i = 0; i < goals.Count; i++)
            {
                if (!goals[i].CheckGoal())
                {
                    isCompleted = false;
                }
            }
            Debug.Log("[Request] Quest: " + title + " is completed: " + isCompleted);
            OnRequestCompleted?.Invoke(isCompleted);
            if (isCompleted)
                Complete();
        }

        public void Complete()
        {
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].Clear(CheckGoals);
            }
            DialogueSystem.SetDialogueValue(CompletedDialogueValue, 2);
        }

    }

    /// <summary>
    /// This class describes a QuestCondition. This is proabably some kind of worldstate?
    /// </summary>
    [System.Serializable]
    public abstract class RequestGoal
    {
        public string description;
        protected bool isCompleted = false;
        public bool IsCompleted;

        protected string npcName;

        public virtual void Init(string _npcName, System.Action _callback)
        {
            isCompleted = false;
            npcName = _npcName;
        }

        public abstract void Clear(System.Action _callback);

        public abstract bool CheckGoal();
    }
}