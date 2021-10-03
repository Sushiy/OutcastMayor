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
    public struct Stack
    {
        public Item item;
        public int count;

        public Stack(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }

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
                    return new Stack(null, 0);
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

    public System.Action<int> onSlotUpdated;
    public bool shouldUpdate = false;
    public System.Action onInventoryUpdated;

    private void Update()
    {
        if(shouldUpdate)
        {
            shouldUpdate = false;
            onInventoryUpdated?.Invoke();
        }
    }

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
                shouldUpdate = true;
                print("Stack added onto slot " + firstFreeSlotIndex);
                if (stack.item == null)
                {
                    return true;
                }
            }
            if (firstFreeSlotIndex < 0 && slots[i].item == null)
            {
                firstFreeSlotIndex = i;
            }
        }
        if(stack.item != null && firstFreeSlotIndex >= 0)
        {
            print("Stack added to free slot " + firstFreeSlotIndex);
            slots[firstFreeSlotIndex] = stack;
            onSlotUpdated.Invoke(firstFreeSlotIndex);
            shouldUpdate = true;
            return true;
        }

        //if you didnt find a free slot, you can't add the stack!
        return false;
    }

    public Stack GrabFromSlot(int index)
    {
        Stack result = new Stack(slots[index]);
        slots[index].item = null;
        slots[index].count = 0;
        onSlotUpdated.Invoke(index);
        shouldUpdate = true;
        return result;
    }

    /// <summary>
    /// Remove the required number from the stack
    /// </summary>
    /// <param name="index"></param>
    /// <param name="stack"></param>
    /// <returns>Returns the number of items that could not be removed (overflow) </returns>
    public int DeleteCountFromSlot(int index, int count)
    {
        Stack s = slots[index];
        if(s.count > count)
        {
            print("Delete " + count + "/" + s.count + " from slot " + index);
            s.count -= count;
            onSlotUpdated(index);
            shouldUpdate = true;
            slots[index] = s;
            return 0;
        }
        else
        {
            print("Delete " + count + "/" + s.count + " from slot " + index);
            count -= s.count;
            s.count = 0;
            s.item = null;
            onSlotUpdated(index);
            shouldUpdate = true;
            slots[index] = s;
            print("Slot "+ index + " is now null:" + (slots[index].item == null).ToString());
            return count;
        }
    }

    public void Delete(Stack stack)
    {
        if(Contains(stack))
        {
            int totalCount = stack.count;
            //Look through all stacks and remove until totalCount == 0
            for (int i = slots.Length-1; i >= 0 && totalCount > 0; i--)
            {
                if (slots[i].item == stack.item)
                {
                    print("Deleting " + stack.count + " " + stack.item.Name + " from slot " + i);
                    totalCount = DeleteCountFromSlot(i, totalCount);
                }
            }
        }
        else
        {
            Debug.LogError("The ressources to delete are not here");
        }
    }

    public Stack AddStackToSlot(Stack stack, int slotIndex)
    {
        Stack result = new Stack(null,0);
        if(stack.item != slots[slotIndex].item)
        {
            //Replace stack
            result = slots[slotIndex];
            slots[slotIndex] = stack;
        }
        else
        {
            slots[slotIndex].Add(stack);
        }
        onSlotUpdated.Invoke(slotIndex);
        shouldUpdate = true;
        return result;
    }

    public bool Contains(Stack stack)
    {
        int totalCount = 0;
        //Look through all stacks and check recipes
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item == stack.item)
            {
                totalCount += slots[i].count;
            }
        }
        return totalCount >= stack.count;

    }
}
