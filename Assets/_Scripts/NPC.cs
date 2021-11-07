using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string characterName;

    public string CharacterName
    {
        get
        {
            return characterName;
        }
    }
}
