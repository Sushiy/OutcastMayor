using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingView : UIPanel
{
    public BuildingMode buildingMode;

    public RecipeButton[] recipeButtons;

    public override void Show()
    {
        UpdateData();
        base.Show();
    }

    public void UpdateData()
    {
        //What do I do if i am lacking recipe buttons?

        for(int i = 0; i < recipeButtons.Length; i++)
        {
            if(i < buildingMode.recipes.Length)
            {
                recipeButtons[i].SetData(buildingMode.recipes[i].Name, buildingMode.recipes[i].Icon);
                recipeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                recipeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ButtonClicked(int i)
    {
        buildingMode.ChooseBuildRecipe(buildingMode.recipes[i]);
    }
}