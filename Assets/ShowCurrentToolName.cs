using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OutcastMayor
{
    public class ShowCurrentToolName : MonoBehaviour
    {
        [SerializeField]
        VectorBuilding vectorBuilding;
        TMP_Text textlabel;

        void Start()
        {
            textlabel = GetComponentInChildren<TMP_Text>();
            vectorBuilding.onChangeTool += OnUpdateTool;
            OnUpdateTool(vectorBuilding.buildMode);
        }

        void OnUpdateTool(VectorBuilding.BuildMode _buildMode)
        {
            textlabel.text = _buildMode.ToString();
        }
    }

}
