using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public string firstDialogueLine;

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);
        DialogueSystem.StartDialogue(firstDialogueLine);
    }
}
