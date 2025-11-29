using OutcastMayor.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Interaction
{
    [RequireComponent(typeof(NPC))]
    public class NPCInteractable : Interactable
    {
        [SerializeField] private string baseDialogueLine;
        private NPC npc;

        private void Start()
        {
            npc = GetComponent<NPC>();
            name = npc.CharacterName;
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            DialogueSystem.StartDialogue(baseDialogueLine);
        }
    }
}
