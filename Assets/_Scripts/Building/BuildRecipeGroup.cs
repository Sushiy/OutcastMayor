using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recipe for a building block
/// </summary>
[CreateAssetMenu(fileName = "NewBuildRecipeGroup", menuName = "ScriptableObjects/BuildRecipeGroup", order = 1)]
public class BuildRecipeGroup : BuildRecipe
{
    public BuildRecipe[] buildRecipes;
    private int currentRecipeIndex = 0;

    public override Inventory.ItemStack[] Materials
    {
        get
        {
            return buildRecipes[currentRecipeIndex].Materials;
        }
    }
    public override Buildable BuildingPrefab
    {
        get
        {
            return buildRecipes[currentRecipeIndex].BuildingPrefab;
        }
    }
    public override Construction ConstructionPrefab
    {
        get
        {
            return buildRecipes[currentRecipeIndex].ConstructionPrefab;
        }
    }
    public void NextVariation()
    {
        currentRecipeIndex++;
        if(currentRecipeIndex == buildRecipes.Length)
        {
            currentRecipeIndex = 0;
        }
        SelectRecipe(currentRecipeIndex);
    }
    public void PreviousVariation()
    {
        currentRecipeIndex--;
        if (currentRecipeIndex == -1)
        {
            currentRecipeIndex = buildRecipes.Length-1;
        }
        SelectRecipe(currentRecipeIndex);
    }

    public void SelectRecipe(int index)
    {
        materials = buildRecipes[index].Materials;
        buildingPrefab = buildRecipes[index].BuildingPrefab;
        constructionPrefab = buildRecipes[index].ConstructionPrefab;
    }
    public override void Alternate(float alternateInput)
    {
        if(alternateInput > 0)
        {
            NextVariation();
        }
        if(alternateInput < 0)
        {
            PreviousVariation();
        }
    }
}