using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "SleepAction", menuName = "ScriptableObjects/UtilityAI/Actions/SleepAction", order = 1)]
    public class SleepAction : Action
    {
        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Bed bedTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Bed)
                {
                    bedTarget = instanceData[i] as Bed;
                }
            }

            Vector3 target = bedTarget.transform.position + (controller.transform.position - bedTarget.transform.position).normalized * 1.0f;
            Debug.Log("<color=green>" + controller.name + "-> SleepAction moveto.</color>");
            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Bed bedTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Bed)
                {
                    bedTarget = instanceData[i] as Bed;
                }
            }
            string log = "<color=green>" + controller.name + " -> SleepAction for " + bedTarget.gameObject.name + "</color>";

            bedTarget.Interact(controller.Interactor);
            Debug.Log(log);

            //Queue Sleeping

            controller.ActionCompleted();
        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Can you cancel sleeping?
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            ActionInstance instance = new ActionInstance(this, owner, new Object[] { owner.GetComponent<Bed>(), owner.transform }, new int[0]);
            if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
            {
                instances.Add(instance);
            }
            return instances.ToArray();
        }
    }

}
