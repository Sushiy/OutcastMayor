using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "ScriptableObjects/Quest", order = 1)]
public class Quest : ScriptableObject
{
    public NPC questgiver;

    public string title;

    public string description;

    public string reward;

    public QuestGoal[] goals;
    public bool CheckGoals()
    {
        for(int i = 0; i < goals.Length; i++)
        {
            if (!goals[i].isCompleted)
                return false;
        }
        return true;
    }

}


/// <summary>
/// This class describes a QuestCondition. This is proabably some kind of worldstate?
/// </summary>
[System.Serializable]
public class QuestGoal
{
    public QuestType goalType;

    public string description;

    public bool isCompleted;

    public int requiredAmount;

    public int currentAmount;

    public NPC targetCharacter;

    public Action OnUpdateGoal;

    public virtual bool IsGoalCompleted()
    {
        switch(goalType)
        {
            case QuestType.FreeValidRoom:
                return RoomManager.HasValidRoom();
            case QuestType.ItemDelivery:
                if (currentAmount >= requiredAmount)
                    return true;
                else
                    return false;
            case QuestType.FurnitureRequest:
                return false;
            default:
                return false;
        }
    }
}

public enum QuestType
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
