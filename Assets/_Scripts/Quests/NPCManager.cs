using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private List<NPC> npcs;

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
        if(npcs == null)
        {
            npcs = new List<NPC>();
        }
    }

    public static void AddNPC(NPC npc)
    {
        Instance.npcs.Add(npc);
    }

    public static NPC GetNPCByName(string name)
    {
        for(int i = 0; i < Instance.npcs.Count; i++)
        {
            if(name == Instance.npcs[i].CharacterName)
            {
                return Instance.npcs[i];
            }
        }
        return null;
    }
}
