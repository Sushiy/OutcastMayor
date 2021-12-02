using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "UseItemAction", menuName = "ScriptableObjects/UtilityAI/Actions/UseItemAction", order = 1)]
    public class UseItemAction : Action
    {
        [SerializeField] Inventory.ItemStack stack;

        public override void Execute(UtilityAIController controller, Object[] instanceData)
        {
            Debug.Log(controller.name + " used a stack of " + stack.item.Name + ":" + stack.count);
            controller.inventory.Delete(stack);
        }

        public override ActionInstance[] GetActionInstances(UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, new Object[0]) };
            return result;
        }
    }

}
