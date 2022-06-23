using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item", order = 1)]
public class Item: ScriptableObject
{
    [Flags]
    public enum Tag
    {
        None = 0,
        Equippable = 1,
        Food = 2
    }

    [Header("Info")]
    public string DisplayName;
    public Sprite icon;
    public int stackLimit;
    public float weight;

    public Tag tags;

    [Header("Internal")]
    public GameObject prefab;

    protected void OnValidate()
    {
        ItemCatalogue.TryAddItem(this);
    }

    public bool HasTag(Tag tag)
    {
        return (tags & tag) == tag;
    }
}
