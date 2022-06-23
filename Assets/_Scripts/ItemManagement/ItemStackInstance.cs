using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OutcastMayor.Interaction
{
    public class ItemStackInstance : Interactable
    {
        public Inventory.ItemStack source;

        public UnityAction OnPickedUp;

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            //Put in interactors Inventory
            Inventory i = interactor.GetComponent<Inventory>();
            if (i == null)
            {
                Debug.LogError(interactor.name + " doesn't have an inventory to put " + source.count + " " + source.item.DisplayName + " in!");
                return;
            }

            if (i.Add(new Inventory.ItemStack(source)))
            {
                //Debug.Log("Added" + source.count + " " + source.item.DisplayName + " to " + interactor.name + "'s Inventory");
                OnEndHover(interactor);
                Destroy(gameObject);
                OnPickedUp?.Invoke();
            }
            else
            {
                Debug.Log(interactor.name + "'s Inventory is too full to add" + source.count + " " + source.item.DisplayName + "!");
                return;
            }
        }
    }

}
