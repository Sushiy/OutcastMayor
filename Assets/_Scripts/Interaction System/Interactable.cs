using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    public class Interactable : MonoBehaviour
    {
        public new string name = "Object";
        public string interaction = "Use";

        public virtual void Interact(Interactor interactor)
        {
            //print("Base Interactables don't do anything!");
        }

        public virtual void OnStartHover(Interactor interactor)
        {
            UI.UIManager.ShowHoverUI(this);
        }

        public virtual void OnEndHover(Interactor interactor)
        {
            UI.UIManager.HideHoverUI();
        }
    }
}
