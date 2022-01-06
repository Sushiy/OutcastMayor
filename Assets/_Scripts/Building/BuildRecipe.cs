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
    [SerializeField]
    protected Inventory.ItemStack[] materials;
    public virtual Inventory.ItemStack[] Materials
    {
        get
        {
            return materials;
        }
    }
    /// <summary>
    /// The prefab to spawn when building this
    /// </summary>
    [SerializeField]
    protected Buildable buildingPrefab;
    public virtual Buildable BuildingPrefab
    {
        get
        {
            return buildingPrefab;
        }
    }
    [SerializeField]
    protected Construction constructionPrefab;
    public virtual Construction ConstructionPrefab
    {
        get
        {
            return constructionPrefab;
        }
    }

    protected Vector3 buildScale = Vector3.one;
    public Vector3 BuildScale
    {
        get
        {
            return buildScale;
        }
    }

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

    public virtual void Alternate(float alternateInput)
    {

    }
}