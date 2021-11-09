using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NonPlayerCharacter
/// </summary>
[CreateAssetMenu(fileName = "NewNPC", menuName = "ScriptableObjects/NPC", order = 1)]
public class NPC : ScriptableObject
{
    [SerializeField] private string characterName;

    [SerializeField] private Request[] availableRequests;
    
    public string CharacterName
    {
        get
        {
            return characterName;
        }
    }

    public Request GetQuest(int i)
    {
        return availableRequests[i];
    }
}
