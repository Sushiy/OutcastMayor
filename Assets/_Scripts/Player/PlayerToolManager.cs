using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{    public class PlayerToolManager : MonoBehaviour
    {
        public SwingableTool activeTool;

        private Character parentCharacter;

        private void Awake()
        {
            parentCharacter = GetComponentInParent<Character>();
            parentCharacter.OnHeldItemChanged.AddListener(OnHeldItemChanged);
        }

        public void OnHeldItemChanged(GameObject heldObject)
        {
            activeTool = null;

            activeTool = heldObject.GetComponent<SwingableTool>();
            if(activeTool != null)
                activeTool.Equip();
        }

        public void SwingTool()
        {
            if (activeTool)
                parentCharacter.CharacterAnimation.SetSwing();
        }

        public void OnSwingStart()
        {
            if (activeTool)
                activeTool.OnStartSwing();
        }

        public void OnSwingEnd()
        {
            if (activeTool)
                activeTool.OnEndSwing();
        }
    }
}