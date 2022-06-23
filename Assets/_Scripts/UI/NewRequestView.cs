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
            activeQuest.OnUpdateRequest += UpdateText;
            base.Show();
        }

        public override void Hide()
        {
            activeQuest.OnUpdateRequest -= UpdateText;
            activeQuest = null;
            base.Hide();
        }

        public void UpdateText(Request q)
        {
            this.titleText.text = q.title;
            if (q.isCompleted)
            {
                this.descText.text = q.description;
                goalText.text = "Speak to " + q.requester.CharacterName + " again.";
            }
            else
            {
                this.descText.text = q.description;
                string s = "";
                for (int i = 0; i < q.goals.Length; i++)
                {
                    s += "- " + q.goals[i].description + " " + q.goals[i].currentAmount + "/" + q.goals[i].requiredAmount;
                    if (i + 1 < q.goals.Length)
                        s += "\n";
                }
                goalText.text = s;
            }
        }
    }


}