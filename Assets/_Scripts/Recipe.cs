using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    public string Name;
    public Sprite icon;

    public Inventory.Stack[] inputs;
    public Inventory.Stack output;

    public bool IsValid(Inventory inventory)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (!inventory.Contains(inputs[i]))
            {
                return false;
            }
        }
        return true;
    }
}
