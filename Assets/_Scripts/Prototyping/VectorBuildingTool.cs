using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OutcastMayor.VectorBuilding
{
    [RequireComponent(typeof(VectorBuilding))]
    public abstract class VectorBuildingTool : MonoBehaviour
    {
        protected VectorBuilding vectorBuilding;

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            vectorBuilding = GetComponent<VectorBuilding>();
        }

        public abstract void OnClick(Vector3 _clickPosition, ControlElement _clickedControl);
    }

}
