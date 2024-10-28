using System.Collections;
using System.Collections.Generic;
using OutcastMayor.VectorBuilding;
using UnityEngine;

namespace OutcastMayor.VectorBuilding
{
    public class LineTool : VectorBuildingTool
    {
        public VectorPoint lastLinePoint = null;

        public Shapes.Line currentLine;
        public override void OnClick(Vector3 _clickPosition, ControlElement _clickedControl)
        {
            VectorPointGraph currentVectorPointGraph = vectorBuilding.currentVectorPointGraph;
             if(_clickedControl != null)
            {
                VectorPoint clickedPoint;
                if(_clickedControl is ControlPoint)
                {
                    clickedPoint = (_clickedControl as ControlPoint).vectorPoint;
                    if(vectorBuilding.currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPointGraph;
                        currentLine.Color = currentVectorPointGraph.GraphColor;
                        if(lastLinePoint == null)
                        {
                            lastLinePoint = clickedPoint;
                        }
                    }
                }
                else
                {
                    VectorEdge v = (_clickedControl as ControlEdge).vectorEdge;
                    
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = v.vectorPointGraph;
                        currentLine.Color = currentVectorPointGraph.GraphColor;
                    }

                    Vector3 edgeNormalized = v.Vector.normalized;
                    float distance = Vector3.Dot(edgeNormalized, _clickPosition - v.p1.worldPosition);
                    Vector3 worldPosition = v.p1.worldPosition + distance * edgeNormalized;
                    clickedPoint = new VectorPoint(worldPosition, currentVectorPointGraph);
                }
                if(lastLinePoint != null)
                {
                    if(currentVectorPointGraph.ContainsEdge(lastLinePoint, clickedPoint))
                    {
                        Debug.LogWarning("[VectorBuilding] These points are already connected");
                        lastLinePoint = clickedPoint;
                    }
                    else
                    {
                        VectorEdge e = new VectorEdge(lastLinePoint, clickedPoint);
                        currentVectorPointGraph.AddShape(new List<VectorPoint>{lastLinePoint, clickedPoint}, new List<VectorEdge>{e});
                        lastLinePoint = clickedPoint;
                        if(lastLinePoint != clickedPoint)   
                            currentVectorPointGraph.closed = true;
                    }                            
                }
                else
                {
                    //Just set last point to this point and do nothing else?
                    lastLinePoint = clickedPoint;
                }
            }
            else
            {
                if(currentVectorPointGraph == null)
                {
                    currentVectorPointGraph = new VectorPointGraph();
                    vectorPointGraphs.Add(currentVectorPointGraph);
                    currentLine.Color = currentVectorPointGraph.GraphColor;
                }

                VectorPoint newPoint = new VectorPoint(_clickPosition, currentVectorPointGraph);
                if(lastLinePoint == null)
                {
                    currentVectorPointGraph.AddPoint(newPoint);
                }
                else
                {
                    VectorEdge e = new VectorEdge(lastLinePoint, newPoint);
                    currentVectorPointGraph.AddShape(new List<VectorPoint>{lastLinePoint, newPoint}, new List<VectorEdge>{e});
                }
                lastLinePoint = newPoint;
                

                //Also add a controlPoint
                vectorBuilding.AddControlPoint(newPoint);                        
            }
            vectorBuilding.OnGraphUpdated();
            currentLine.Start = currentLine.transform.InverseTransformPoint(lastLinePoint.worldPosition);
        }
    }

}
