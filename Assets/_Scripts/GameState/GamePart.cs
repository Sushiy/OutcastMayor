using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OutcastMayor.GameState
{
    public class GamePartBehaviour : MonoBehaviour
    {
        [SerializeField]
        string partName;
        public string PartName => partName;

        [SerializeField]
        UnityEvent OnGamePartStart;

        [SerializeField]
        Transform playerSpawnLocation;

        [SerializeField]
        NPCSpawnLocation[] npcSpawnLocations;

        [SerializeField]
        GameObject[] activateObjects;

        [System.Serializable]
        public struct NPCSpawnLocation
        {
            public string npcName;

            public Transform spawnLocation;
        }

        public void StartGamePart(bool sceneStartsHere)
        {
            if(sceneStartsHere)
            {
                Player.Instance.Movement.TeleportTo(playerSpawnLocation.position);
                Player.Instance.Movement.SnapYRotation(playerSpawnLocation.rotation);                
            }
            foreach(GameObject go in activateObjects)
            {
                go.SetActive(true);
            }
            OnGamePartStart?.Invoke(); 
        }

        public Transform GetSpawnLocation(string npcName)
        {
            Transform target = null;
            foreach(NPCSpawnLocation npcSpawn in npcSpawnLocations)
            {
                if(npcSpawn.npcName == npcName)
                {
                    target = npcSpawn.spawnLocation;
                    break;
                }
            }
            return target;
        }
    }
    
}
