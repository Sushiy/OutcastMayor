using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public Image icon;
    public new TMPro.TMP_Text name;
    public Button button;

    public void SetData(string name, Sprite icon)
    {
        this.icon.sprite = icon;
        this.name.text = name;
    }
}
