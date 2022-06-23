using OutcastMayor.Interaction;
using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Zoning
{
    [RequireComponent(typeof(Inventory))]
    public class StockpileZone : Interactable
    {
        int sizeX, sizeZ;
        Vector3[] gridPositions;
        ItemStackInstance[] itemStacks;

        Inventory inventory;

        private void OnEnable()
        {
            inventory = GetComponent<Inventory>();
            inventory.onSlotUpdated += UpdateSlot;
        }

        private void OnDisable()
        {
            inventory.onSlotUpdated -= UpdateSlot;
        }

        public void InitStockpile(int sizeX, int sizeZ, List<List<Vector3>> gridCornerPositions)
        {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;

            inventory.SetSlotCount(sizeX * sizeZ);
            itemStacks = new ItemStackInstance[sizeX * sizeZ];

            gridPositions = new Vector3[sizeX * sizeZ];
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    //Get four corners
                    Vector3 sum = gridCornerPositions[x][z] + gridCornerPositions[x + 1][z] + gridCornerPositions[x][z + 1] + gridCornerPositions[x + 1][z + 1];
                    sum /= 4f;
                    gridPositions[x + z * sizeX] = sum;
                    Debug.DrawRay(gridPositions[x + z * sizeX], Vector3.up, Color.red, 5.0f);
                }
            }
        }

        public void UpdateSlot(int index)
        {
            Inventory.ItemStack stack = inventory.slots[index];
            if (stack.item == null)
            {
                if (itemStacks[index] != null)
                {
                    itemStacks[index] = null;
                    Destroy(itemStacks[index].gameObject);
                }
            }
            else
            {
                if (itemStacks[index] != null)
                {
                    if (itemStacks[index].source.item == stack.item)
                    {
                        itemStacks[index].source.count = stack.count;
                    }
                    else
                    {
                        GameObject old = itemStacks[index].gameObject;
                        Destroy(old);
                        itemStacks[index] = Instantiate(stack.item.prefab, gridPositions[index], Quaternion.identity, transform).GetComponent<ItemStackInstance>();
                        itemStacks[index].OnPickedUp += () => PickupSlot(index);
                        itemStacks[index].source.count = stack.count;
                    }
                }
                else
                {
                    itemStacks[index] = Instantiate(stack.item.prefab, gridPositions[index], Quaternion.identity, transform).GetComponent<ItemStackInstance>();
                    itemStacks[index].OnPickedUp += () => PickupSlot(index);
                    itemStacks[index].source.count = stack.count;
                }
            }
        }

        public void PickupSlot(int index)
        {
            Inventory.ItemStack stack = inventory.slots[index];
            inventory.DeleteCountFromSlot(index, stack.count);
        }

        public Inventory.ItemStack itemStackTest;

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            inventory.Add(itemStackTest);
        }

    }

}
