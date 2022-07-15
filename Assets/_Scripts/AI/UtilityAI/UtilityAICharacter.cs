using OutcastMayor.Building;
using OutcastMayor.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public class UtilityAICharacter : Character
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
        public float satedness;
        //how sleepy the npc is
        public float sleepy;
        private AIMovement aiMovement;

        //Temporary stuff
        public List<Construction> availableConstructions;
        public List<Stockpile> availableStockpiles;

        /// <summary>
        /// NPC State when they are not doing anything
        /// </summary>
        public State IdleState;
        /// <summary>
        /// NPC State when they are calculating a new thing to do (this might be superfluous)
        /// </summary>
        public State ThinkingState;
        /// <summary>
        /// NPC State when they are moving somewhere
        /// </summary>
        public State MoveToState;
        /// <summary>
        /// NPC State when they are performing an action
        /// </summary>
        public State PerformingState;

        protected override void Awake()
        {
            base.Awake();
            availableConstructions = new List<Construction>();
            reasoner = new Reasoner(); //??
            aiMovement = GetComponent<AIMovement>();

            //Setup states
            IdleState = new State(IdleStart, IdleUpdate, IdleExit, this, "Idle");
            PerformingState = new State(PerformingStart, PerformingUpdate, PerformingExit, this, "Performing");
            MoveToState = new State(MovingStart, MovingUpdate, MovingExit, this, "Moving");

            currentState = IdleState;
        }

        #region STATES

        void IdleStart()
        {
            timeSinceLastUpdate = updateInterval - 1.0f;
        }

        void IdleUpdate()
        {
            if (timeSinceLastUpdate > updateInterval)
            {
                //Stop old action if there is one (can this happen here?)
                if (currentAction != null)
                    currentAction.Cancel(this);
                FindNewAction();
            }
            else
            {
                timeSinceLastUpdate += Time.deltaTime;
            }
        }

        void IdleExit(State nextState)
        {

        }

        void PerformingStart()
        {
            currentAction.Init(this);
            currentAction.actionReference.onComplete.AddListener(ActionCompleted);
        }

        void PerformingUpdate()
        {
            //Should we also check for new actions periodically here?
            if(aiMovement.IsMoving())
            {
                if (timeSinceLastUpdate > updateInterval)
                {
                    FindNewAction();
                }
                else
                {
                    timeSinceLastUpdate += Time.deltaTime;
                }
            }

            currentAction.Perform(this);
        }

        void PerformingExit(State nextState)
        {
            //currentAction = null;
        }

        void MovingStart()
        {
        }

        void MovingUpdate()
        {
            if (timeSinceLastUpdate > updateInterval)
            {
                //Stop old action if there is one (can this happen here?)
                if (currentAction != null)
                    currentAction.Cancel(this);
                FindNewAction();
            }
            else
            {
                timeSinceLastUpdate += Time.deltaTime;
            }
        }

        void MovingExit(State nextState)
        {
        }

        #endregion

        public void MoveTo(Vector3 position, bool running)
        {
            if(!aiMovement.IsAlreadyArrived(position))
            {
                ChangeState(MoveToState);
                aiMovement.MoveTo(position, running, ArrivedAtLocation);
            }
            else
            {
                //Debug.Log("Agent is already there");
                ChangeState(PerformingState);
            }
        }

        public void ArrivedAtLocation()
        {
            //print("Arrived at moveToLocation");
            aiMovement.OnPathComplete.RemoveListener(ArrivedAtLocation);
            ChangeState(PerformingState);
        }

        public void ActionCompleted()
        {
            ChangeState(IdleState);
        }

        private void Update()
        {
            if (!isSleeping)
            {
                sleepy += .01f * Time.deltaTime;
                sleepy = Mathf.Clamp01(sleepy);

                currentState.Update();
            }
            else
            {
                sleepy -= .03f * Time.deltaTime;
                sleepy = Mathf.Clamp01(sleepy);
                if(sleepy == 0.0f)
                {
                    WakeUp();
                }
            }

            satedness -= .033f * Time.deltaTime;
            satedness = Mathf.Clamp01(satedness);
        }

        public void FindNewAction()
        {
            reasoner.GatherActionInstances(this);

            //Update AI
            ActionInstance newBestAction = reasoner.DetermineBestAction(this);

            //If you found a new action, do it!
            if (newBestAction != null)
            {
                currentAction = newBestAction;
                currentAction.Init(this);
            }
            else
            {
                print("no valid action was found");
            }
            timeSinceLastUpdate = 0;
        }

        public override void Sleep()
        {
            base.Sleep();
            timeSinceLastUpdate = updateInterval;
        }

        public override void Eat(Food food)
        {
            base.Eat(food);
            this.satedness += food.nourishment;
        }
    }
}