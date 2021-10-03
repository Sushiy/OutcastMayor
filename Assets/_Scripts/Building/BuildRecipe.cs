using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildRecipe", menuName = "ScriptableObjects/BuildRecipe", order = 1)]
public class BuildRecipe : ScriptableObject
{
    public string Name = "Buildable";
    public Sprite Icon;

    public Inventory.Stack[] materials;
    public Buildable prefab;

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