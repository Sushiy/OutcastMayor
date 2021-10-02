using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Inventory stores a character's items in an array of slots
/// 
/// </summary>
public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class Stack
    {
        public Item item;
        public int count;

        public Stack(Stack other)
        {
            item = other.item;
            count = other.count;
        }

        /// <summary>
        /// Add Itm
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="newCount"></param>
        /// <returns>Result goes back to the inventorycursor</returns>
        public Stack Add(Stack stack)
        {
            //Check if we are adding the same item
            if (stack.item == item)
            {
                //Same item:
                //Check if there is enough space left to stack
                if(count + stack.count <= item.stackLimit)
                {
                    //Add to stack
                    count += stack.count;
                    return null;
                }
                else
                {
                    //???
                    int r = (count + stack.count) - item.stackLimit;
                    count = item.stackLimit;
                    stack.count = r;
                    return stack;
                }
            }
            else
            {
                //Different item:
                //Replace and hold the other item?
                return stack;
            }
        }
    }

    public Stack[] slots;

    public Stack heldStack;

    public System.Action<int> onSlotUpdated;

    public bool Add(Stack stack)
    {
        int firstFreeSlotIndex = -1;
        //Find Stacks of the same type to add to
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item == stack.item)
            {
                stack = slots[i].Add(stack);
                onSlotUpdated.Invoke(i);
                print("Stack added onto slot " + firstFreeSlotIndex);
                if (stack == null)
                {
                    return true;
                }
            }
            if (firstFreeSlotIndex < 0 && slots[i].item == null)
            {
                firstFreeSlotIndex = i;
            }
        }
        if(stack != null && firstFreeSlotIndex >= 0)
        {
            print("Stack added to free slot " + firstFreeSlotIndex);
            slots[firstFreeSlotIndex] = stack;
            onSlotUpdated.Invoke(firstFreeSlotIndex);
            return true;
        }

        //if you didnt find a free slot, you can't add the stack!
        return false;
    }

    public Stack RemoveStackFromSlot(int index)
    {
        Stack result = new Stack(slots[index]);
        slots[index] = null;
        onSlotUpdated.Invoke(index);
        return result;
    }

    public void AddStackToSlot(Stack stack, int slotIndex)
    {
        if(stack.item != slots[slotIndex].item)
        {
            //Replace stack
            heldStack = slots[slotIndex];
            slots[slotIndex] = stack;
        }
        else
        {
            slots[slotIndex].Add(stack);
        }
        onSlotUpdated.Invoke(slotIndex);
    }
}
