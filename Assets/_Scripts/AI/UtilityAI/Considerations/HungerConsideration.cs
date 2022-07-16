using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/HungerConsideration", order = 1)]
    public class HungerConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] { "Hunger" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return new float[] { 1.0f - controller.satedness};
        }

        protected override float CalculateScore(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return 1.0f - controller.satedness;
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
