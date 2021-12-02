using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class UtilityAIController : MonoBehaviour
    {
        /// <summary>
        /// Update Interval for this AI (in seconds)
        /// </summary>
        [SerializeField] float updateInterval;
        float timeSinceLastUpdate;

        public Action[] availableActions;
        public Action currentAction;

        //Context/Worldstates

        Reasoner reasoner;

        //how sated the npc is
        public float food;
        //how sleepy the npc is
        public float sleepy;
        public Inventory inventory;
        public Interactor interactor;
        public AIMovement aIMovement;

        //Temporary stuff
        public List<Construction> availableConstructions;
        public List<Stockpile> availableStockpiles;

        private void Awake()
        {
            availableConstructions = new List<Construction>();
            reasoner = new Reasoner(); //??
            inventory = GetComponent<Inventory>();
            interactor = GetComponent<Interactor>();
            aIMovement = GetComponent<AIMovement>();
        }

        private void Update()
        {
            food -= .033f * Time.deltaTime;
            food = Mathf.Clamp01(food);
            sleepy += .01f * Time.deltaTime;
            sleepy = Mathf.Clamp01(sleepy);
            if (timeSinceLastUpdate > updateInterval)
            {
                //Update AI
                ActionInstance newBestAction = reasoner.DetermineBestAction(availableActions, this);
                //Maybe don't do the same thing again?
                if(newBestAction != null)
                    newBestAction.actionReference.Execute(this, newBestAction.instanceData);
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

    }
}