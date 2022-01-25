using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item", order = 1)]
public class Item: ScriptableObject
{
    [Flags]
    public enum Tags
    {
        Equippable,
        Food,

    }

    [Header("Info")]
    public string DisplayName;
    public Sprite icon;
    public int stackLimit;
    public float weight;

    [Header("Internal")]
    public GameObject prefab;

    private void OnValidate()
    {
        ItemCatalogue.TryAddItem(this);
    }
}
