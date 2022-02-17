using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "EatAction", menuName = "ScriptableObjects/UtilityAI/Actions/EatAction", order = 1)]
    public class EatAction : Action
    {
        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            controller.ChangeState(controller.PerformingState);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " started eating");

            if (controller.Inventory.Contains("Apple"))
            {
                controller.Inventory.Delete("Apple");
            }
            controller.food += .5f;

        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Put away the food?
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, owner, new Object[0], new int[0]) };
            return result;
        }
    }
}
