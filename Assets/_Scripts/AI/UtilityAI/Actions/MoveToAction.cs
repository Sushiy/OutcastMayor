using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "NewMoveToAction", menuName = "ScriptableObjects/UtilityAI/Actions/MoveToAction", order = 1)]
    public class MoveToAction : Action
    {
        public override void Execute(UtilityAIController controller, Object[] instanceData)
        {
            Debug.Log(controller.name + " is moving...where?");
        }

        public override ActionInstance[] GetActionInstances(UtilityAIController controller)
        {
            ActionInstance[] result = new ActionInstance[] { new ActionInstance(this, new Object[0]) };
            return result;
        }
    }
}
