using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using JetBrains.Annotations;
using System;
using Yarn.Unity;

namespace OutcastMayor
{
    [ExecuteAlways]
    public class VectorBuilding : ImmediateModeShapeDrawer
    {
        public BasicPlayerInputManager inputManager;

        public ControlPoint controlPoint;

        public enum BuildMode
        {
            Line,
            Rectangle
        }

        public BuildMode buildMode = BuildMode.Line;

        public VectorPoint lastLinePoint = null;

        public VectorPointGraph currentVectorPointGraph;

        public List<VectorPointGraph> vectorPointGraphs = new List<VectorPointGraph>();        

        public List<ControlPoint> controlPoints;

        void Awake()
        {
            inputManager.onPrimaryPerformed += PrimaryClick;
            inputManager.on1Pressed += ChangeToLineMode;
            inputManager.on2Pressed += ChangeToRectangleMode;

            currentVectorPointGraph = new VectorPointGraph();
            vectorPointGraphs.Add(currentVectorPointGraph);
        }

        /// <summary>
        /// Place a continuous stream of linepoints
        /// </summary>
        public void ChangeToLineMode()
        {
            buildMode = BuildMode.Line;
        }

        /// <summary>
        /// Place a rectangle with 4 points
        /// </summary>
        public void ChangeToRectangleMode()
        {
            buildMode = BuildMode.Rectangle;
        }

        public void PrimaryClick()
        {
            Vector3 clickPosition = inputManager.HitInfo.point;

            ControlPoint clickedPoint = inputManager.HitInfo.collider.GetComponent<ControlPoint>();

            //Just place another point and add it to the last placed linepoint
            if(buildMode == BuildMode.Line)
            {
                if(clickedPoint != null)
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPoint.vectorPointGraph;
                        if(lastLinePoint == null)
                        {
                            lastLinePoint = clickedPoint.vectorPoint;
                        }
                    }
                    else
                    {
                        if(lastLinePoint != null)
                        {
                            lastLinePoint.AddPoint(clickedPoint.vectorPoint);
                            clickedPoint.vectorPoint.AddPoint(lastLinePoint);
                            currentVectorPointGraph.AddPoint(clickedPoint.vectorPoint);
                            lastLinePoint = clickedPoint.vectorPoint;
                        }
                    }
                }
                else
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = new VectorPointGraph();
                        vectorPointGraphs.Add(currentVectorPointGraph);
                    }

                    VectorPoint v = new VectorPoint(clickPosition, lastLinePoint, currentVectorPointGraph);
                    lastLinePoint.AddPoint(v);
                    currentVectorPointGraph.AddPoint(v);
                    lastLinePoint = v;
                    

                    //Also add a controlPoint
                    ControlPoint newControlPoint = Instantiate(controlPoint, transform);
                    newControlPoint.transform.position = clickPosition;
                    newControlPoint.SetData(v, this);
                    AddControlPoint(newControlPoint);

                }
            }
        }

        public void AddControlPoint(ControlPoint _point)
        {
            controlPoints.Add(_point);
        } 

        HashSet<VectorPoint> alreadyDrawnPoints = new HashSet<VectorPoint>();
        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                // set up static parameters. these are used for all following Draw.Line calls
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Pixels;
                Draw.Thickness = 4; // 4px wide

                foreach(VectorPointGraph graph in vectorPointGraphs)
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

    [System.Serializable]
    public class VectorPoint
    {
        public Vector3 worldPosition;

        [HideInInspector]
        public List<VectorPoint> connectedPoints;

        public int connectedPointCount = 0;

        [HideInInspector]
        public VectorPointGraph vectorPointGraph;

        public VectorPoint(Vector3 _worldPosition, VectorPoint _previousPoint, VectorPointGraph _vectorPointGraph)
        {
            worldPosition = _worldPosition;
            connectedPoints = new List<VectorPoint>();
            vectorPointGraph = _vectorPointGraph;
            AddPoint(_previousPoint);
        }

        public void AddPoint(VectorPoint _vectorPoint)
        {
            if(_vectorPoint != null && !connectedPoints.Contains(_vectorPoint))
            {
                connectedPoints.Add(_vectorPoint);
                connectedPointCount++;
            }
        }
    }

    [System.Serializable]
    public class VectorPointGraph
    {
        [HideInInspector]
        public List<VectorPoint> includedPoints;

        public Color graphColor;

        public VectorPointGraph()
        {
            includedPoints = new List<VectorPoint>();
            graphColor = UnityEngine.Random.ColorHSV(0f,1f, 0.5f,1f,.5f,1f);
        }

        public void AddPoint(VectorPoint _point)
        {
            if(!includedPoints.Contains(_point))
                includedPoints.Add(_point);
            else
                Debug.LogError("This graph already contains this point");
        }
    }

}
