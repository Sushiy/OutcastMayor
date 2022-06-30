using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "FoodNourismentConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/FoodNourismentConsideration", order = 1)]
    public class FoodNourismentConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[] { typeof(Food) };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Food food = considerationData.data[0] as Food;
            return Evaluate(maxValue - (controller.satedness - food.nourishment));
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            Food food = null;
            for(int i = 0; i < instanceData.Length;i++)
            {
                food = instanceData[i] as Food;
            }

            considerationData = new ConsiderationData(this, new Object[] {food});

            return true;
        }
    }
}
