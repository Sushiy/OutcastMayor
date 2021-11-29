using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "EatAction", menuName = "ScriptableObjects/UtilityAI/Actions/EatAction", order = 1)]
    public class EatAction : Action
    {
        public override void Execute(UtilityAIController controller)
        {
            Debug.Log(controller.name + " started eating");
            controller.food += .5f;
        }
    }

}
