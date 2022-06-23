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

        public void EndHover()
        {
            nameText.text = "";
            actionText.text = "";
        }
    }
}
