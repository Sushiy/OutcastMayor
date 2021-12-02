using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        public string Name;
        public float weight = 1.0f;
        public Consideration[] considerations;

        //Init stuff here?
        public virtual void Awake()
        {

        }

        //Start this action "state"
        public virtual void OnEnter()
        {

        }

        //Stop this action "state"
        public virtual void OnExit()
        {

        }
        public abstract ActionInstance[] GetActionInstances(UtilityAIController controller);

        //Do your action state stuff here!
        public abstract void Execute(UtilityAIController controller, Object[] instanceData);
    }

    public class ActionInstance
    {
        public Action actionReference;

        public Object[] instanceData;

        public ActionInstance(Action actionReference, Object[] instanceData)
        {
            this.actionReference = actionReference;
            this.instanceData = new Object[instanceData.Length];
            instanceData.CopyTo(this.instanceData, 0);
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
    }
}
