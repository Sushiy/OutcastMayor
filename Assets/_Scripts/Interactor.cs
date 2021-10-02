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

    private void Update()
    {
        RaycastHit raycastHit;
        if(Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out raycastHit, 10.0f))
        {
            //If this is the same collider as last time, 
            if (raycastHit.collider == hoveredCollider)
            {
                return;
            }

            //Check what we have hit
            hoveredCollider = raycastHit.collider;
            hoveredInteractable = raycastHit.collider.GetComponent<Interactable>();
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
