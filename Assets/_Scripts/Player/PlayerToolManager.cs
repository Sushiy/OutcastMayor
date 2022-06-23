using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{    public class PlayerToolManager : MonoBehaviour
    {
        public SwingableTool[] tools;

        public SwingableTool activeTool;

        private Character parentCharacter;

        private void Awake()
        {
            parentCharacter = GetComponentInParent<Character>();
            EquipTool(0);
        }

        public void EquipTool(int i)
        {
            activeTool = tools[i];
            activeTool.Equip();
        }

        public void UnequipTool()
        {
            activeTool.gameObject.SetActive(false);
            activeTool = null;
        }

        public void SwingTool()
        {
            parentCharacter.CharacterAnimation.SetSwing();
        }

        public void OnSwingStart()
        {
            activeTool.OnStartSwing();
        }

        public void OnSwingEnd()
        {
            activeTool.OnEndSwing();
        }
    }
}