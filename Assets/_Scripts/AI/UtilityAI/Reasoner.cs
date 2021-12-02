using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Reasoner
    {
        List<ActionInstance> actionInstances;

        public ActionInstance DetermineBestAction(Action[] actions, UtilityAIController controller)
        {
            if (actionInstances == null)
            {
                actionInstances = new List<ActionInstance>();
            }
            else
            {
                actionInstances.Clear();
            }

            float bestScore = -1;
            ActionInstance bestAction = null;
            string log = "<b>Utility Log:</b>\n";
            log += actions.Length + " available actions.\n";
            for (int i = 0; i < actions.Length; i++)
            {
                ActionInstance[] instances = actions[i].GetActionInstances(controller);
                log += "-" + actions[i].Name + " generated " + instances.Length + "instances.\n";
                actionInstances.AddRange(instances);
            }

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

            //Dave Mark averaging
            for (int i = 0; i < actionInstance.actionReference.considerations.Length; i++)
            {
                float considerationScore = actionInstance.actionReference.considerations[i].ScoreConsideration(actionInstance.actionReference, controller, actionInstance.instanceData);
                log += "    -" + actionInstance.actionReference.considerations[i].Name + ":" + considerationScore + "\n";
                if (considerationScore == 0.0f && actionInstance.actionReference.considerations[i].isExclusiveConsideration)
                {
                    return 0.0f;
                }
                actionScore *= considerationScore;
            }

            float originalScore = actionScore;
            float modFactor = 1 - (1 / actionInstance.actionReference.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            actionScore = originalScore + (makeupValue * originalScore);
            log += " result:" + originalScore + "->" +  actionScore +"\n";
            return Mathf.Clamp01(actionScore) * actionInstance.actionReference.weight;
        }
    }

}
