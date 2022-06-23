using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Items
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "ScriptableObjects/Recipe", order = 1)]
    public class Recipe : ScriptableObject
    {
        public string Name
        {
            get
            {
                return output.item.DisplayName;
            }
        }
        public Sprite Icon
        {
            get
            {
                return output.item.icon;
            }
        }

        public Inventory.ItemStack[] inputs;
        public Inventory.ItemStack output;

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


}