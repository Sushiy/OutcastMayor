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
            activeQuest.OnRequestCompleted += UpdateText;
            base.Show();
        }

        public override void Hide()
        {
            activeQuest.OnRequestCompleted -= UpdateText;
            activeQuest = null;
            base.Hide();
        }

        public void UpdateText(Request _request)
        {
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
                for (int i = 0; i < _request.RequestData.goals.Count; i++)
                {
                    if(_request.RequestData.goals[i].IsCompleted)
                    {
                        s += "<color=green>";
                    }
                    s += "- " + _request.RequestData.goals[i].description;
                    if (i + 1 < _request.RequestData.goals.Count)
                        s += "\n";
                    if(_request.RequestData.goals[i].IsCompleted)
                    {
                        s += "</color>";
                    }
                }
                goalText.text = s;
            }
        }
    }


}