using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        public string Name;
        public float weight = 1.0f;
        [SerializeReference]
        public Consideration[] considerations;

        public UnityEngine.Events.UnityEvent onComplete;

        //Init stuff here?
        public virtual void Awake()
        {

        }

        public virtual bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
        {
            return !owner.isOccupied;
        }

        public abstract ActionInstance[] GetActionInstances(SmartObject owner, UtilityAICharacter controller);


        //This is called right when the action is chosen, and determines which state the character should go into etc.
        public abstract void Init(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues);

        /// <summary>
        /// This method controls the actual Action and is called repeatedly by the character, while it is in the performing state
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="instanceData"></param>
        /// <param name="instanceValues"></param>
        public abstract void Perform(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues);

        /// <summary>
        /// This is called when the action is cancelled
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="instanceData"></param>
        /// <param name="instanceValues"></param>
        public abstract void Cancel(UtilityAICharacter controller, Object[] instanceData, int[] instanceValues);
    }

    public class ActionInstance
    {
        public Action actionReference;

        public SmartObject owner;

        public Object[] instanceData;

        public int[] instanceValues;

        public ActionInstance(Action actionReference, SmartObject owner, Object[] instanceData, int[] instanceValue)
        {
            this.actionReference = actionReference;
            this.owner = owner;
            this.instanceData = new Object[instanceData.Length];
            instanceData.CopyTo(this.instanceData, 0);
            if(instanceValues != null)
            {
                Debug.Log("byte array is not null");
                this.instanceValues = new int[instanceValues.Length];
                instanceValues.CopyTo(this.instanceValues, 0);
            }
            else
            {
                this.instanceValues = new int[0];
            }
        }

        public string InstanceDataToString()
        {
            string result = "";
            for(int i = 0; i < instanceData.Length; i++)
            {
                if (instanceData[i] == null)
                    continue;
                if(i == instanceData.Length-1)
                {
                    result += instanceData[i].name;
                }
                else
                {
                    result += instanceData[i].name + ", ";
                }
            }
            return result;
        }

        //Init this action
        public virtual void Init(UtilityAICharacter controller)
        {
            owner.isOccupied = true;
            actionReference.Init(controller, instanceData, instanceValues);
        }

        public virtual void Perform(UtilityAICharacter controller)
        {
            actionReference.Perform(controller, instanceData, instanceValues);
        }

        //Stop this action "state"
        public virtual void Cancel(UtilityAICharacter controller)
        {
            owner.isOccupied = false;
            actionReference.Cancel(controller, instanceData, instanceValues);
        }
    }
}
