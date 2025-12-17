using System.Collections;
using System.Collections.Generic;
using OutcastMayor.Building;
using UnityEngine;


/// <summary>
/// Collection of recipes. This is mostly useful to unlock stuff in groups
/// </summary>
[CreateAssetMenu(fileName = "NewBuildRecipeCollection", menuName = "ScriptableObjects/BuildRecipeCollection", order = 1)]
public class BuildRecipeCollection : ScriptableObject
{
    [SerializeField]
    private BuildRecipe[] buildRecipes;

    public BuildRecipe[] GetRecipes()
    {
        return buildRecipes;
    }
}
