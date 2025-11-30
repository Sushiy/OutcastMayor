using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Interaction
{
    public class CartInteractable : Interactable
    {
        ConfigurableJoint joint;

        [SerializeField]
        Transform characterPosition;

        public Interactor currentInteractor;

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            if(currentInteractor == null)
            {
                currentInteractor = interactor;
                interactor.parentCharacter.Movement.TeleportTo(characterPosition.position);
                interactor.parentCharacter.Movement.SnapYRotation(characterPosition.rotation);

                joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.anchor = new Vector3(0, 0.75f, -1.75f);
                joint.connectedBody = interactor.parentCharacter.Rigidbody;
                joint.connectedAnchor = Vector3.zero;
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Limited;
                SoftJointLimit limit = joint.angularZLimit;
                limit.limit = 45;
                joint.angularZLimit = limit;
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = new Vector3(0, .8f, 0);

                interactor.parentCharacter.SetWeightedDown(true);
            }
            else if(currentInteractor == interactor)
            {
                Destroy(joint);
                joint = null;
                currentInteractor = null;
                interactor.parentCharacter.SetWeightedDown(false);
            }
        }


    }    
}