using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : Interactable
{
    public Recipe[] recipes;
    public CraftingTableView uiView;

    public void Craft(Recipe recipe, Inventory inventory)
    {
        if(recipe.IsValid(inventory))
        {
            for(int i = 0; i < recipe.inputs.Length; i++)
            {
                inventory.Delete(recipe.inputs[i]);
            }
            inventory.Add(recipe.output);
        }
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);
        Inventory i = interactor.GetComponent<Inventory>();
        if(i == null)
        {
            Debug.LogError(interactor.name + " doesn't have an inventory to craft with!");
            return;
        }
        uiView.SetTable(this, i);
        UIManager.instance.ToggleCraftingTable();
    }
}
