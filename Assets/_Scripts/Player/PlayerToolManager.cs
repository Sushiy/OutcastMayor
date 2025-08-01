using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{    public class PlayerToolManager : MonoBehaviour
    {
        public Tool activeTool;

        private Character parentCharacter;

        private void Awake()
        {
            parentCharacter = GetComponentInParent<Character>();
            parentCharacter.OnHeldItemChanged.AddListener(OnHeldItemChanged);
        }

        public void OnHeldItemChanged(GameObject heldObject)
        {
            if(activeTool)
                activeTool.Unequip();

            activeTool = heldObject.GetComponent<Tool>();
            if(activeTool != null)
                activeTool.Equip(parentCharacter);
        }

        public void ToolPrimary()
        {
            if(activeTool != null)
                activeTool.OnUseToolPrimary(parentCharacter);
        }
        public void ToolSecondary()
        {
            if(activeTool != null)
                activeTool.OnUseToolSecondary(parentCharacter);
        }
        public void ToolTertiary()
        {
            if(activeTool != null)
                activeTool.OnUseToolTertiary(parentCharacter);
        }


        public void ToolMenu()
        {
            if(activeTool != null)
            {
                activeTool.OnToolMenu();
            }
        }

        public void ToolRotate(float _value, bool _isModifierDown)
        {
            if(activeTool != null)
            {
                activeTool.OnRotateTool(_value, _isModifierDown);
            }
        }

        public void OnToolAnimationEvent(string _value)
        {
            if (activeTool)
                activeTool.OnToolAnimationEvent(_value);
        }

        public bool OnToolRaycast(Vector3 _raycastOrigin, Vector3 _raycastDirection)
        {
            if (activeTool)
                return activeTool.OnProcessRaycast(_raycastOrigin, _raycastDirection);
            else
                return false;
        }
    }
}