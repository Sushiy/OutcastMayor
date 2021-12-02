using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "BuildConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/BuildConstructionAction", order = 1)]
    public class BuildConstructionAction : Action
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>An array of action instances</returns>
        public override ActionInstance[] GetActionInstances(UtilityAIController controller)
        {
            ActionInstance[] instances = new ActionInstance[controller.availableConstructions.Count];

            for(int i = 0; i < instances.Length; i++)
            {
                instances[i] = new ActionInstance(this, new Object[] { controller.availableConstructions[i], controller.availableConstructions[i].transform });
            }

            return instances;
        }

        public override void Execute(UtilityAIController controller, Object[] instanceData)
        {
            Construction constructionTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
            }

            Vector3 target = constructionTarget.transform.position + (controller.transform.position - constructionTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> BuildConstructionAction moveto.</color>");
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(constructionTarget, controller));

        }

        public void CompleteAction(Construction constructionTarget, UtilityAIController controller)
        {
            string log = "<color=green>" + controller.name + " -> BuildConstructionAction for " + constructionTarget.gameObject.name + "</color>";

            constructionTarget.Interact(controller.interactor);
            if (constructionTarget.IsCompleted)
            {
                controller.availableConstructions.Remove(constructionTarget);
            }
            Debug.Log(log);
        }
    }

}
