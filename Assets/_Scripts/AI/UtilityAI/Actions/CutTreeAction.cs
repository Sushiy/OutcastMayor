using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "CutTreeAction", menuName = "ScriptableObjects/UtilityAI/Actions/CutTreeAction", order = 1)]
    public class CutTreeAction : Action
    {
        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " goes to cut a tree");
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
            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " tries to cut a tree");

            string log = controller.name + " -> CutTreeAction \n";
            controller.CharacterAnimation.SetSwing();
            Debug.Log(log);
        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Store Axe
        }


        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {

            List<ActionInstance> instances = new List<ActionInstance>();

            CuttableTree treeTarget = owner.GetComponent<CuttableTree>();

            if (treeTarget != null)
            {

                for (int i = 0; i < controller.availableStockpiles.Count; i++)
                {
                    ActionInstance instance = new ActionInstance(this, owner, new Object[] { treeTarget, owner.transform }, new int[0]);
                    if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                    {
                        instances.Add(instance);
                    }
                }

            }
            return instances.ToArray();
        }
    }
}
