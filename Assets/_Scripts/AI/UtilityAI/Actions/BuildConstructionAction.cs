using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "BuildConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/BuildConstructionAction", order = 1)]
    public class BuildConstructionAction : Action
    {
        Construction construction;

        public override void InitReasonerData(UtilityAIController controller)
        {
            if (controller.availableConstructions.Count == 0)
            {
                construction = null;
            }
            else
            {
                //I need to somehow choose the right construction!
                construction = controller.availableConstructions[0];
            }
            //I need to initiate the considerations with the construction data?
            controller.chosenConstruction = construction;
        }

        public override void Execute(UtilityAIController controller)
        {
            string log = controller.name + " -> BuildConstructionAction for " + construction.gameObject.name + ":\n";

            construction.Interact(controller.interactor);
            if(construction.IsCompleted)
            {
                controller.availableConstructions.Remove(construction);
            }
            Debug.Log(log);
        }
    }

}
