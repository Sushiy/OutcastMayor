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

        //Init stuff here?
        public virtual void Awake()
        {

        }

        public virtual bool CheckInstanceRequirement(SmartObject owner, Object[] instanceData, int[] instanceValues)
        {
            return !owner.isOccupied;
        }

        public abstract ActionInstance[] GetActionInstances(SmartObject owner, UtilityAIController controller);

        //Do your action state stuff here!
        public abstract void Execute(UtilityAIController controller, Object[] instanceData, int[] instanceValues);
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

        //Start this action "state"
        public virtual void OnEnter()
        {
            owner.isOccupied = true;
        }

        //Stop this action "state"
        public virtual void OnExit()
        {
            owner.isOccupied = false;
        }
    }
}
