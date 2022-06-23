using OutcastMayor.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Interaction
{
    public class NPCInteractable : Interactable
    {
        [SerializeField] private string firstDialogueLine;
        [SerializeField] private NPC npc;
        private void Start()
        {
            if (npc != null)
            {
                NPCManager.AddNPC(npc);
            }
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            print("test");
            DialogueSystem.StartDialogue(firstDialogueLine);

        }
    }
}
