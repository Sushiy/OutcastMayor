using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recipe for a building block
/// </summary>
[CreateAssetMenu(fileName = "NewBuildRecipe", menuName = "ScriptableObjects/BuildRecipe", order = 1)]
public class BuildRecipe : ScriptableObject
{
    public string Name = "Buildable";
    public Sprite Icon;

    /// <summary>
    /// The materials needed to build this Recipe
    /// </summary>
    public Inventory.Stack[] materials;
    /// <summary>
    /// The actionPoints needed to build this Recipe (actionPoints come from Hammering!)
    /// </summary>
    public int actionPoints;
    /// <summary>
    /// The prefab to spawn when building this
    /// </summary>
    public Buildable prefab;

    /// <summary>
    /// Check if the given inventory has all needed materials
    /// </summary>
    /// <param name="inventory">The inventory to check for materials</param>
    /// <returns>true if all materials are present, false otherwise</returns>
    public bool IsValid(Inventory inventory)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (!inventory.Contains(materials[i]))
            {
                return false;
            }
        }
        return true;
    }
}