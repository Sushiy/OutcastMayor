using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "FoodNourismentConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/FoodNourismentConsideration", order = 1)]
    public class FoodNourishmentConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[] { typeof(Food) };
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] {"Hunger", "Nourishment" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Food food = considerationData.data[0] as Food;
            float hunger = maxValue - controller.satedness;
            return new float[] { hunger, food.nourishment };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            Food food = considerationData.data[0] as Food;
            float hunger = maxValue - controller.satedness;
            float f = hunger - food.nourishment;
            return Evaluate(Mathf.Abs(f));
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            Food food = null;
            for(int i = 0; i < instanceData.Length;i++)
            {
                food = instanceData[i] as Food;
                break;
            }

            considerationData = new ConsiderationData(this, new Object[] { food });

            if (food == null)
            {
                Debug.Log("No food" + instanceData.Length);
                return false;
            }


            return true;
        }
    }
}
