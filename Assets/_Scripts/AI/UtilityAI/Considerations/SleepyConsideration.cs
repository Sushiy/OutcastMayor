using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "SleepyConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/SleepyConsideration", order = 1)]
    public class SleepyConsideration : Consideration
    {
        public override System.Type[] GetRequiredDataTypes()
        {
            return new System.Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] { "Sleepiness" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return new float[] { controller.sleepy };
        }

        protected override float CalculateScore(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            return controller.sleepy;
        }
        public override bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new Object[0]);

            return true;
        }
    }
}
