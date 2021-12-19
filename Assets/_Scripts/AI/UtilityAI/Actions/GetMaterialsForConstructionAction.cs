using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "GetMaterialsForConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetMaterialsForConstructionAction", order = 1)]
    public class GetMaterialsForConstructionAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues)
        {
            Construction constructionTarget = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if (constructionTarget == null)
            {
                Debug.LogError("No ConstructionTarget");
                return;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return;
            }

            Vector3 target = stockpileTarget.transform.position + (controller.transform.position - stockpileTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> BuildConstructionAction moveto.</color>");
            controller.aIMovement.MoveTo(target, false, () => CompleteAction(constructionTarget, stockpileTarget, controller));
        }

        public void CompleteAction(Construction constructionTarget, Stockpile stockpile, UtilityAIController controller)
        {
            string log = controller.name + " -> GetMaterialsForConstructionAction for " + constructionTarget.gameObject.name + ":\n";
            for (int i = 0; i < constructionTarget.buildRecipe.Materials.Length; i++)
            {
                Item neededItem = constructionTarget.buildRecipe.Materials[i].item;
                int neededCount = constructionTarget.GetRemainingCount(controller.interactor, i) - controller.inventory.GetTotalCount(neededItem);
                controller.inventory.Add(new Inventory.ItemStack(neededItem, neededCount));
                stockpile.inventory.Delete(neededItem, neededCount);
                log += neededItem.Name + ":" + neededCount;
            }
            Debug.Log(log);

        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            for(int i = 0; i < controller.availableConstructions.Count;i++)
            {
                ActionInstance instance = new ActionInstance(this, owner, new Object[] { controller.availableConstructions[i], owner.GetComponent<Stockpile>(), owner.transform }, new int[0]);
                if(CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                {
                    instances.Add(instance);
                }
            }

            return instances.ToArray();
        }
        public override bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
        {
            if (owner.isOccupied) return false;
            Construction constructionTarget = null;
            Stockpile stockpileTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
                if (instanceData[i] is Stockpile)
                {
                    stockpileTarget = instanceData[i] as Stockpile;
                }
            }
            if(constructionTarget == null)
            {
                Debug.LogError("No ConstructionTarget");
                return false;
            }
            if (stockpileTarget == null)
            {
                Debug.LogError("No StockpileTarget");
                return false;
            }

            //Check if the stockpile contains ANY material related to the build. if so, its fine
            for (int i = 0; i<constructionTarget.buildRecipe.Materials.Length; i++)
            {
                if(stockpileTarget.inventory.Contains(constructionTarget.buildRecipe.Materials[i].item))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
