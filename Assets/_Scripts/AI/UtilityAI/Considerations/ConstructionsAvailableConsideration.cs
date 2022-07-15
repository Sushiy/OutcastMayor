using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    [CreateAssetMenu(fileName = "ConstructionsAvailableConsideration", menuName = "ScriptableObjects/UtilityAI/Considerations/ConstructionsAvailableConsideration", order = 1)]
    public class ConstructionsAvailableConsideration : Consideration
    {
        public override Type[] GetRequiredDataTypes()
        {
            return new Type[0];
        }

        public override string[] GetSourceValueNames()
        {
            return new string[] {"available Constructions Count" };
        }

        public override float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData data)
        {
            return new float[] { controller.availableConstructions.Count };
        }

        public override float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            if(controller.availableConstructions.Count > 0)
            {
                return Evaluate(1.0f);
            }
            else
            {
                return Evaluate(0.0f);
            }
        }

        public override bool TryGetConsiderationData(UnityEngine.Object[] instanceData, out ConsiderationData considerationData)
        {
            considerationData = new ConsiderationData(this, new UnityEngine.Object[0]);
            return true;
        }
    }
}
