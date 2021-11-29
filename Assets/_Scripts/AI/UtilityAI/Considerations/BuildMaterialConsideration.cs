using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "BuildMaterialConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/BuildMaterialConsideration", order = 1)]
    public class BuildMaterialConsideration : Consideration
    {
        public override float ScoreConsideration(Action action, UtilityAIController controller)
        {
            //!!! TODO: How do I get arbitrary info into the considerations? weird!
            //stack = action as

            float availableStacks = 0;
            if (controller.chosenConstruction == null) return 0.0f;
            for (int i = 0; i < controller.chosenConstruction.buildRecipe.materials.Length; i++)
            {
                Inventory.ItemStack stack = new Inventory.ItemStack(controller.chosenConstruction.buildRecipe.materials[i].item, controller.chosenConstruction.GetRemainingCount(controller.interactor, i));
                if (controller.inventory.Contains(stack))
                {
                    availableStacks += 1.0f;
                }
            }

            availableStacks /= controller.chosenConstruction.buildRecipe.materials.Length;

            return curve.Evaluate(availableStacks);
        }
    }
}
