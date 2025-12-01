using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using StylizedGrassDemo;
using UnityEngine;

namespace OutcastMayor
{
    public class SwingableTool : Tool
    {
        Collider _collider;
        Rigidbody _rigidbody;

        Vector3 _oldPosition;
        Vector3 _velocity;

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Equip(Character _parentCharacter)
        {
            base.Equip(_parentCharacter);
            gameObject.SetActive(true);

            //Set collider to inactive just in case
            _collider.enabled = false;
        }

        public override void OnUseToolPrimary(Character _parentCharacter)
        {
            _parentCharacter.CharacterAnimation.SetSwing();
        }

        public override void OnToolAnimationEvent(string _evt)
        {
            if(_evt == "swingStart")
            {
                SwingStart();
            }
            else if(_evt == "swingEnd")
            {
                SwingEnd();
            }
        }

        public void SwingStart()
        {
            print("OnStartSwing");
            _collider.enabled = true;
            StartCoroutine(Swinging());
        }

        public void SwingEnd()
        {
            print("OnEndSwing");
            _collider.enabled = false;
            StopCoroutine(Swinging());
        }

        IEnumerator Swinging()
        {
            while(true)
            {
                _velocity = (_collider.transform.position - _oldPosition) / Time.deltaTime;
                _oldPosition = _collider.transform.position;
                yield return new WaitForSeconds(.25f);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 10)
            {
                print("Tool: " + gameObject.name + " hit Hittable: " + other.gameObject.name);

                other.gameObject.GetComponentInParent<IHittable>().OnHit(other.ClosestPointOnBounds(_collider.transform.position), _velocity);
            }
        }
    }


}