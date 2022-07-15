using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace OutcastMayor.UI
{
    public class HoverUIController : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text actionText;

        public void StartHover(Interactable i)
        {
            nameText.text = i.name;
            actionText.text = "[E] " + i.interaction;
        }

        public void StartHover(Interactable i, string customNameTxt, string customActionTxt)
        {
            if(customNameTxt == "")
            {
                nameText.text = i.name;
            }
            else
            {
                nameText.text = customNameTxt;

            }
            if(customActionTxt == "")
            {
                actionText.text = "[E]" + i.interaction;
            }
            else
            {
                actionText.text = customActionTxt;
            }
        }

        public void EndHover()
        {
            nameText.text = "";
            actionText.text = "";
        }
    }
}
