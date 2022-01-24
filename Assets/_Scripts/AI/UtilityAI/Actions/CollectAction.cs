using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "CollectAction", menuName = "ScriptableObjects/UtilityAI/Actions/CollectAction", order = 1)]
    /// <summary>
    /// Gather Objects from the ground and put them into any stockpile
    /// CollectActions require an ItemstackInstance on the owning GameObject
    /// </summary>
    public class CollectAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            ItemStackInstance itemStack = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if (itemStack == null)
            {
                Debug.LogError("No itemStack");
                return;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return;
            }

            Vector3 target = itemStack.transform.position + (controller.transform.position - itemStack.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> CollectAction moveto.</color>");
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(itemStack, controller));
        }

        public void CompleteAction(ItemStackInstance itemStack, UtilityAIController controller)
        {
            string log = controller.name + " -> HaulAction on" + itemStack.name + ":\n";
            itemStack.Interact(controller.Interactor);
            Debug.Log(log);

        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            ItemStackInstance itemStackInstance = owner.GetComponent<ItemStackInstance>();

            if(itemStackInstance != null)
            {

                for (int i = 0; i < controller.availableStockpiles.Count; i++)
                {
                    ActionInstance instance = new ActionInstance(this, owner, new Object[] { itemStackInstance, controller.availableStockpiles[i], owner.transform }, new int[0]);
                    if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                    {
                        instances.Add(instance);
                    }
                }

            }
            return instances.ToArray();
        }


        /// <summary>
        /// Checks if the following requirements are met: Instance Data includes an ItemstackInstance and a Stockpile; The stockpile has enough space for the itemstack 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="instanceData"></param>
        /// <param name="instanceValues"></param>
        /// <returns></returns>
        public override bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
        {
            if (owner.isOccupied) return false;
            ItemStackInstance itemStack = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if (itemStack == null)
            {
                Debug.LogError("No itemStack");
                return false;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return false;
            }

            return stockpileTarget.inventory.HasSpaceFor(itemStack.source);
        }
    }
}
