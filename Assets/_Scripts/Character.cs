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

        GameObject heldItemGameObject;

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
        Item heldItem;
        public Dictionary<int, (Item, GameObject)> heldItems;

        public Action<Character> OnStopSleeping;

        protected virtual void Awake()
        {
            inventory = GetComponent<Inventory>();
            interactor = GetComponent<Interactor>();
            interactor.SetParentCharacter(this);
            characterAnimation = GetComponent<CharacterAnimation>();
            movement = GetComponent<IMovement>();
            rigidbody = GetComponent<Rigidbody>();
            heldItems = new Dictionary<int, (Item, GameObject)>();
        }

        void Start()
        {            
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
            //if(heldItemSlotID == slotID) return;
            //if we are already holding an item, kick it out?
            if(heldItem != null && heldItemGameObject != null)
            {
                heldItemGameObject.SetActive(false);
                heldItemGameObject = null;
                heldItem = null;
            }

            if(inventory.slots[slotID].count > 0)
            {
                heldItemSlotID = slotID;
                heldItem = inventory.slots[slotID].item;
                if(heldItems.ContainsKey(heldItemSlotID))
                {
                    if(heldItems[heldItemSlotID].Item1 == heldItem)
                    {
                        //Use the item from the dictionary
                        heldItemGameObject = heldItems[heldItemSlotID].Item2;
                        heldItemGameObject.SetActive(true);
                    }
                    else
                    {
                        //Spawn the object
                        heldItemGameObject = Instantiate(heldItem.prefab);  
                        //Update the item dictionary
                        Destroy(heldItems[heldItemSlotID].Item2);
                        heldItems[heldItemSlotID] = (heldItem, heldItemGameObject);                        
                    }
                }
                else
                {
                    //Spawn the object
                    heldItemGameObject = Instantiate(heldItem.prefab);
                    //Update the item dictionary
                    heldItems.Add(heldItemSlotID, (heldItem, heldItemGameObject));                    
                }

                if(heldItem.HasTag(Item.Tag.Equippable))
                {
                    heldItemGameObject.transform.parent = toolTransform;
                    heldItemGameObject.transform.localPosition = Vector3.zero;
                    heldItemGameObject.transform.localRotation = Quaternion.identity;
                    heldItemGameObject.transform.localScale = Vector3.one;
                    characterAnimation.SetCarryState(0);
                }
                else
                {
                    heldItemGameObject.transform.parent = carryTransform;
                    heldItemGameObject.transform.localPosition = Vector3.zero;
                    heldItemGameObject.transform.localRotation = Quaternion.identity;
                    heldItemGameObject.transform.localScale = Vector3.one;
                    characterAnimation.SetCarryState(1);
                }

                if(OnHeldItemChanged != null)
                {
                    OnHeldItemChanged?.Invoke(heldItemGameObject);                    
                }
            }
            else
            {

            }
        }
    }
}