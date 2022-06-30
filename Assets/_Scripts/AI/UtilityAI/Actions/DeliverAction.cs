using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "DeliverAction", menuName = "ScriptableObjects/UtilityAI/Actions/DeliverAction", order = 1)]
    /// <summary>
    /// Clear your inventory from stuff put them into any stockpile
    /// </summary>
    public class DeliverAction : Action
    {
        public override void Init(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            Transform moveTarget = instanceData[1] as Transform;
            if (moveTarget == null)
            {
                Debug.LogError("No MoveTarget");
                return;
            }

            Vector3 target = moveTarget.transform.position + (controller.transform.position - moveTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> DeliverAction moveto.</color>");
            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            Stockpile stockpileTarget = instanceData[0] as Stockpile;
            Inventory.ItemStack itemStack = new Inventory.ItemStack();
            itemStack.item = instanceData[2] as Item;
            itemStack.count = instanceValues[0];
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return;
            }
            string log = controller.name + " -> DeliverAction " + itemStack.item.DisplayName + " to " + stockpileTarget.name + ":\n";
            stockpileTarget.inventory.Add(itemStack);
            controller.Inventory.Delete(itemStack);
            Debug.Log(log);
            controller.ActionCompleted();
        }

        public override void Cancel(UtilityAICharacter controller, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //What to do with the stuff in your inventory?
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            for (int i = 0; i < controller.Inventory.slots.Length; i++)
            {
                if(controller.Inventory.slots[i].item == null || controller.Inventory.slots[i].count == 0)
                {
                    continue;
                }
                ActionInstance instance = new ActionInstance(this, owner, new UnityEngine.Object[] { owner.GetComponent<Stockpile>(), owner.transform, controller.Inventory.slots[i].item}, new int[] {controller.Inventory.slots[i].count });
                //Debug.Log("Slot: " + i + "/" + controller.Inventory.slots.Length+ " contains: " + controller.Inventory.slots[i].count);
                if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                {
                    instances.Add(instance);
                }
            }

            return instances.ToArray();
        }
        public override bool CheckInstanceRequirement(SmartObject owner, UnityEngine.Object[] instanceData, int[] instanceValues)
        {
            if (owner.isOccupied) return false;
            Stockpile stockpileTarget = instanceData[0] as Stockpile;
            Inventory.ItemStack itemStack = new Inventory.ItemStack();
            itemStack.item = instanceData[2] as Item;
            if (itemStack.item == null)
            {
                Debug.LogError("No itemStack");
                return false;
            }
            else
            {
                if(instanceValues.Length == 0)
                {
                    Debug.LogError("instanceValue not working");
                }
                else
                {
                    itemStack.count = (int)instanceValues[0];
                }
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return false;
            }

            return stockpileTarget.inventory.HasSpaceFor(itemStack);
        }

        public override Type[] GetProvidedDataTypes()
        {
            return new Type[] { typeof(Stockpile), typeof(Transform), typeof(Item) };
        }
    }
}
