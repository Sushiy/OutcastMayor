using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public class Reasoner
    {
        List<ActionInstance> actionInstances;

        public void GatherActionInstances(UtilityAICharacter controller)
        {
            string s = "<b>" + controller.name + " Gathering Action Instances </b> \n";
            if (actionInstances == null)
            {
                actionInstances = new List<ActionInstance>();
            }
            else
            {
                actionInstances.Clear();
            }
            //Add all Actions native to the character
            for( int i = 0; i < controller.availableActions.Length; i++)
            {
                ActionInstance[] instances = controller.availableActions[i].GetActionInstances(Blackboard.smartObjects[i], controller);
                actionInstances.AddRange(instances);
            }
            s += "- Added " + actionInstances.Count + " instances from " + controller.availableActions.Length + " intrinsic Actions \n";

            //Add all actions from smartobjects
            int countAds = 0;
            int countInstances2 = 0;
            for (int i = 0; i < Blackboard.smartObjects.Count; i++)
            {
                countAds += Blackboard.smartObjects[i].advertisements.Length;
                for (int a = 0; a < Blackboard.smartObjects[i].advertisements.Length; a++)
                {
                    if(Blackboard.smartObjects[i].advertisements[a].advertisedAction == null)
                    {
                        Debug.LogError("No advertisements");
                    }
                    ActionInstance[] instances = Blackboard.smartObjects[i].advertisements[a].advertisedAction.GetActionInstances(Blackboard.smartObjects[i], controller);
                    actionInstances.AddRange(instances);
                    countInstances2 += instances.Length;
                }
            }
            s += "- Added " + countInstances2 + " instances from " + Blackboard.smartObjects.Count + " smartObjects with " + countAds + " advertisements \n";
            s += "Total Instances: " + (actionInstances.Count);
            Debug.Log(s);
            actionInstances.Sort((p1, p2) => p2.actionReference.weight.CompareTo(p1.actionReference.weight));
        }

        public ActionInstance DetermineBestAction(UtilityAICharacter controller)
        {

            float bestScore = -1;
            ActionInstance bestAction = null;
            string log = "<b>Utility Log for " + controller.name + ":</b>\n";

            log += actionInstances.Count + " available actions Instances.\n";

            log += "<b> Scoring ActionInstances:</b>\n";
            for (int i = 0; i < actionInstances.Count; i++)
            {
                if (actionInstances[i].actionReference.weight < bestScore)
                {
                    log += "-" + actionInstances[i].actionReference.Name + "(" + actionInstances[i].InstanceDataToString() + ")\n";
                    log += "    -Weight " + actionInstances[i].actionReference.weight + " is lower than bestScore: " + bestScore + " \n"; 
                }
                else
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
            }
            Debug.Log(log);
            if (bestScore > 0.0f)
                return bestAction;
            else
                return null;
        }

        public float ScoreAction(ActionInstance actionInstance, UtilityAICharacter controller, out string log)
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
                    if (considerationData == null || considerationData.data == null)
                    {
                        log += "ConsiderationData is null \n";
                    }
                    else
                    {
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
                            log += "    -" + consideration.Name + ": Score is 0 and Consideration is exclusive \n";
                            return 0.0f;
                        }
                    }
                    actionScore *= considerationScore;

                }
                else
                {
                    log += "    -" + consideration.Name + ": could not get ConsiderationData \n";
                    return 0.0f;
                }

            }

            float originalScore = actionScore;
            float modFactor = 1 - (1 / actionInstance.actionReference.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            actionScore = originalScore + (makeupValue * originalScore);
            actionScore = Mathf.Clamp01(actionScore);
            float weightedScore = actionScore * actionInstance.actionReference.weight;
            log += " result:" + actionScore + "->" + weightedScore + "\n";
            return weightedScore;
        }
    }

}
