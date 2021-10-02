using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item", order = 1)]
public class Item: ScriptableObject
{
    [Header("Info")]
    public string Name;
    public int ID = -1;

    public float weight;
    public int stackLimit;

    [Header("Internal")]
    public GameObject prefab;
}
