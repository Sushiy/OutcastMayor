using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    public class VectorBuilding : MonoBehaviour
    {
        public BasicPlayerInputManager inputManager;

        public ControlPoint controlPoint;

        public enum BuildMode
        {
            Line,
            Rectangle
        }

        public BuildMode buildMode = BuildMode.Line;

        [Header("Line Tool")]
        public VectorPoint lastLinePoint = null;

        public Shapes.Line currentLine;

        public VectorPointGraph currentVectorPointGraph;

        public List<VectorPointGraph> vectorPointGraphs = new List<VectorPointGraph>();        

        public List<ControlPoint> controlPoints;

        void  Awake()
        {
            inputManager.onPrimaryPerformed += PrimaryClick;
            inputManager.onSecondaryPerformed += SecondaryClick;
            inputManager.on1Pressed += ChangeToLineMode;
            inputManager.on2Pressed += ChangeToRectangleMode;

            currentVectorPointGraph = new VectorPointGraph();
            currentLine.Color = currentVectorPointGraph.graphColor;
            vectorPointGraphs.Add(currentVectorPointGraph);
        }

        void Update()
        {
            Vector3 mousePosition = inputManager.HitInfo.point;
            if(lastLinePoint != null)
            {
                currentLine.End = currentLine.transform.InverseTransformPoint(mousePosition);
            }

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

        public void SecondaryClick()
        {
            if(currentVectorPointGraph != null)
            {
                currentVectorPointGraph = null;
                lastLinePoint = null;
            }
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
                        currentLine.Color = currentVectorPointGraph.graphColor;
                        if(lastLinePoint == null)
                        {
                            lastLinePoint = clickedPoint.vectorPoint;
                        }
                    }
                    else
                    {
                        if(lastLinePoint != null)
                        {
                            if(lastLinePoint.connectedPoints.Contains(clickedPoint.vectorPoint))
                            {
                                Debug.LogWarning("[VectorBuilding] These points are already connected");
                                lastLinePoint = clickedPoint.vectorPoint;
                            }
                            else
                            {
                                lastLinePoint.AddPoint(clickedPoint.vectorPoint);
                                clickedPoint.vectorPoint.AddPoint(lastLinePoint);
                                currentVectorPointGraph.AddPoint(clickedPoint.vectorPoint);
                                lastLinePoint = clickedPoint.vectorPoint;  
                            }                            
                        }
                    }
                }
                else
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = new VectorPointGraph();
                        vectorPointGraphs.Add(currentVectorPointGraph);
                        currentLine.Color = currentVectorPointGraph.graphColor;
                    }

                    
                    Debug.Log("[VectorBuilding] Add Point normally");

                    VectorPoint newPoint = new VectorPoint(clickPosition, lastLinePoint, currentVectorPointGraph);
                    if(lastLinePoint != null)
                        lastLinePoint.AddPoint(newPoint);
                    currentVectorPointGraph.AddPoint(newPoint);
                    lastLinePoint = newPoint;
                    

                    //Also add a controlPoint
                    ControlPoint newControlPoint = Instantiate(controlPoint, transform);
                    newControlPoint.transform.position = clickPosition;
                    newControlPoint.SetData(newPoint, this);
                    AddControlPoint(newControlPoint);
                }
                
                currentLine.Start = currentLine.transform.InverseTransformPoint(lastLinePoint.worldPosition);
            }
        }

        public void AddControlPoint(ControlPoint _point)
        {
            controlPoints.Add(_point);
        } 
    }

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
                Debug.LogWarning("[VectorBuilding->VectorPointGraph] This graph already contains this point");
        }
    }

}
