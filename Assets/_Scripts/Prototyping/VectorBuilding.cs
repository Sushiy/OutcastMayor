using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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


        [Header("Rectangle Tool")]
        ///State of the rectangle building. 0 = No point placed, 1= start point placed, 2 = first line placed
        public int rectState = 0;
        public Shapes.Rectangle currentRectangle;
        public Transform currentRectangleParent;
        public VectorPoint rectP1,rectP2,rectP3,rectP4;


        [Header("Graph")]
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
            if(buildMode == BuildMode.Line)
            {
                if(lastLinePoint != null)
                {
                    currentLine.End = currentLine.transform.InverseTransformPoint(mousePosition);
                }
            }            
            else if(buildMode == BuildMode.Rectangle)
            {
                if(rectState == 0)
                {                    
                    currentRectangle.Width = .1f;
                    currentRectangle.Height = .1f;
                    currentRectangleParent.transform.LookAt(rectP2.worldPosition, Vector3.up);
                }
                else if(rectState == 1)
                {
                    currentRectangleParent.transform.LookAt(mousePosition, Vector3.up);
                    currentRectangle.Width = Vector3.Distance(rectP1.worldPosition, mousePosition);
                    currentRectangle.Height = 0.1f;
                }
                else if(rectState == 2)
                {
                    currentRectangle.Height = Vector3.Distance(rectP2.worldPosition, mousePosition);
                }
            }
        }

        /// <summary>
        /// Place a continuous stream of linepoints
        /// </summary>
        [Button]
        public void ChangeToLineMode()
        {
            buildMode = BuildMode.Line;
        }

        /// <summary>
        /// Place a rectangle with 4 points
        /// </summary>
        [Button]
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

            #region LineBuilding
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

                    VectorPoint newPoint = new VectorPoint(clickPosition, lastLinePoint, currentVectorPointGraph);
                    if(lastLinePoint != null)
                        lastLinePoint.AddPoint(newPoint);
                    currentVectorPointGraph.AddPoint(newPoint);
                    lastLinePoint = newPoint;
                    

                    //Also add a controlPoint
                    AddControlPoint(newPoint);
                }
                currentLine.Start = currentLine.transform.InverseTransformPoint(lastLinePoint.worldPosition);
            }
            #endregion
            if(buildMode == BuildMode.Rectangle)
            {
                if(clickedPoint != null)
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPoint.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }

                    if(rectState == 0)
                    {
                        rectP1 = clickedPoint.vectorPoint;
                        currentRectangleParent.position = clickedPoint.vectorPoint.worldPosition;
                        rectState = 1;
                    }
                    else if(rectState == 1)
                    {
                        rectP2 = clickedPoint.vectorPoint;
                        rectP1.AddPoint(rectP2);
                        currentRectangleParent.transform.LookAt(rectP2.worldPosition, Vector3.up);
                        currentRectangle.Width = Vector3.Distance(rectP1.worldPosition, rectP2.worldPosition);
                        rectState = 2;
                    }
                    else if(rectState == 2)
                    {
                        rectP3 = new VectorPoint(clickPosition, rectP2, currentVectorPointGraph);
                        rectP2.AddPoint(rectP3);

                        rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), rectP3, currentVectorPointGraph);
                        rectP1.AddPoint(rectP4);

                        currentRectangle.Width = Vector3.Distance(rectP2.worldPosition, rectP3.worldPosition);

                        //Add finished points
                        
                        currentVectorPointGraph.AddPoint(rectP1);
                        currentVectorPointGraph.AddPoint(rectP2);
                        currentVectorPointGraph.AddPoint(rectP3);
                        currentVectorPointGraph.AddPoint(rectP4);
                        AddControlPoint(rectP1);
                        AddControlPoint(rectP2);
                        AddControlPoint(rectP3);
                        AddControlPoint(rectP4);
                        rectState = 0;
                        currentVectorPointGraph = null;
                    }
                }
                else
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = new VectorPointGraph();
                        vectorPointGraphs.Add(currentVectorPointGraph);
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }
                    if(rectState == 0)
                    {
                        rectP1 = new VectorPoint(clickPosition, null, currentVectorPointGraph);
                        currentRectangleParent.position = clickPosition;
                        rectState = 1;
                    }
                    else if(rectState == 1)
                    {
                        rectP2 = new VectorPoint(clickPosition, rectP1, currentVectorPointGraph);
                        rectP1.AddPoint(rectP2);
                        currentRectangleParent.transform.LookAt(rectP2.worldPosition, Vector3.up);
                        currentRectangle.Width = Vector3.Distance(rectP1.worldPosition, rectP2.worldPosition);
                        rectState = 2;
                    }
                    else if(rectState == 2)
                    {
                        rectP3 = new VectorPoint(clickPosition, rectP2, currentVectorPointGraph);
                        rectP2.AddPoint(rectP3);

                        rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), rectP3, currentVectorPointGraph);
                        rectP1.AddPoint(rectP4);

                        currentRectangle.Width = Vector3.Distance(rectP2.worldPosition, rectP3.worldPosition);

                        //Add finished points
                        
                        currentVectorPointGraph.AddPoint(rectP1);
                        currentVectorPointGraph.AddPoint(rectP2);
                        currentVectorPointGraph.AddPoint(rectP3);
                        currentVectorPointGraph.AddPoint(rectP4);
                        AddControlPoint(rectP1);
                        AddControlPoint(rectP2);
                        AddControlPoint(rectP3);
                        AddControlPoint(rectP4);
                        rectState = 0;
                        currentVectorPointGraph = null;
                    }
                }
            }
        }

        public void AddControlPoint(VectorPoint _point)
        {
            ControlPoint newControlPoint = Instantiate(controlPoint, transform);
            newControlPoint.transform.position = _point.worldPosition;
            newControlPoint.SetData(_point, this);
            controlPoints.Add(newControlPoint);
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
