using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Reasoner
    {
        public Action DetermineBestAction(Action[] actions, UtilityAIController controller)
        {
            float bestScore = -1;
            Action bestAction = null;
            string log = "<b>Utility Log:</b>\n";
            for(int i = 0; i < actions.Length; i++)
            {

                float score = ScoreAction(actions[i], controller);
                log += "-" + actions[i].Name + " " + score + "\n";
                if(score > bestScore)
                {
                    bestScore = score;
                    bestAction = actions[i];
                }
            }
            Debug.Log(log);
            if (bestScore > 0.0f)
                return bestAction;
            else
                return null;
        }

        public float ScoreAction(Action action, UtilityAIController controller)
        {
            float sum = 0;

            action.InitReasonerData(controller);

            //Average score
            for(int i = 0; i < action.considerations.Length; i++)
            {
                float score = action.considerations[i].ScoreConsideration(action, controller);
                if(score == 0.0f && action.considerations[i].isExclusiveConsideration)
                {
                    return 0.0f;
                }
                sum += score;
            }
            return sum / action.considerations.Length;
        }
    }

}
