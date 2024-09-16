using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

namespace OutcastMayor
{
    public class ImmediateDrawer : ImmediateModeShapeDrawer
    {
        VectorBuilding vectorBuilding;

        HashSet<VectorPoint> alreadyDrawnPoints = new HashSet<VectorPoint>();

        void Awake()
        {
            vectorBuilding = GetComponent<VectorBuilding>();
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                // set up static parameters. these are used for all following Draw.Line calls
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Pixels;
                Draw.Thickness = 4; // 4px wide

                foreach(VectorPointGraph graph in vectorBuilding.vectorPointGraphs)
                {
                    alreadyDrawnPoints.Clear();
                    Draw.Color = graph.graphColor;
                    foreach(VectorPoint point in graph.includedPoints)
                    {
                        foreach(VectorPoint connectedPoint in point.connectedPoints)
                        {
                            if(alreadyDrawnPoints.Contains(connectedPoint))
                            {
                                //skip
                                continue;
                            }
                            alreadyDrawnPoints.Add(point);
                            Draw.Line(point.worldPosition, connectedPoint.worldPosition);
                        }
                    }
                }
            }         
        } 
    }

}

