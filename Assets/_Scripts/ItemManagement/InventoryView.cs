using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : UIPanel
{
    public Inventory inventory;
    public InventorySlotView[] slotViews;

    public Inventory.ItemStack grabbedStack;
    public Sprite emptyIcon;

    public InventorySlotView grabbedItemView;

    protected override void Awake()
    {
        base.Awake();
        inventory.onSlotUpdated += UpdateSlot;
        grabbedItemView.UpdateData("", "0", emptyIcon);
        grabbedItemView.gameObject.SetActive(false);
    }

    public void UpdateSlot(int index)
    {
        Inventory.ItemStack stack = inventory.slots[index];
        if(stack.item == null)
        {
            slotViews[index].UpdateData("", "0", emptyIcon);
        }
        else
        {
            slotViews[index].UpdateData(stack.item.Name, stack.count.ToString(), stack.item.icon);
        }
    }

    public void SlotClicked(int index)
    {
        if(grabbedStack.item == null)
        {
            grabbedStack = inventory.GrabFromSlot(index);
            grabbedItemView.UpdateData(grabbedStack.item.Name, grabbedStack.count.ToString(), grabbedStack.item.icon);
            grabbedItemView.gameObject.SetActive(true);
        }
        else
        {
            grabbedStack = inventory.AddStackToSlot(grabbedStack, index);
            if(grabbedStack.item == null)
            {
                grabbedItemView.UpdateData("", "0", emptyIcon);
                grabbedItemView.gameObject.SetActive(false);
            }
            else
            {
                grabbedItemView.UpdateData(grabbedStack.item.Name, grabbedStack.count.ToString(), grabbedStack.item.icon);
            }
        }
    }
}
