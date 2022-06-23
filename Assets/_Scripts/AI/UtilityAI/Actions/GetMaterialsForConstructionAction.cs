using OutcastMayor.Building;
using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "GetMaterialsForConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetMaterialsForConstructionAction", order = 1)]
    public class GetMaterialsForConstructionAction : Action
    {
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
            string log = controller.name + " -> GetMaterialsForConstructionAction for " + constructionTarget.gameObject.name + ":\n";
            for (int i = 0; i < constructionTarget.buildRecipe.Materials.Length; i++)
            {
                Item neededItem = constructionTarget.buildRecipe.Materials[i].item;
                int neededCount = constructionTarget.GetRemainingCount(controller.Interactor, i) - controller.Inventory.GetTotalCount(neededItem);
                controller.Inventory.Add(new Inventory.ItemStack(neededItem, neededCount));
                stockpileTarget.inventory.Delete(neededItem, neededCount);
                log += neededItem.DisplayName + ":" + neededCount;
            }
            Debug.Log(log);
        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Drop the materials you got
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
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
