using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepAction", menuName = "ScriptableObjects/UtilityAI/Actions/SleepAction", order = 1)]
    public class SleepAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData)
        {
            Debug.Log(controller.name + " started sleeping");
            controller.sleepy = 0;
        }

        public override ActionInstance[] GetActionInstances(UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, new Object[0]) };
            return result;
        }
    }

}
