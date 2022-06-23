using OutcastMayor.Interaction;
using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OutcastMayor.UI
{
    public class CraftingTableView : UIPanel
    {
        CraftingTable craftingTable;
        Inventory inventory;
        public Transform recipeButtonParent;
        public List<RecipeButton> recipeButtons;

        [Header("Recipe View")]
        public Recipe selectedRecipe;
        public TMPro.TMP_Text recipeTitle;
        public RecipeItemView[] recipeInputViews;
        public RecipeItemView recipeOutputView;

        [SerializeField]
        Button craftingButton;

        public void SetTable(CraftingTable craftingTable, Inventory inventory)
        {
            this.craftingTable = craftingTable;
            this.inventory = inventory;
            //Do i have enough buttons?

            //if i do:
            for (int i = 0; i < recipeButtons.Count; i++)
            {
                if (i < craftingTable.recipes.Length)
                {
                    Recipe r = craftingTable.recipes[i];
                    recipeButtons[i].gameObject.SetActive(true);
                    recipeButtons[i].SetData(r.Name, r.Icon);
                }
                else
                {
                    recipeButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateAvailability()
        {
            craftingButton.interactable = (selectedRecipe != null && selectedRecipe.IsValid(inventory));
        }

        public void ShowRecipe(int index)
        {
            ShowRecipe(craftingTable.recipes[index]);
        }

        public void ShowRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            recipeTitle.text = selectedRecipe.Name;

            for (int i = 0; i < recipeInputViews.Length; i++)
            {
                if (i < selectedRecipe.inputs.Length)
                {
                    recipeInputViews[i].Show();
                    recipeInputViews[i].SetData(selectedRecipe.inputs[i].item.DisplayName, selectedRecipe.inputs[i].count, inventory.GetTotalCount(selectedRecipe.inputs[i].item), selectedRecipe.inputs[i].item.icon);
                }
                else
                {
                    recipeInputViews[i].Hide();
                }
            }
            recipeOutputView.Show();
            recipeOutputView.SetData(selectedRecipe.output.item.DisplayName, selectedRecipe.output.count, selectedRecipe.output.item.icon);
            UpdateAvailability();

        }

        public void Craft()
        {
            if (selectedRecipe != null)
                craftingTable.Craft(selectedRecipe, inventory);
        }

        public override void Show()
        {
            inventory.onInventoryUpdated += UpdateAvailability;
            //Update the view, if there is a recipe already selected
            if (selectedRecipe)
            {
                ShowRecipe(selectedRecipe);
            }
            base.Show();
        }

        public override void Hide()
        {
            inventory.onInventoryUpdated -= UpdateAvailability;
            base.Hide();
        }
    }


}