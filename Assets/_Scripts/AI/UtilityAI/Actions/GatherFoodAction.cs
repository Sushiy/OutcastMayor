using OutcastMayor.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    /// <summary>
    /// This action has similar conditions to "EAT", but only picks up a food item to enable eat actions
    /// </summary>
    [CreateAssetMenu(fileName = "GatherAndEatAction", menuName = "ScriptableObjects/UtilityAI/Actions/GatherAndEatAction", order = 1)]
    public class GatherFoodAction : Action
    {
        public override void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " went to eat");
            //Do stuff once at the beginning
            Transform moveTarget = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is Transform)
                {
                    moveTarget = instanceData[i] as Transform;
                }
            }

            Vector3 target = moveTarget.transform.position + (controller.transform.position - moveTarget.transform.position).normalized * 1.0f;
            controller.MoveTo(target, false);
        }

        public override void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            Debug.Log(controller.name + " starts to eat");

            ItemStackInstance itemStack = null;
            for (int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] is ItemStackInstance)
                {
                    itemStack = instanceData[i] as ItemStackInstance;
                }
            }
            if (itemStack == null)
            {
                Debug.LogError("No itemStack");
                return;
            }

            itemStack.Interact(controller.Interactor);
            string log = controller.name + " -> GatherAndEatAction \n";
            Debug.Log(log);
            controller.ActionCompleted();
        }
        public override void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues)
        {
            //Stop Animations or something?
            //Put away the food if you still have it
        }

        public override ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller)
        {
            List<ActionInstance> instances = new List<ActionInstance>();

            ItemStackInstance itemStackInstance = owner.GetComponent<ItemStackInstance>();

            if (itemStackInstance != null)
            {

                for (int i = 0; i < controller.availableStockpiles.Count; i++)
                {
                    ActionInstance instance = new ActionInstance(this, owner, new Object[] { itemStackInstance, owner.transform }, new int[0]);
                    if (CheckInstanceRequirement(owner, instance.instanceData, instance.instanceValues))
                    {
                        instances.Add(instance);
                    }
                }

            }
            return instances.ToArray();
        }
    }
}
