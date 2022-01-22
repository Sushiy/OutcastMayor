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

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        //If the bed is not occupied and the player is not currently sleeping, sleep.
        if (!isOccupied)
        {
            StartSleeping(interactor.parentCharacter);
        }
    }

    public void StartSleeping(Character character)
    {
        isOccupied = true;
        character.CharacterAnimation.SetSleeping(true);
        character.Movement.TeleportTo(transform.position);
        character.Movement.SnapYRotation(Quaternion.Euler(0, 180, 0) * transform.rotation);
    }

    public void StopSleeping(Character character)
    {
        isOccupied = false;
        character.CharacterAnimation.SetSleeping(true);
    }
}
