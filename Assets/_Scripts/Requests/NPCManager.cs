using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    public class NPCManager : MonoBehaviour
    {
        private Dictionary<string, NPC> npcs;

        public static NPCManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("There are two NPCManagers");
                Destroy(this);
            }
            if (npcs == null)
            {
                npcs = new Dictionary<string, NPC>();
            }
        }

        public static void AddNPC(NPC npc)
        {
            Instance.npcs.Add(npc.CharacterName, npc);
        }

        public static NPC GetNPCByName(string name)
        {
            if(Instance.npcs.ContainsKey(name))
            {
                return Instance.npcs[name];
            }
            return null;
        }
    }

}
