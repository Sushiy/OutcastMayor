using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
    public List<Quest> activeQuests;

    public List<Quest> completedQuests;

    public void AddQuest(Quest q)
    {
        activeQuests.Add(q);
    }

    public void CheckActiveQuests()
    {
        for(int i = 0; i < activeQuests.Count; i++)
        {
            //Check each active quest if it is completed/completeable
        }
    }
}
