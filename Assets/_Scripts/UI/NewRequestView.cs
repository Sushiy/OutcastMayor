using OutcastMayor.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UI
{
    public class NewRequestView : UIPanel
    {
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text descText;
        [SerializeField] private TMPro.TMP_Text goalText;

        Request activeQuest;
        public void Show(Request q)
        {
            activeQuest = q;
            UpdateText(activeQuest);
            activeQuest.OnRequestUpdated += UpdateText;
            base.Show();
        }

        public override void Hide()
        {
            activeQuest.OnRequestUpdated -= UpdateText;
            activeQuest = null;
            base.Hide();
        }

        public void UpdateText(Request _request)
        {
            print($"UPDATE REQUEST TEXT {_request.RequestData.questID} : {_request.IsCompleted}");
            this.titleText.text = _request.RequestData.title;
            if (_request.IsCompleted)
            {
                this.descText.text = _request.RequestData.description;
                goalText.text = "Speak to " + _request.RequestData.requester.CharacterName + " again.";
            }
            else
            {
                this.descText.text = _request.RequestData.description;
                string s = "";
                for (int i = 0; i < _request.goals.Length; i++)
                {
                    if(_request.goals[i].IsCompleted)
                    {
                        s += "<color=green>";
                    }
                    s += "- " + _request.goals[i].GetGoalDescription();
                    if(_request.goals[i].IsCompleted)
                    {
                        s += "</color>";
                    }
                    if (i + 1 < _request.goals.Length)
                        s += "\n";
                }
                goalText.text = s;
            }
        }
    }


}