using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "BuildMaterialConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/BuildMaterialConsideration", order = 1)]
    public class BuildMaterialPercentageConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller, Object[] instanceData)
        {
            Construction constructionTarget = null;
            for(int i = 0; i < instanceData.Length; i++)
            {
                if(instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                }
            }

            //!!! TODO: How do I get arbitrary info into the considerations? weird!
            //stack = action as

            float availableStacks = 0;
            if (constructionTarget == null) return 0.0f;

            for (int i = 0; i < constructionTarget.buildRecipe.Materials.Length; i++)
            {
                Inventory.ItemStack stack = new Inventory.ItemStack(constructionTarget.buildRecipe.Materials[i].item, constructionTarget.GetRemainingCount(controller.interactor, i));
                if (controller.inventory.Contains(stack))
                {
                    availableStacks += 1.0f;
                }
            }

            availableStacks /= constructionTarget.buildRecipe.Materials.Length;

            return Evaluate(availableStacks);
        }
    }
}
