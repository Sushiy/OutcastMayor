using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySlotView : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;

    public void UpdateData(string name, int count)
    {
        nameText.text = name;
        countText.text = count.ToString();
    }
}
