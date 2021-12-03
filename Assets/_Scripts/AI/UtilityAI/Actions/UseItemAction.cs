using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "UseItemAction", menuName = "ScriptableObjects/UtilityAI/Actions/UseItemAction", order = 1)]
    public class UseItemAction : Action
    {
        [SerializeField] Inventory.ItemStack stack;

        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " used a stack of " + stack.item.Name + ":" + stack.count);
            controller.inventory.Delete(stack);
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, owner, new Object[0], new int[0]) };
            return result;
        }
    }

}
