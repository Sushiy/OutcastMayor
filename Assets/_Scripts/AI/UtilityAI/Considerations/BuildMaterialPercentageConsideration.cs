using OutcastMayor.Building;
using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "BuildMaterialConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/BuildMaterialConsideration", order = 1)]
    public class BuildMaterialPercentageConsideration : Consideration
    {

        public override Type[] GetRequiredDataTypes()
        {
            return new Type[] { typeof(Construction) };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Construction constructionTarget = considerationData.data[0] as Construction;

            float availableStacks = 0;
            if (constructionTarget == null) return 0.0f;

            for (int i = 0; i < constructionTarget.buildRecipe.Materials.Length; i++)
            {
                Inventory.ItemStack stack = new Inventory.ItemStack(constructionTarget.buildRecipe.Materials[i].item, constructionTarget.GetRemainingCount(controller.Interactor, i));
                if (controller.Inventory.Contains(stack))
                {
                    availableStacks += 1.0f;
                }
            }

            availableStacks /= constructionTarget.buildRecipe.Materials.Length;

            return Evaluate(availableStacks);
        }

        public override bool TryGetConsiderationData(UnityEngine.Object[] instanceData, out ConsiderationData considerationData)
        {
            Construction constructionTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Construction)
                {
                    constructionTarget = instanceData[i] as Construction;
                    break;
                }
            }

            considerationData = new ConsiderationData(this, new UnityEngine.Object[] { constructionTarget });

            return constructionTarget != null;
        }

    }
}
