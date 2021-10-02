using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : UIPanel
{
    public Inventory inventory;
    public InventorySlotView[] slotViews;

    protected override void Awake()
    {
        base.Awake();
        inventory.onSlotUpdated += UpdateSlot;
    }

    public void UpdateSlot(int index)
    {
        Inventory.Stack stack = inventory.slots[index];
        if(stack == null || stack.item == null)
        {
            slotViews[index].UpdateData("null", 0);
        }
        else
        {
            slotViews[index].UpdateData(stack.item.Name, stack.count);
        }
    }
}
