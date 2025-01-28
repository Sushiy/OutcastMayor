using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OutcastMayor
{
    public class VectorBuilding : MonoBehaviour
    {
        public BasicPlayerInputManager inputManager;

        public enum BuildMode
        {
            Line = 0,
            Rectangle = 1,
            Roof = 2,
            Move = 3
        }

        public BuildMode buildMode = BuildMode.Line;
        public ControlPoint controlPointPrefab;
        public ControlEdge controlEdgePrefab;

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
        public List<ControlEdge> controlEdges;

        public List<RoofGraph> roofGraphs = new List<RoofGraph>();
        
        public RoofGraph currentRoofGraph;

        public Action<VectorPointGraph> onUpdate;
        public Action<BuildMode> onChangeTool;

        void  Awake()
        {
            inputManager.onPrimaryPerformed += PrimaryClick;
            inputManager.onSecondaryPerformed += SecondaryClick;
            inputManager.on1Pressed += ChangeToLineMode;
            inputManager.on2Pressed += ChangeToRectangleMode;
            inputManager.on3Pressed += ChangeToRoofMode;

            currentVectorPointGraph = new VectorPointGraph();
            currentLine.Color = currentVectorPointGraph.graphColor;
            vectorPointGraphs.Add(currentVectorPointGraph);

            
            currentRoofGraph = new RoofGraph();
            currentLine.Color = currentRoofGraph.graphColor;
            roofGraphs.Add(currentRoofGraph);
            p = new VectorPoint(Vector3.zero, currentVectorPointGraph);
        }
        VectorPoint p;        
        void Update()
        {
            Vector3 mousePosition = inputManager.HitInfo.point;

            p.worldPosition = mousePosition;
            /*
            if(currentVectorPointGraph != null && currentVectorPointGraph.CheckInside(p, currentVectorPointGraph.edges))
            {
                Debug.DrawRay(p.worldPosition, Vector3.right * 1000.0f, Color.green);
            }
            else
            {
                Debug.DrawRay(p.worldPosition, Vector3.right * 1000.0f, Color.red);
            }*/

            if(buildMode == BuildMode.Line)
            {
                if(lastLinePoint != null)
                {
                    currentLine.End = currentLine.transform.InverseTransformPoint(mousePosition);
                }
            }            
            else if(buildMode == BuildMode.Rectangle || buildMode == BuildMode.Roof)
            {
                if(rectState == 0)
                {                    
                    currentRectangle.Width = .1f;
                    currentRectangle.Height = .1f;
                }
                else if(rectState == 1)
                {
                    Vector3 toMouse = mousePosition - rectP1.worldPosition;
                    currentRectangleParent.LookAt(mousePosition, Vector3.up);
                    currentRectangle.Width = toMouse.magnitude;
                    currentRectangle.Height = 0.1f;
                }
                else if(rectState == 2)
                {
                    
                    float height =  Vector3.Dot(mousePosition - rectP2.worldPosition, currentRectangleParent.right);
                    bool isOpposite = height > 0;
                    currentRectangle.Height = Mathf.Abs(height);
                    if(isOpposite)
                        currentRectangle.transform.localScale = new Vector3(1,-1,1);
                    else
                        currentRectangle.transform.localScale = new Vector3(1,1,1);

                }
            }
        }

        /// <summary>
        /// Place a continuous stream of linepoints
        /// </summary>
        [Button]
        public void ChangeToLineMode()
        {
            ChangeTool(buildMode = BuildMode.Line);
        }

        /// <summary>
        /// Place a rectangle with 4 points
        /// </summary>
        [Button]
        public void ChangeToRectangleMode()
        {
            ChangeTool(BuildMode.Rectangle);
        }
        /// <summary>
        /// Place a rectangle with 4 points
        /// </summary>
        [Button]
        public void ChangeToRoofMode()
        {
            ChangeTool(BuildMode.Roof);
        }

        void ChangeTool(BuildMode _buildMode)
        {
            buildMode = _buildMode;
            onChangeTool?.Invoke(buildMode);
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
            ControlElement clickedControl = null;
            if(inputManager.HitInfo.collider != null)
                clickedControl = inputManager.HitInfo.collider.GetComponent<ControlElement>();

            if(buildMode == BuildMode.Line)
            {
                ClickLineMode(clickPosition,clickedControl);
            }
            if(buildMode == BuildMode.Rectangle)
            {
                ClickRectangleMode(clickPosition, clickedControl);
            }
            if(buildMode == BuildMode.Roof)
            {
                ClickRoofMode(clickPosition, clickedControl);
            }
        }

        public void ClickLineMode(Vector3 _clickPosition, ControlElement _clickedControl)
        {
            if(_clickedControl != null)
            {
                VectorPoint clickedPoint;
                if(_clickedControl is ControlPoint)
                {
                    clickedPoint = (_clickedControl as ControlPoint).vectorPoint;
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
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
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
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
                    currentLine.Color = currentVectorPointGraph.graphColor;
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
                AddControlPoint(newPoint);                        
            }
            onUpdate?.Invoke(currentVectorPointGraph);
            currentLine.Start = currentLine.transform.InverseTransformPoint(lastLinePoint.worldPosition);
        }

        public void ClickRectangleMode(Vector3 _clickPosition, ControlElement _clickedControl)
        {
            if(_clickedControl != null)
            {
                
                VectorPoint clickedPoint;
                if(_clickedControl is ControlPoint)
                {
                    clickedPoint = (_clickedControl as ControlPoint).vectorPoint;
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }
                }
                else
                {
                    VectorEdge v = (_clickedControl as ControlEdge).vectorEdge;
                    
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = v.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }

                    Vector3 edgeNormalized = v.Vector.normalized;
                    float distance = Vector3.Dot(edgeNormalized, _clickPosition - v.p1.worldPosition);
                    Vector3 worldPosition = v.p1.worldPosition + distance * edgeNormalized;
                    clickedPoint = new VectorPoint(worldPosition, currentVectorPointGraph);
                }
                
                if(rectState == 0)
                {
                    rectP1 = clickedPoint;
                    currentRectangleParent.position = clickedPoint.worldPosition;
                    rectState = 1;
                    Debug.Log("[RectangleTool] Step1: Clicked on P1");
                }
                else if(rectState == 1)
                {
                    rectP2 = clickedPoint;
                    currentRectangleParent.transform.LookAt(rectP2.worldPosition, Vector3.up);
                    currentRectangle.Width = Vector3.Distance(rectP1.worldPosition, rectP2.worldPosition);
                    rectState = 2;
                    Debug.Log("[RectangleTool] Step2: Clicked on P2");
                }
                else if(rectState == 2)
                {
                    Vector3 toClick = clickedPoint.worldPosition - rectP2.worldPosition;
                    float height = Vector3.Dot(toClick, currentRectangleParent.right);
                    
                    Vector3 rectP3Pos = rectP2.worldPosition + height * currentRectangleParent.right;
                    Vector3 rectP4Pos = rectP3Pos + (rectP1.worldPosition - rectP2.worldPosition);

                    //Check if p3 is the clicked point
                    float angleToP3 = Vector3.Dot(toClick.normalized, currentRectangleParent.right);
                    Debug.Log("[RectangleTool] Step3: Angle to P3 " + angleToP3);                        
                    float angleToP4 = Vector3.Dot(toClick.normalized, (currentRectangleParent.right+currentRectangleParent.forward).normalized);
                    Debug.Log("[RectangleTool] Step3: Angle to P4 " + angleToP4);
                    if(Mathf.Approximately(Mathf.Abs(angleToP3), 1))
                    {
                        //you actually hit the point with the corner :o
                        rectP3 = clickedPoint;
                        rectP4 = new VectorPoint(rectP4Pos, currentVectorPointGraph);
                        Debug.Log("[RectangleTool] Step3: Clicked on P3");
                    }
                    else
                    {
                        //Check if p4 is the clicked point
                        if(Mathf.Approximately(Mathf.Abs(angleToP4), 1))
                        {
                            rectP3 = new VectorPoint(rectP3Pos, currentVectorPointGraph);
                            rectP4 = clickedPoint;
                            Debug.Log("[RectangleTool] Step3: Clicked on P4");
                        }
                        else
                        {
                            rectP3 = new VectorPoint(rectP3Pos, currentVectorPointGraph);                                
                            rectP4 = new VectorPoint(rectP4Pos, currentVectorPointGraph);
                            Debug.Log("[RectangleTool] Step3: Clicked Wrong Point");
                        }

                    }
                    
                    bool isOpposite = height > 0;
                    currentRectangle.Height = height;
                    if(isOpposite)
                        currentRectangle.transform.localScale = new Vector3(1,-1,1);
                    else
                        currentRectangle.transform.localScale = new Vector3(1,1,1);

                    //Add finished points
                    VectorEdge e1 = new VectorEdge(rectP1, rectP2);
                    VectorEdge e2 = new VectorEdge(rectP2, rectP3);
                    VectorEdge e3 = new VectorEdge(rectP3, rectP4);
                    VectorEdge e4 = new VectorEdge(rectP4, rectP1);

                    currentVectorPointGraph.AddShape(new List<VectorPoint>{rectP1, rectP2, rectP3, rectP4}, new List<VectorEdge>{e1,e2,e3,e4});
                    
                    currentVectorPointGraph.closed = true;
                    rectState = 0;
                    //currentVectorPointGraph = null;
                    
                    onUpdate?.Invoke(currentVectorPointGraph);
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
                    rectP1 = new VectorPoint(_clickPosition, currentVectorPointGraph);
                    currentRectangleParent.position = _clickPosition;
                    rectState = 1;
                    Debug.Log("[RectangleTool] Step1: Clicked No Point");
                }
                else if(rectState == 1)
                {
                    rectP2 = new VectorPoint(_clickPosition, currentVectorPointGraph);
                    
                    Vector3 dir = rectP2.worldPosition - rectP1.worldPosition;
                    currentRectangleParent.LookAt(rectP2.worldPosition, Vector3.up);
                    currentRectangle.Width = dir.magnitude * Vector3.Dot(dir.normalized, currentRectangleParent.forward);
                    rectState = 2;
                    Debug.Log("[RectangleTool] Step2: Clicked No Point");
                }
                else if(rectState == 2)
                {
                    float height = Vector3.Dot(_clickPosition - rectP2.worldPosition, currentRectangleParent.right);
                    Vector3 p3 = rectP2.worldPosition + height * currentRectangleParent.right;
                    rectP3 = new VectorPoint(p3, currentVectorPointGraph);

                    rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), currentVectorPointGraph);
                    
                    Debug.Log("[RectangleTool] Step3: Clicked No Point");
                    bool isOpposite = height > 0;
                    currentRectangle.Height = height;
                    if(isOpposite)
                        currentRectangle.transform.localScale = new Vector3(1,-1,1);
                    else
                        currentRectangle.transform.localScale = new Vector3(1,1,1);

                    //Add finished points
                    VectorEdge e1 = new VectorEdge(rectP1, rectP2);
                    VectorEdge e2 = new VectorEdge(rectP2, rectP3);
                    VectorEdge e3 = new VectorEdge(rectP3, rectP4);
                    VectorEdge e4 = new VectorEdge(rectP4, rectP1);

                    currentVectorPointGraph.AddShape(new List<VectorPoint>{rectP1, rectP2, rectP3, rectP4}, new List<VectorEdge>{e1,e2,e3,e4});
                    currentVectorPointGraph.closed = true;
                    rectState = 0;
                    //currentVectorPointGraph = null;
                    onUpdate?.Invoke(currentVectorPointGraph);
                }
            }
        }

        public void ClickRoofMode(Vector3 _clickPosition, ControlElement _clickedControl)
        {
            if(_clickedControl != null)
            {
                VectorPoint clickedPoint;
                if(_clickedControl is ControlPoint)
                {
                    clickedPoint = (_clickedControl as ControlPoint).vectorPoint;
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = clickedPoint.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }
                }
                else
                {
                    VectorEdge v = (_clickedControl as ControlEdge).vectorEdge;
                    
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = v.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }

                    Vector3 edgeNormalized = v.Vector.normalized;
                    float distance = Vector3.Dot(edgeNormalized, _clickPosition - v.p1.worldPosition);
                    Vector3 worldPosition = v.p1.worldPosition + distance * edgeNormalized;
                    clickedPoint = new VectorPoint(worldPosition, currentVectorPointGraph);
                }
            }
            else
            {
                if(currentRoofGraph == null)
                {
                    currentRoofGraph = new RoofGraph();
                    roofGraphs.Add(currentRoofGraph);
                    currentRectangle.Color = currentRoofGraph.graphColor;
                }
                if(rectState == 0)
                {
                    rectP1 = new VectorPoint(_clickPosition, currentRoofGraph);
                    currentRectangleParent.position = _clickPosition;
                    rectState = 1;
                }
                else if(rectState == 1)
                {
                    rectP2 = new VectorPoint(_clickPosition, currentRoofGraph);
                    
                    Vector3 dir = rectP2.worldPosition - rectP1.worldPosition;
                    currentRectangleParent.LookAt(rectP2.worldPosition, Vector3.up);
                    currentRectangle.Width = dir.magnitude * Vector3.Dot(dir.normalized, currentRectangleParent.forward);
                    rectState = 2;
                }
                else if(rectState == 2)
                {
                    float height = Vector3.Dot(_clickPosition - rectP2.worldPosition, currentRectangleParent.right);
                    Vector3 p3 = rectP2.worldPosition + height * currentRectangleParent.right;
                    rectP3 = new VectorPoint(p3, currentRoofGraph);

                    rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), currentRoofGraph);
                    
                    bool isOpposite = height > 0;
                    currentRectangle.Height = height;
                    if(isOpposite)
                        currentRectangle.transform.localScale = new Vector3(1,-1,1);
                    else
                        currentRectangle.transform.localScale = new Vector3(1,1,1);

                    //Add finished points
                    currentRoofGraph.AddEdge(rectP1, rectP2);
                    currentRoofGraph.AddEdge(rectP2, rectP3);
                    currentRoofGraph.AddEdge(rectP3, rectP4);
                    currentRoofGraph.AddEdge(rectP1, rectP4);
                    
                    currentRoofGraph.AddPoint(rectP1);
                    currentRoofGraph.AddPoint(rectP2);
                    currentRoofGraph.AddPoint(rectP3);
                    currentRoofGraph.AddPoint(rectP4);
                    currentRoofGraph.GenerateRoofFromRect(currentRectangle.Width, Mathf.Abs(currentRectangle.Height));
                    currentRoofGraph.closed = true;
                    rectState = 0;
                    onUpdate?.Invoke(currentRoofGraph);
                    //currentRoofGraph = null;
                }
            }
        }

        public void AddControlPoint(VectorPoint _point)
        {
            ControlPoint newControlPoint = Instantiate(controlPointPrefab, transform);
            newControlPoint.transform.position = _point.worldPosition;
            newControlPoint.SetData(_point, this);
            ControlPoint newUpperControlPoint = Instantiate(controlPointPrefab, transform);
            newUpperControlPoint.transform.position = _point.upperWorldPosition;
            newUpperControlPoint.SetData(_point, this);
            controlPoints.Add(newControlPoint);
            controlPoints.Add(newUpperControlPoint);
        } 
    }
}
