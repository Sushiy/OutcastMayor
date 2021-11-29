using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "GetMaterialsForConstructionAction", menuName = "ScriptableObjects/UtilityAI/Actions/GetMaterialsForConstructionAction", order = 1)]
    public class GetMaterialsForConstructionAction : Action
    {
        Construction construction;

        public override void InitReasonerData(UtilityAIController controller)
        {
            if(controller.availableConstructions.Count == 0)
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
            string log = controller.name + " -> GetMaterialsForConstructionAction for " + construction.gameObject.name + ":\n";
            for (int i = 0; i < construction.buildRecipe.materials.Length; i++)
            {
                Item neededItem = construction.buildRecipe.materials[i].item;
                int neededCount = construction.GetRemainingCount(controller.interactor, i) - controller.inventory.GetTotalCount(neededItem);
                controller.inventory.Add(new Inventory.ItemStack(neededItem, neededCount));
                log += neededItem.Name + ":" + neededCount;
            }
            Debug.Log(log);
        }
    }

}
