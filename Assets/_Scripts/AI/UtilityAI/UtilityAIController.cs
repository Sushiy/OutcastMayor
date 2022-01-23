using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class UtilityAIController : Character
    {
        /// <summary>
        /// Update Interval for this AI (in seconds)
        /// </summary>
        [SerializeField] float updateInterval;
        float timeSinceLastUpdate;

        public Action[] availableActions;
        public ActionInstance currentAction;

        //Context/Worldstates

        Reasoner reasoner;

        //how sated the npc is
        public float food;
        //how sleepy the npc is
        public float sleepy;
        public AIMovement aIMovement;

        //Temporary stuff
        public List<Construction> availableConstructions;
        public List<Stockpile> availableStockpiles;

        protected override void Awake()
        {
            base.Awake();
            availableConstructions = new List<Construction>();
            reasoner = new Reasoner(); //??
            aIMovement = GetComponent<AIMovement>();
        }

        private void Update()
        {
            food -= .033f * Time.deltaTime;
            food = Mathf.Clamp01(food);
            if(!isSleeping)
            {
                sleepy += .01f * Time.deltaTime;
                sleepy = Mathf.Clamp01(sleepy);
                if (timeSinceLastUpdate > updateInterval)
                {
                    if (currentAction != null)
                        currentAction.OnExit();
                    reasoner.GatherActionInstances(this);
                    //Update AI
                    ActionInstance newBestAction = reasoner.DetermineBestAction(availableActions, this);
                    //Maybe don't do the same thing again?
                    if (newBestAction != null)
                    {
                        currentAction = newBestAction;
                        currentAction.OnEnter();
                        currentAction.actionReference.Execute(this, newBestAction.instanceData, newBestAction.instanceValues);
                    }
                    else
                    {
                        print("no valid action was found");
                    }
                    timeSinceLastUpdate = 0;
                }
                else
                {
                    timeSinceLastUpdate += Time.deltaTime;
                }

            }
            else
            {
                sleepy -= .1f * Time.deltaTime;
                sleepy = Mathf.Clamp01(sleepy);
                if(sleepy == 0)
                {
                    WakeUp();
                }
            }
        }

        public override void Sleep()
        {
            base.Sleep();
            timeSinceLastUpdate = updateInterval;
        }

    }
}