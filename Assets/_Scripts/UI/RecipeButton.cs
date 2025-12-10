using System;
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

    UnityAction<int> callback;
    int buttonIndex;

    void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void SetData(string _name, Sprite _icon, UnityAction<int> _callback, int _buttonIndex)
    {
        this.icon.sprite = _icon;
        this.name.text = _name;
        callback = _callback;
        buttonIndex = _buttonIndex;
    }

    void OnClick()
    {
        callback?.Invoke(buttonIndex);
    }
}
