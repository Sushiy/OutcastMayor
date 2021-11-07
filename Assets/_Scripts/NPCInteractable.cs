using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public string firstDialogueLine;
    NPC npc;
    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);
        print("test");
        DialogueSystem.StartDialogue(firstDialogueLine);

    }
}
