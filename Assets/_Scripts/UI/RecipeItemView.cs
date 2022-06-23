using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OutcastMayor.UI
{
    public class RecipeItemView : UIPanel
    {
        public TMP_Text itemName;
        public TMP_Text itemCount;
        public Image itemIcon;

        public void SetData(string name, int necessaryCount, int availableCount, Sprite icon)
        {
            Color c = availableCount >= necessaryCount ? Color.black : Color.red;
            itemName.text = name;
            itemCount.text = availableCount.ToString() + "/" + necessaryCount.ToString();
            itemCount.color = c;
            itemIcon.sprite = icon;
        }
        public void SetData(string name, int resultCount, Sprite icon)
        {
            itemName.text = name;
            itemName.color = Color.black;
            itemCount.text = resultCount.ToString();
            itemCount.color = Color.black;
            itemIcon.sprite = icon;
        }
    }

}
