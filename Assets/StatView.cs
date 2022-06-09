using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class StatView : MonoBehaviour
{
    [Header("Settings")]
    public string statName = "Stat";
    public Color barColor = Color.green;

    [Header("References")]
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text valueText;
    public UnityEngine.UI.Image barFill;

    private void Awake()
    {
        nameText.text = statName;
        barFill.color = barColor;
    }

    public void SetValue(float f)
    {
        valueText.text = ((int)f).ToString();
    }

    private void OnValidate()
    {
        nameText.text = statName;
        barFill.color = barColor;
    }
}
