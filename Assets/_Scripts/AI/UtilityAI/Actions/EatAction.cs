using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "EatAction", menuName = "ScriptableObjects/UtilityAI/Actions/EatAction", order = 1)]
    public class EatAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " started eating");
            controller.food += .5f;

        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, owner, new Object[0], new int[0]) };
            return result;
        }
    }
}
