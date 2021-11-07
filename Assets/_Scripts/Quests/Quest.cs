using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public NPC questgiver;

    public string title;

    public string description;

    public string reward;

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

}
public enum QuestType
{
    Shelter,
    ItemDelivery
}
