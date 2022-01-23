using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "BuildConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/BuildConstructionAction", order = 1)]
    public class BuildConstructionAction : Action
    {        
        /// <summary>
        /// Build the Instances, in this case it is only one.
        /// </summary>
        /// <returns>An array of action instances</returns>
        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();
            ActionInstance instance = new ActionInstance(this, owner, new Object[] { owner.GetComponent<Construction>(), owner.transform }, null);
            if(CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
            {
                instances.Add(instance);
            }
            return instances.ToArray();
        }

        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
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

            constructionTarget.Interact(controller.Interactor);
            if (constructionTarget.IsCompleted)
            {
                controller.availableConstructions.Remove(constructionTarget);
            }
            Debug.Log(log);
        }
    }

}
