using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTableView : UIPanel
{
    CraftingTable craftingTable;
    Inventory inventory;
    public Transform recipeButtonParent;
    public List<RecipeButton> recipeButtons;

    public void SetTable(CraftingTable craftingTable, Inventory inventory)
    {
        //Do i have enough buttons?

        //if i do:
        for(int i = 0; i < recipeButtons.Count; i++)
        {
            if(i < craftingTable.recipes.Length)
            {
                Recipe r = craftingTable.recipes[i];
                recipeButtons[i].gameObject.SetActive(true);
                recipeButtons[i].SetRecipe(r, () => craftingTable.Craft(r, inventory));
            }
            else
            {
                recipeButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
