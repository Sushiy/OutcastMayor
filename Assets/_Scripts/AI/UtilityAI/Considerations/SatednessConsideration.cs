using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "SatednessConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/SatednessConsideration", order = 1)]
    public class SatednessConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] { "Satedness" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return new float[] { controller.satedness / maxValue };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return Evaluate(controller.satedness/maxValue);
        }

        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
