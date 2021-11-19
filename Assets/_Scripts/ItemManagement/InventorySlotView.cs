using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotView : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;
    public Image icon;

    private void Awake()
    {
        nameText.text = "";
        countText.text = "";
    }

    public void UpdateData(string name, string count, Sprite sprite)
    {
        nameText.text = name;
        countText.text = count;
        icon.sprite = sprite;
    }
}
