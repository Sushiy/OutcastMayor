using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        public string Name;
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
        public virtual void InitReasonerData(UtilityAIController controller)
        {

        }

        //Do your action state stuff here!
        public abstract void Execute(UtilityAIController controller);
    }
}
