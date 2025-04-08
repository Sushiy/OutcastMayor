using System.Collections;
using System.Collections.Generic;
using OutcastMayor.Building;
using OutcastMayor.UI;
using TMPro;
using UnityEngine;

namespace OutcastMayor
{
    public class BuildTool : Tool
    {
        BuildingMode buildingMode;
        public bool raycastHit
        {
            private set;
            get;
        }
        private RaycastHit hitInfo;

        [SerializeField]
        private LayerMask buildRaycastLayerMask;

        void Awake()
        {
            buildingMode = GetComponent<BuildingMode>();            
        }

        public override void Equip(Character _parentCharacter)
        {            
            gameObject.SetActive(true);
            if(buildingMode)
                buildingMode.EnterBuildMode();
        }

        public override void Unequip()
        {
            gameObject.SetActive(false);
            if(buildingMode)
                buildingMode.ExitBuildMode();
        }

        public override void OnUseToolPrimary(Character _parentCharacter)
        {
            //???
            if(buildingMode)
                buildingMode.Build();
        }

        public override void OnToolMenu()
        {
            if(buildingMode)
                UIManager.Instance.ToggleBuildingView(buildingMode);
        }

        public override void OnRotateTool(float _rotateValue)
        {
            if(buildingMode)
                buildingMode.Rotate(_rotateValue);
        }

        public override void OnRotateVerticalTool(float _value)
        {
            if(buildingMode)
                buildingMode.Alternate(_value);
        }

        public override void OnUseToolSecondary(Character _parentCharacter)
        {
            if (_parentCharacter.Interactor.hoveredInteractable is Building.Construction)
            {
                ((Building.Construction)_parentCharacter.Interactor.hoveredInteractable).Destroy();
                _parentCharacter.Interactor.hoveredInteractable.OnEndHover(_parentCharacter.Interactor);
            }
        }
        
        public override bool OnProcessRaycast(Vector3 _raycastOrigin, Vector3 _raycastDirection)
        {
            Ray ray = new Ray(_raycastOrigin, _raycastDirection);
            buildingMode.ProcessRayCast(raycastHit, ray, hitInfo);
            raycastHit = Physics.Raycast(ray, out hitInfo, 10.0f, buildRaycastLayerMask);
            return true;
        }
    }

}