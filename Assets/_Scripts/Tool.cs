using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace OutcastMayor
{
    public abstract class Tool : MonoBehaviour
    {
        public abstract void Equip(Character _parentCharacter);

        public virtual void Unequip()
        {

        }

        public abstract void OnUseToolPrimary(Character _parentCharacter);
        public virtual void OnUseToolSecondary(Character _parentCharacter)
        {

        }
        public virtual void OnUseToolTertiary(Character _parentCharacter)
        {

        }
        public virtual void OnToolAnimationEvent(string _evt)
        {

        }
        public virtual void OnToolMenu()
        {

        }

        public virtual void OnRotateTool(float _value)
        {

        }

        public virtual void OnRotateVerticalTool(float _value)
        {

        }

        public virtual bool OnProcessRaycast(Vector3 _raycastOrigin, Vector3 _raycastDirection)
        {
            return false;
        }
    }

}
