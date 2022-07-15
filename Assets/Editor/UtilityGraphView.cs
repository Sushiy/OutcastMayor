using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class UtilityGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<UtilityGraphView, GraphView.UxmlTraits> { }

    public UtilityGraphView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        //this.AddManipulator(new SelectionDragger());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UtillityReasonerWindow.uss");
        styleSheets.Add(styleSheet);

    }
}
