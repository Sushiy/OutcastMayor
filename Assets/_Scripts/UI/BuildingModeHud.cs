using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using OutcastMayor.Building;
using UnityEngine;

namespace OutcastMayor.UI
{
    public class BuildingModeHUD : UIPanel
    {
        BuildingMode buildingMode;

        [SerializeField]
        TMPro.TMP_Text rotationModeText;
        string[] RotationModeStrings = {"World Align", "View Align", "Surface Align"};

        void Start()
        {
            buildingMode = Player.Instance.BuildingMode;
            buildingMode.onBuildRotationModeChanged += BuildRotationModeChanged;
            BuildRotationModeChanged(buildingMode.buildRotationMode);
        }

        void OnDestroy()
        {
            if(buildingMode != null)
                buildingMode.onBuildRotationModeChanged -= BuildRotationModeChanged;            
        }

        void BuildRotationModeChanged(BuildingMode.BuildRotationMode _buildRotationMode)
        {
            print($"[BuildingModeHUD] BuildRotationModeChanged {_buildRotationMode}");
            rotationModeText.text = RotationModeStrings[(int)_buildRotationMode];
        }
    }
    
}
