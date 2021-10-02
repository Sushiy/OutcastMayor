using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public Image icon;
    public TMPro.TMP_Text name;
    public Button button;

    public void SetRecipe(Recipe recipe, UnityAction callback)
    {
        icon.sprite = recipe.icon;
        name.text = recipe.Name;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(callback);
    }
}
