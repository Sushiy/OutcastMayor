using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Unity.VisualScripting;

namespace OutcastMayor
{
    public class ImmediateDrawer : ImmediateModeShapeDrawer
    {
        VectorBuilding vectorBuilding;

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
                    Draw.Color = graph.graphColor;
                    foreach(VectorEdge edge in graph.edges)
                    {
                        if(edge.isInside)
                            Draw.Color = graph.graphColor * .0f;
                        else
                            Draw.Color = graph.graphColor;
                        Draw.Line(edge.p1.worldPosition, edge.p2.worldPosition+Vector3.up *.0f);
                        Draw.Line(edge.p1.upperWorldPosition, edge.p2.upperWorldPosition);
                    }
                    foreach(VectorPoint point in graph.points)
                    {
                        if(point.isInside)
                            Draw.Color = graph.graphColor * .0f;
                        else
                            Draw.Color = graph.graphColor;
                        Draw.Line(point.worldPosition, point.upperWorldPosition);
                        Draw.Sphere(point.worldPosition, 0.1f);
                    }
                }

                foreach(RoofGraph graph in vectorBuilding.roofGraphs)
                {
                    Draw.Color = graph.graphColor;
                    foreach(VectorEdge edge in graph.edges)
                    {
                        Draw.Line(edge.p1.worldPosition, edge.p2.worldPosition);
                    }
                }
            }         
        } 
    }

}

