using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
    /// <summary>
    /// Gathers the functionality of buildable interactables.
    /// </summary>
    public class BuildableInteractable : Buildable
    {
        [SerializeField]
        Collider[] interactableColliders;
        
        Interactable interactable;

        protected override void Awake()
        {
            base.Awake();
            interactable = GetComponent<Interactable>();
        }

        public override void SetBlueprintMode(Material ghostMaterial)
        {
            base.SetBlueprintMode(ghostMaterial);
            if(interactable)
            {
                interactable.Disable();
                foreach(Collider c in interactableColliders)
                {
                    c.enabled = false;
                }
            }
        }

        public override void OnBlueprintCompleted()
        {
            base.OnBlueprintCompleted();
            if(interactable)
            {
                interactable.Enable();
            }
            foreach(Collider c in interactableColliders)
            {
                c.enabled = true;
                c.gameObject.layer = LayerConstants.Interactable;
            }
        }
    }
}