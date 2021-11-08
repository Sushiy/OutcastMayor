using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public NPC questgiver;

    public string title;

    public string description;

    public string reward;

    public QuestGoal[] goals;

    /*
    public bool CheckGoals()
    {
        for(int i = 0; i < goals.Length; i++)
        {
            if(current)
        }
    }
    */

}


/// <summary>
/// This class describes a QuestCondition. This is proabably some kind of worldstate?
/// </summary>
public class QuestGoal
{
    public string description;

    public bool isCompleted;

    public int requiredAmount;

    public int currentAmount;

    public QuestType goalType;

    public virtual bool IsGoalCompleted()
    {
        if (currentAmount >= requiredAmount)
            return true;
        else
            return false;
    }

}

/// <summary>
/// Create at least 1 valid room for this character
/// </summary>
public class AssignRoomGoal : QuestGoal
{
    /// <summary>
    /// The character that should get a room assigned
    /// </summary>
    public NPC targetCharacter;

    public override bool IsGoalCompleted()
    {
        return RoomManager.DoesNPCHaveValidRoom(targetCharacter);
    }
}

/// <summary>
/// Have the following furniture across the rooms assigned to this character.
/// </summary>
public class RoomFurnitureGoal : QuestGoal
{

}

public enum QuestType
{
    Shelter,
    ItemDelivery
}
