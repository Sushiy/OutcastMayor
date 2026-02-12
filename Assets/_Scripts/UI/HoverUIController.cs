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
        
        private CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void StartHover(Interactable i)
        {
            nameText.text = i.name;
            actionText.text = i.interaction;
            canvasGroup.alpha = 1;
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
                actionText.text = i.interaction;
            }
            else
            {
                actionText.text = customActionTxt;
            }
            canvasGroup.alpha = 1;
        }

        public void EndHover()
        {
            canvasGroup.alpha = 0;
            nameText.text = "";
            actionText.text = "";
        }
    }
}
