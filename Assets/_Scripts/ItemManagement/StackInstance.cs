using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackInstance : Interactable
{
    public Inventory.ItemStack source;

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);
        //Put in interactors Inventory
        Inventory i = interactor.GetComponent<Inventory>();
        if(i == null)
        {
            Debug.LogError(interactor.name + " doesn't have an inventory to put " + source.count + " " + source.item.Name + " in!");
            return;
        }

        if(i.Add(new Inventory.ItemStack(source)))
        {
            Debug.Log("Added" + source.count + " " + source.item.Name + " to " + interactor.name + "'s Inventory");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log(interactor.name +"'s Inventory is too full to add" + source.count + " " + source.item.Name + "!");
            return;
        }
    }
}
