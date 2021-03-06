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

        public override string[] GetSourceValueNames()
        {
            return new string[]{ "Carried Construction Material Stacks: ", "Required Cosntruction Material Stacks"};
        }

        public override float[] GetSourceValues(UtilityAICharacter controller,ConsiderationData data)
        {
            Construction constructionTarget = data.data[0] as Construction;

            float availableStacks = 0;
            if (constructionTarget == null)
            {
                Debug.LogError("No Construction for BuildMaterialPercentageConsideration");
                return null;
            }

            for (int i = 0; i < constructionTarget.buildRecipe.Materials.Length; i++)
            {
                Inventory.ItemStack stack = new Inventory.ItemStack(constructionTarget.buildRecipe.Materials[i].item, constructionTarget.GetRemainingCount(controller.Interactor, i));
                if (controller.Inventory.Contains(stack))
                {
                    availableStacks += 1.0f;
                }
            }

            return new float[] {availableStacks, constructionTarget.buildRecipe.Materials.Length };
        }

        protected override float CalculateScore(UtilityAICharacter controller, ConsiderationData considerationData)
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

            return availableStacks;
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
