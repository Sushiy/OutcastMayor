using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Items
{
    public class Stockpile : MonoBehaviour
    {
        public Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }
    }

}
