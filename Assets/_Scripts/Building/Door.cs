using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace OutcastMayor.Interaction
{
    public class Door : Interactable
    {
        private bool isOpen = false;
        [SerializeField]
        Vector3 axis = Vector3.forward;
        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            //1. Find out which side the interactor is coming from
            Vector3 fromInteractor = transform.position - interactor.transform.position;
            float sign = Mathf.Sign(Vector3.Dot(fromInteractor, transform.parent.right));

            //2. Turn off collision until the rotation has finished

            //3. Open the door by swinging it away from the interactor
            if (isOpen)
            {
                transform.DOLocalRotate(new Vector3(0, 0, 0), 1.0f);
            }
            else
            {
                transform.DOLocalRotate(axis * 100 * -sign, 1.0f);
            }
            isOpen = !isOpen;
            //4. Turn Collision back on
            //5.
        }
    }

}
