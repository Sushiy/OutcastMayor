using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item: ScriptableObject
{
    [Header("Info")]
    public string Name;
    public int ID;

    public float weight;
    public int stackLimit;

    [Header("Internal")]
    public GameObject prefab;
}
