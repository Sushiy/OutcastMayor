using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepAction", menuName = "ScriptableObjects/UtilityAI/Actions/SleepAction", order = 1)]
    public class SleepAction : Action
    {
        public override void Execute(UtilityAIController controller)
        {
            Debug.Log(controller.name + " started sleeping");
            controller.sleepy = 0;
        }
    }

}
