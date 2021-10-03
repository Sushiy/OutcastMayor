using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItemView : UIPanel
{
    public TMP_Text itemName;
    public TMP_Text itemCount;
    public Image itemIcon;

    public void SetData(string name, int count, Sprite icon)
    {
        itemName.text = name;
        itemCount.text = count.ToString();
        itemIcon.sprite = icon;
    }
}
