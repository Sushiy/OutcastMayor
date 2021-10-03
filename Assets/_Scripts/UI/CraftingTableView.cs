using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetTable(CraftingTable craftingTable, Inventory inventory)
    {
        this.craftingTable = craftingTable;
        this.inventory = inventory;
        //Do i have enough buttons?

        //if i do:
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            if(i < craftingTable.recipes.Length)
            {
                Recipe r = craftingTable.recipes[i];
                recipeButtons[i].gameObject.SetActive(true);
                recipeButtons[i].button.interactable = r.IsValid(inventory);
                recipeButtons[i].SetRecipe(r);
            }
            else
            {
                recipeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAvailability()
    {
        for (int i = 0; i < craftingTable.recipes.Length; i++)
        {
                recipeButtons[i].button.interactable = craftingTable.recipes[i].IsValid(inventory);
        }
    }

    public void ShowRecipe(int index)
    {
        selectedRecipe = craftingTable.recipes[index];
        recipeTitle.text = selectedRecipe.Name;

        for(int i = 0; i < recipeInputViews.Length; i++)
        {
            if(i < selectedRecipe.inputs.Length)
            {
                recipeInputViews[i].Show();
                recipeInputViews[i].SetData(selectedRecipe.inputs[i].item.Name, selectedRecipe.inputs[i].count, selectedRecipe.inputs[i].item.icon);
            }
            else
            {
                recipeInputViews[i].Hide();
            }
        }
        recipeOutputView.Show();
        recipeOutputView.SetData(selectedRecipe.output.item.Name, selectedRecipe.output.count, selectedRecipe.output.item.icon);

    }

    public void Craft()
    {
        craftingTable.Craft(selectedRecipe, inventory);
    }

    public override void Show()
    {
        inventory.onInventoryUpdated += UpdateAvailability;
        base.Show();
    }

    public override void Hide()
    {
        inventory.onInventoryUpdated -= UpdateAvailability;
        base.Hide();
    }
}
