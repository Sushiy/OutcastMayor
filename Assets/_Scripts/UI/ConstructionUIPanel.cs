using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUIPanel : UIPanel
{
    public TMPro.TMP_Text recipeTitle;
    public InventorySlotView[] views;

    public void SetData(Interactor interactor, Construction construction)
    {
        BuildRecipe r = construction.buildRecipe;
        recipeTitle.text = r.Name;

        for(int i = 0; i < views.Length; i++)
        {
            if(i < r.Materials.Length)
            {
                views[i].gameObject.SetActive(true);
                views[i].UpdateData(r.Materials[i].item.DisplayName, construction.GetCountString(interactor, i), r.Materials[i].item.icon);
            }
            else
            {
                views[i].gameObject.SetActive(false);
            }
        }
    }
}
