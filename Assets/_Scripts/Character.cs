using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OutcastMayor
{    
    public abstract class Character : Statemachine
    {
        protected Inventory inventory;
        public Inventory Inventory => inventory;
        protected Interactor interactor;
        public Interactor Interactor => interactor;
        protected CharacterAnimation characterAnimation;
        public CharacterAnimation CharacterAnimation => characterAnimation;
        protected IMovement movement;
        public IMovement Movement => movement;

        [SerializeField]
        Transform toolTransform;
        [SerializeField]
        Transform carryTransform;

        GameObject heldItemPrefab;

        [HideInInspector]
        public UnityEvent<GameObject> OnHeldItemChanged;

        new Rigidbody rigidbody;
        public Rigidbody Rigidbody => rigidbody;

        protected bool weightedDown;
        public bool WeightedDown => weightedDown;
        public bool isSleeping
        {
            protected set;
            get;
        }

        int heldItemSlotID = 0;

        public Action<Character> OnStopSleeping;

        protected virtual void Awake()
        {
            inventory = GetComponent<Inventory>();
            interactor = GetComponent<Interactor>();
            interactor.SetParentCharacter(this);
            characterAnimation = GetComponent<CharacterAnimation>();
            movement = GetComponent<IMovement>();
            rigidbody = GetComponent<Rigidbody>();
            HoldItem(0);
        }

        public virtual void Eat(Items.Food food)
        {
            inventory.Delete(food);
        }

        public virtual void Sleep()
        {
            isSleeping = true;
            characterAnimation.SetSleeping(true);
        }

        public virtual void WakeUp()
        {
            isSleeping = false;
            characterAnimation.SetSleeping(false);
            OnStopSleeping?.Invoke(this);
        }

        public virtual void SetWeightedDown(bool _weightedDown)
        {
            weightedDown = _weightedDown;
        }

        public virtual void HoldItem(int slotID)
        {
            //if we are already holding an item, kick it out?
            if(heldItemPrefab)
            {
                Destroy(heldItemPrefab.gameObject);
                heldItemPrefab = null;
            }

            heldItemSlotID = slotID;
            Item item = inventory.slots[slotID].item;
            if(inventory.slots[slotID].count > 0)
            {
                if(item.HasTag(Item.Tag.Equippable))
                {
                    heldItemPrefab = Instantiate(item.prefab, toolTransform);
                    characterAnimation.SetCarryState(0);
                }
                else
                {
                    heldItemPrefab = Instantiate(item.prefab, carryTransform);
                    characterAnimation.SetCarryState(1);
                }
                OnHeldItemChanged?.Invoke(heldItemPrefab);
            }
            else
            {

            }
        }
    }
}