using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    public class Interactor : MonoBehaviour
    {
        public Transform rayCastOrigin;

        Collider hoveredCollider;
        public Interactable hoveredInteractable;

        public Character parentCharacter
        {
            get;
            private set;
        }

        public void SetParentCharacter(Character character)
        {
            parentCharacter = character;
        }

        public void Interact()
        {
            if (hoveredInteractable)
            {
                hoveredInteractable.Interact(this);
            }
        }

        public void ProcessRayCast(bool raycastHit, RaycastHit hitInfo)
        {
            Interactable previousInteractable = hoveredInteractable;
            if (raycastHit && hitInfo.collider.gameObject.layer == LayerConstants.Interactable)
            {
                //If this is the same collider as last time, 
                if (hitInfo.collider == hoveredCollider)
                {
                    return;
                }

                //Check what we have hit
                hoveredCollider = hitInfo.collider;
                hoveredInteractable = hitInfo.collider.GetComponentInParent<Interactable>();

                //print("Hovered = " + (hoveredInteractable != null) + " previous = " + (previousInteractable != null));
                if (hoveredInteractable != null && hoveredInteractable.isActiveAndEnabled)
                {
                    //Start hovering on the next interactable
                    hoveredInteractable.OnStartHover(this);
                }
                else
                {
                    //if you hovered an interactable last frame, stop
                    if (previousInteractable != null)
                    {
                        previousInteractable.OnEndHover(this);
                    }
                }
            }
            else
            {
                if (previousInteractable != null)
                {
                    hoveredInteractable.OnEndHover(this);
                    hoveredCollider = null;
                    hoveredInteractable = null;
                }
            }
        }
    }
}