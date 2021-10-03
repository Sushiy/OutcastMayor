using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform rayCastOrigin;

    public HoverUIController hoverUIController;

    Collider hoveredCollider;
    Interactable hoveredInteractable;

    public void Interact()
    {
        if(hoveredInteractable)
        {
            hoveredInteractable.Interact(this);
        }
    }

    public void ProcessRayCast(bool raycastHit, RaycastHit hitInfo)
    {
        if(raycastHit)
        {
            //If this is the same collider as last time, 
            if (hitInfo.collider == hoveredCollider)
            {
                return;
            }

            //Check what we have hit
            hoveredCollider = hitInfo.collider;
            hoveredInteractable = hitInfo.collider.GetComponent<Interactable>();
            if(hoveredInteractable != null)
            {
                if(hoverUIController != null)
                {
                    hoverUIController.StartHover(hoveredInteractable);
                }
            }
            else
            {
                hoverUIController.EndHover();
            }
        }
        else
        {
            hoveredCollider = null;
            hoverUIController.EndHover();
        }
    }
}
