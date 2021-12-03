using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "GetItemAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetItemAction", order = 1)]
    public class GetXOfItemAction : Action
    {
        [SerializeField] Inventory.ItemStack stack;

        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            int newCount = stack.count - controller.inventory.GetTotalCount(stack.item);
            Debug.Log(controller.name + " grabbed a stack of " + stack.item.Name + ":" + newCount);
            controller.inventory.Add(new Inventory.ItemStack(stack.item, newCount));
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, owner, new Object[0], new int[0]) };
            return result;
        }
    }

}
