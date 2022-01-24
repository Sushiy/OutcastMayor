using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Reasoner
    {
        List<ActionInstance> actionInstances;

        public void GatherActionInstances(UtilityAIController controller)
        {
            if (actionInstances == null)
            {
                actionInstances = new List<ActionInstance>();
            }
            else
            {
                actionInstances.Clear();
            }
            for( int i = 0; i < controller.availableActions.Length; i++)
            {
                ActionInstance[] instances = controller.availableActions[i].GetActionInstances(Blackboard.smartObjects[i], controller);
                actionInstances.AddRange(instances);
            }

            Debug.Log("Found " + Blackboard.smartObjects.Count + " smartObjects");
            for (int i = 0; i < Blackboard.smartObjects.Count; i++)
            {
                for(int a = 0; a < Blackboard.smartObjects[i].advertisements.Length; a++)
                {
                    if(Blackboard.smartObjects[i].advertisements[a].advertisedAction == null)
                    {
                        Debug.LogError("No advertisements");
                    }
                    ActionInstance[] instances = Blackboard.smartObjects[i].advertisements[a].advertisedAction.GetActionInstances(Blackboard.smartObjects[i], controller);
                    actionInstances.AddRange(instances);
                }
            }

        }

        public ActionInstance DetermineBestAction(UtilityAIController controller)
        {

            float bestScore = -1;
            ActionInstance bestAction = null;
            string log = "<b>Utility Log:</b>\n";

            log += actionInstances.Count + " available actions Instances.\n";

            log += "<b> Scoring ActionInstances:</b>\n";
            for (int i = 0; i < actionInstances.Count; i++)
            {
                string s;
                float score = ScoreAction(actionInstances[i], controller, out s);
                log += s;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = actionInstances[i];
                }
            }
            Debug.Log(log);
            if (bestScore > 0.0f)
                return bestAction;
            else
                return null;
        }

        public float ScoreAction(ActionInstance actionInstance, UtilityAIController controller, out string log)
        {
            log = "-" + actionInstance.actionReference.Name + "(" + actionInstance.InstanceDataToString() + ")\n";
            float actionScore = 1;

            //Dave Mark averaging: Tries to counteract that product average go smaller all the time
            for (int i = 0; i < actionInstance.actionReference.considerations.Length; i++)
            {

                Consideration consideration = actionInstance.actionReference.considerations[i];
                //Try to get the considerationData necessary for this consideration from the actioninstance data
                ConsiderationData considerationData;
                if(consideration.TryGetConsiderationData(actionInstance.instanceData, out considerationData))
                {
                    float considerationScore = 0;
                    if (controller.CheckConsiderationMemory(considerationData, out considerationScore))
                    {
                        log += "    -(Remembered)" + consideration.Name + ":" + considerationScore + "\n";
                    }
                    else
                    {
                        considerationScore = consideration.ScoreConsideration(controller, considerationData);
                        controller.AddToConsiderationMemory(considerationData, considerationScore);
                        log += "    -" + consideration.Name + ":" + considerationScore + "\n";
                    }
                    if (considerationScore == 0.0f && consideration.isExclusiveConsideration)
                    {
                        return 0.0f;
                    }
                    actionScore *= considerationScore;

                }
                else
                {
                    return 0.0f;
                }

            }

            float originalScore = actionScore;
            float modFactor = 1 - (1 / actionInstance.actionReference.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            actionScore = originalScore + (makeupValue * originalScore);
            actionScore = Mathf.Clamp01(actionScore) * actionInstance.actionReference.weight;
            log += " result:" + originalScore + "->" +  actionScore +"\n";
            return actionScore;
        }
    }

}
