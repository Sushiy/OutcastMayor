using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour
{
    public Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }
}
