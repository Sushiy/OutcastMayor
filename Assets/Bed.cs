using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bed lets Characters rest over night or when they are exhausted.
/// </summary>
public class Bed : Interactable
{
    private int sleepHash = Animator.StringToHash("bIsSleeping");

    public bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        //If the bed is not occupied and the player is not currently sleeping, sleep.
        if (!isOccupied)
        {
            StartSleeping();
        }
    }

    public void StartSleeping()
    {
        isOccupied = true;
    }

    public void StopSleeping()
    {
        isOccupied = false;
    }
}
