using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform rayCastOrigin;

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
        Interactable previousInteractable = hoveredInteractable;
        if(raycastHit)
        {
            //If this is the same collider as last time, 
            if (hitInfo.collider == hoveredCollider)
            {
                return;
            }

            //Check what we have hit
            hoveredCollider = hitInfo.collider;
            hoveredInteractable = hitInfo.collider.GetComponentInParent<Interactable>();
            if(hoveredInteractable != null)
            {
                hoveredInteractable.OnStartHover(this);
            }
            else
            {
                previousInteractable.OnEndHover(this);
            }
        }
        else
        {
            if(previousInteractable != null)
            {
                hoveredInteractable.OnEndHover(this);
                hoveredCollider = null;
                hoveredInteractable = null;
            }
        }
    }
}
