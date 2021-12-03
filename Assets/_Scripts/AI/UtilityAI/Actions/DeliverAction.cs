using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "DeliverAction", menuName = "ScriptableObjects/UtilityAI/Actions/DeliverAction", order = 1)]
    /// <summary>
    /// Gather Objects from the ground and put them into any stockpile
    /// </summary>
    public class DeliverAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Stockpile stockpileTarget = instanceData[0] as Stockpile;
            Inventory.ItemStack itemStack = new Inventory.ItemStack();
            itemStack.item = instanceData[2] as Item;
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return;
            }

            Vector3 target = stockpileTarget.transform.position + (controller.transform.position - stockpileTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> DeliverAction moveto.</color>");
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(stockpileTarget, itemStack, controller));
        }

        public void CompleteAction(Stockpile stockpileTarget, Inventory.ItemStack itemStack, UtilityAIController controller)
        {
            string log = controller.name + " -> DeliverAction on" + stockpileTarget.name + ":\n";
            stockpileTarget.inventory.Add(itemStack);
            controller.inventory.Delete(itemStack);
            Debug.Log(log);

        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            for (int i = 0; i < controller.inventory.slots.Length; i++)
            {
                if(controller.inventory.slots[i].item == null || controller.inventory.slots[i].count == 0)
                {
                    continue;
                }
                int[] b = new int[1];
                b[0] = controller.inventory.slots[i].count;
                ActionInstance instance = new ActionInstance(this, owner, new Object[] { owner.GetComponent<Stockpile>(), owner.transform, controller.inventory.slots[i].item}, b);
                if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                {
                    instances.Add(instance);
                }
            }

            return instances.ToArray();
        }
        public override bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
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
    }
}
