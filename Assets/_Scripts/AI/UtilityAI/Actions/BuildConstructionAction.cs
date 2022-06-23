using OutcastMayor.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "BuildConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/BuildConstructionAction", order = 1)]
    public class BuildConstructionAction : Action
    {
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
        }

        /// <summary>
        /// Build the Instances, in this case it is only one.
        /// </summary>
        /// <returns>An array of action instances</returns>
        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();
            ActionInstance instance = new ActionInstance(this, owner, new Object[] { owner.GetComponent<Construction>(), owner.transform }, null);
            if(CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
            {
                instances.Add(instance);
            }
            return instances.ToArray();
        }

        //Do your action state stuff here!
        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Do stuff once at the beginning
            Transform moveTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    moveTarget = instanceData[i] as Transform;
                }
            }

            Vector3 target = moveTarget.transform.position + (controller.transform.position - moveTarget.transform.position).normalized * 1.0f;

            Debug.Log("<color=green>" + controller.name + "-> BuildConstructionAction moveto.</color>");
            
            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Do stuff once at the beginning
            Construction constructionTarget = null;
            string log = "<color=green>" + controller.name + " -> BuildConstructionAction for " + constructionTarget.gameObject.name + "</color>";
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
            }

            constructionTarget.Interact(controller.Interactor);
            if (constructionTarget.IsCompleted)
            {
                controller.availableConstructions.Remove(constructionTarget);
            }
            Debug.Log(log);
            Cancel(controller, null, null);
        }
    }

}
