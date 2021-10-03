using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : UIPanel
{
    public Inventory inventory;
    public InventorySlotView[] slotViews;

    public Inventory.Stack grabbedStack;
    public Sprite emptyIcon;
    protected override void Awake()
    {
        base.Awake();
        inventory.onSlotUpdated += UpdateSlot;
    }

    public void UpdateSlot(int index)
    {
        Inventory.Stack stack = inventory.slots[index];
        if(stack.item == null)
        {
            slotViews[index].UpdateData("", 0, emptyIcon);
        }
        else
        {
            slotViews[index].UpdateData(stack.item.Name, stack.count, stack.item.icon);
        }
    }

    public void SlotClicked(int index)
    {
        if(grabbedStack.item == null)
        {
            grabbedStack = inventory.GrabFromSlot(index);
        }
        else
        {
            grabbedStack = inventory.AddStackToSlot(grabbedStack, index);
        }
    }
}
