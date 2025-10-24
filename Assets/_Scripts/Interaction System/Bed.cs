using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Interaction
{
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
            //Position the character on the bed
            character.Movement.TeleportTo(transform.position);
            character.Movement.SnapYRotation(Quaternion.Euler(0, 180, 0) * transform.rotation);
            //Set the player to sleep
            character.Sleep();
            character.OnStopSleeping += StopSleeping;
        }

        public void StopSleeping(Character character)
        {
            isOccupied = false;
            character.WakeUp();
            character.OnStopSleeping -= StopSleeping;
        }
    }


}