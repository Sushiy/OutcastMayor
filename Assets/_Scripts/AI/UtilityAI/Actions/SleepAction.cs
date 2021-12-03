using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepAction", menuName = "ScriptableObjects/UtilityAI/Actions/SleepAction", order = 1)]
    public class SleepAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " started sleeping");
            controller.sleepy = 0;
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, owner, new Object[0], new int[0]) };
            return result;
        }
    }

}
