using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Interaction
{
    public class CraftingTable : Interactable
    {
        public Recipe[] recipes;

        public Queue<Recipe> craftingQueue;

        public UI.CraftingTableView uiView;

        private void Awake()
        {
            craftingQueue = new Queue<Recipe>();
        }

        public void Craft(Recipe recipe, Inventory inventory)
        {
            if (recipe.IsValid(inventory))
            {
                for (int i = 0; i < recipe.inputs.Length; i++)
                {
                    inventory.Delete(recipe.inputs[i]);
                }
                inventory.Add(new Inventory.ItemStack(recipe.output));
            }
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            Inventory i = interactor.GetComponent<Inventory>();
            if (i == null)
            {
                Debug.LogError(interactor.name + " doesn't have an inventory to craft with!");
                return;
            }
            uiView.SetTable(this, i);
            UI.UIManager.Instance.ToggleCraftingTable();
        }
    }

}
