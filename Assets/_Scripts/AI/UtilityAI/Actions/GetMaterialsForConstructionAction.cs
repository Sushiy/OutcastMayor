using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "GetMaterialsForConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetMaterialsForConstructionAction", order = 1)]
    public class GetMaterialsForConstructionAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData)
        {
            Construction constructionTarget = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
                if(instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }

            Vector3 target = stockpileTarget.transform.position + (controller.transform.position - stockpileTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> BuildConstructionAction moveto.</color>");
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(constructionTarget, controller));
        }

        public void CompleteAction(Construction constructionTarget, UtilityAIController controller)
        {
            string log = controller.name + " -> GetMaterialsForConstructionAction for " + constructionTarget.gameObject.name + ":\n";
            for (int i = 0; i < constructionTarget.buildRecipe.materials.Length; i++)
            {
                Item neededItem = constructionTarget.buildRecipe.materials[i].item;
                int neededCount = constructionTarget.GetRemainingCount(controller.interactor, i) - controller.inventory.GetTotalCount(neededItem);
                controller.inventory.Add(new Inventory.ItemStack(neededItem, neededCount));
                log += neededItem.Name + ":" + neededCount;
            }
            Debug.Log(log);

        }

        public override ActionInstance[] GetActionInstances(UtilityAIController controller)
        {
            ActionInstance[] instances = new ActionInstance[controller.availableConstructions.Count * controller.availableStockpiles.Count];

            for (int i = 0; i < controller.availableConstructions.Count; i++)
            {
                for(int j = 0; j < controller.availableStockpiles.Count;j++)
                {
                    instances[j + i* controller.availableStockpiles.Count] = new ActionInstance(this, new Object[] { controller.availableConstructions[i], controller.availableStockpiles[j], controller.availableStockpiles[j].transform});
                }
            }

            return instances;
        }
    }

}
