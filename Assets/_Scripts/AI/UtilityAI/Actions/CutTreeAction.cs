using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "CutTreeAction", menuName = "ScriptableObjects/UtilityAI/Actions/CutTreeAction", order = 1)]
    public class CutTreeAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " goes to cut a tree");

            Tree treeTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Tree)
                {
                    treeTarget = instanceData[i] as Tree;
                }
            }
            if (treeTarget == null)
            {
                Debug.LogError("No treeTarget");
                return;
            }

            Vector3 target = treeTarget.transform.position + (controller.transform.position - treeTarget.transform.position).normalized * .5f;
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(treeTarget, controller));
        }

        public void CompleteAction(Tree treeTarget, UtilityAIController controller)
        {
            string log = controller.name + " -> CutTreeAction \n";
            controller.CharacterAnimation.SetSwing();
            Debug.Log(log);
        }


        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {

            List<ActionInstance> instances = new List<ActionInstance>();

            Tree treeTarget = owner.GetComponent<Tree>();

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
