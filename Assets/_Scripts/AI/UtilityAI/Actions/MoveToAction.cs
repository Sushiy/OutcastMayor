using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "NewMoveToAction", menuName = "ScriptableObjects/UtilityAI/Actions/MoveToAction", order = 1)]
    public class MoveToAction : Action
    {
        public override void Execute(UtilityAIController controller)
        {
            Debug.Log(controller.name + " is moving...where?");
        }
    }
}
