using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest
/// </summary>
[CreateAssetMenu(fileName = "NewRequest", menuName = "ScriptableObjects/Request", order = 1)]
public class Request : ScriptableObject
{
    public NPC requester;

    public string title;

    public string description;

    public string reward;

    public RequestGoal[] goals;

    public bool isCompleted = false;

    public Action<Request> OnUpdateRequest;

    public string CompletedDialogueValue;

    public void Init()
    {
        for (int i = 0; i < goals.Length; i++)
        {
            goals[i].Init(CheckGoals);
        }
        CheckGoals();
    }

    public void CheckGoals()
    {
        isCompleted = true;
        for (int i = 0; i < goals.Length; i++)
        {
            goals[i].IsGoalCompleted();
            if (!goals[i].isCompleted)
            {
                isCompleted = false;
            }
        }
        Debug.Log("Quest: " + title + " is completed: " + isCompleted);
        OnUpdateRequest?.Invoke(this);
        if (isCompleted)
            Complete();
    }

    public void Complete()
    {
        for (int i = 0; i < goals.Length; i++)
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
public class RequestGoal
{
    public GoalType goalType;

    public string description;

    public bool isCompleted;

    public int requiredAmount;

    public int currentAmount;

    public NPC targetCharacter;

    public void Init(System.Action callback)
    {
        currentAmount = 0;
        isCompleted = false;
        switch (goalType)
        {
            case GoalType.FreeValidRoom:
                Debug.Log("TestCallbackstuff");
                RoomManager.instance.OnRoomValidated += callback;
                break;
            case GoalType.ItemDelivery:
                break;
            case GoalType.FurnitureRequest:
                break;
            default:
                break;
        }
    }

    public void Clear(System.Action callback)
    {
        switch (goalType)
        {
            case GoalType.FreeValidRoom:
                RoomManager.instance.OnRoomValidated -= callback;
                break;
            case GoalType.ItemDelivery:
                break;
            case GoalType.FurnitureRequest:
                break;
            default:
                break;
        }

    }

    public void IsGoalCompleted()
    {
        switch(goalType)
        {
            case GoalType.FreeValidRoom:
                isCompleted = RoomManager.HasValidRoom();
                if (isCompleted)
                    currentAmount = 1;
                break;
            case GoalType.ItemDelivery:
                if (currentAmount >= requiredAmount)
                    isCompleted = true;
                else
                    isCompleted = false;
                break;
            case GoalType.FurnitureRequest:
                isCompleted = false;
                break;
            default:
                isCompleted = false;
                break;
        }
    }
}

public enum GoalType
{
    /// <summary>
    /// Create at least 1 valid room for this character
    /// </summary>
    FreeValidRoom,
    ItemDelivery,
    /// <summary>
    /// Have the following furniture across the rooms assigned to this character.
    /// </summary>
    FurnitureRequest
}
