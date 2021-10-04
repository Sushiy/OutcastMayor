using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentBuildPanel : UIPanel
{
    public TMPro.TMP_Text recipeTitle;
    public InventorySlotView[] views;

    public void SetData(BuildRecipe r)
    {
        recipeTitle.text = r.Name;

        for(int i = 0; i < views.Length; i++)
        {
            if(i < r.materials.Length)
            {
                views[i].gameObject.SetActive(true);
                views[i].UpdateData(r.materials[i].item.Name, r.materials[i].count, r.materials[i].item.icon);
            }
            else
            {
                views[i].gameObject.SetActive(false);
            }
        }
    }
}
