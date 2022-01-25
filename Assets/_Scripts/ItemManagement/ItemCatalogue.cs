using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCatalogue
{
    private static Dictionary<string, Item> catalogue;

    public static bool TryAddItem(Item item)
    {
        if(catalogue == null)
        {
            catalogue = new Dictionary<string, Item>();
        }

        if(catalogue.ContainsKey(item.name))
        {
            Debug.LogError("ItemCatalogue: Item with name:" + item.name + "already exists");
            return false; 
        }
        else
        {
            catalogue.Add(item.name, item);
            return true;
        }
    }
}
