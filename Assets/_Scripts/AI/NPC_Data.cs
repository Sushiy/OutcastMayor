using OutcastMayor.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    /// <summary>
    /// NonPlayerCharacter
    /// </summary>
    [CreateAssetMenu(fileName = "NewNPC", menuName = "ScriptableObjects/NPC", order = 1)]
    public class NPC_Data : ScriptableObject
    {
        [SerializeField] private string characterName;

        [SerializeField] private RequestData[] availableRequests;

        public string CharacterName
        {
            get
            {
                return characterName;
            }
        }

        public RequestData GetQuest(int i)
        {
            return availableRequests[i];
        }
    }

}
