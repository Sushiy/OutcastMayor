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
        /// <summary>
        /// Place a rectangle with 4 points
        /// </summary>
        [Button]
        public void ChangeToRoofMode()
        {
            buildMode = BuildMode.Roof;
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

            ControlElement clickedControl = inputManager.HitInfo.collider.GetComponent<ControlElement>();

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

    public class VectorPoint
    {
        public Vector3 worldPosition;
        public bool isInside = false;

        public Vector3 upperWorldPosition;

        public int connectedPointCount = 0;

        [HideInInspector]
        public VectorPointGraph vectorPointGraph;

        public VectorPoint(Vector3 _worldPosition, VectorPointGraph _vectorPointGraph)
        {
            worldPosition = _worldPosition;
            upperWorldPosition = _worldPosition + Vector3.up * 2f;
            vectorPointGraph = _vectorPointGraph;
        }
    }

    public class VectorEdge : IEquatable<VectorEdge>
    {
        public VectorPoint p1, p2;
        public bool isInside = false;
        [HideInInspector]
        public VectorPointGraph vectorPointGraph;
        public Vector3 Vector
        {
            get
            {
                return p2.worldPosition - p1.worldPosition;
            }
        }
        public Vector3 Center
        {
            get
            {
                return (p1.worldPosition + p2.worldPosition)/2.0f;
            }
        }
        public float Length
        {
            get
            {
                return Vector3.Distance(p1.worldPosition, p2.worldPosition);
            }
        }

        public VectorEdge(VectorPoint _p1, VectorPoint _p2)
        {
            p1 = _p1;
            p2 = _p2;
        }

        public bool Equals(VectorEdge _other)
        {
            return this == _other || (p1 == _other.p1 && p2 == _other.p2) || (p2 == _other.p1 && p1 == _other.p2);
        }
    }

    public class VectorPointGraph
    {
        public List<VectorPoint> points;
        public List<VectorEdge> edges;

        public List<VectorEdge> deletedInsideEdges;

        public Color graphColor;

        public bool closed = false;

        public VectorPointGraph()
        {
            points = new List<VectorPoint>();
            edges =  new List<VectorEdge>();
            deletedInsideEdges = new List<VectorEdge>();
            graphColor = UnityEngine.Random.ColorHSV(0f,1f, 0.5f,1f,.5f,1f);
        }

        public void AddPoint(VectorPoint _point)
        {
            if(!points.Contains(_point))
                points.Add(_point);
            else
                Debug.LogWarning("[VectorBuilding->VectorPointGraph] This graph already contains this point");
        }
        public void AddPoints(List<VectorPoint> _points)
        {
            foreach(VectorPoint p in _points)
            {
                AddPoint(p);
            }
        }
        public void AddEdge(VectorEdge _edge)
        {
            if(!edges.Contains(_edge))
            {
                
                edges.Add(_edge);
            }
            else
                Debug.LogWarning("[VectorBuilding->VectorPointGraph] This graph already contains this edge");
        }

        public void AddEdge(VectorPoint _p1, VectorPoint _p2)
        {
            AddEdge(new VectorEdge(_p1, _p2));
        }

        public void AddEdges(List<VectorEdge> _edges)
        {
            foreach(VectorEdge e in _edges)
            {
                AddEdge(e);
            }
        }
        /// <summary>
        /// Add a shape (Points and edges) to the graph. Check if any of the points and edges are inside and mark them.
        /// </summary>
        /// <param name="_incomingPoints"></param>
        /// <param name="_incomingEdges"></param>
        public void AddShape(List<VectorPoint> _incomingPoints, List<VectorEdge> _incomingEdges)
        {
            List<VectorPoint> intersectionPoints = new List<VectorPoint>();
            List<VectorEdge> edgeSegments = new List<VectorEdge>();
            List<VectorEdge> bisectedEdges = new List<VectorEdge>();
            
            List<VectorEdge> testEdges = new List<VectorEdge>(edges);
            List<VectorEdge> originalIncomingEdges = new List<VectorEdge>(_incomingEdges);
            List<VectorEdge> incomingEdgeSegments = new List<VectorEdge>();

            List<VectorPoint> insidePoints = new List<VectorPoint>();
            List<VectorEdge> insideEdges = new List<VectorEdge>();

            List<VectorEdge> duplicateEdges = new List<VectorEdge>();
            //Remove any duplicate points and edges
            
            /*
            for(int i = _incomingPoints.Count-1; i >= 0; i--)
            {
                if(points.Contains(_incomingPoints[i]))
                {
                    _incomingPoints.Remove(_incomingPoints[i]);
                }
            }
            */
            bool hasDuplicateEdge = false;
            for(int i = _incomingEdges.Count-1; i >= 0; i--)
            {
                VectorEdge duplicate = _incomingEdges[i];
                if(edges.Contains(duplicate))
                {
                    VectorEdge original = edges.Find(x => x.Equals(duplicate));
                    duplicateEdges.Add(original);
                    insideEdges.Add(original);
                    _incomingEdges.Remove(duplicate);
                    hasDuplicateEdge = true;
                }
            }
            Debug.Log("[AddShape] has duplicate Edge:" + hasDuplicateEdge);

            //Find and add intersections
            Debug.Log("[AddShape] Original Edges:" + edges.Count + " Original Points: " + points.Count);
            Debug.Log("[AddShape] Incoming Edges:" + _incomingEdges.Count + " Incoming Points:" + _incomingPoints.Count);
            bool hasOverlap = false;
            foreach(VectorEdge incomingEdge in _incomingEdges)
            {
                edgeSegments.Clear();
                if(GetEdgeIntersection(incomingEdge, testEdges, ref intersectionPoints, ref edgeSegments, ref bisectedEdges, ref incomingEdgeSegments))
                {
                    if(intersectionPoints.Count > 0)
                        bisectedEdges.Add(incomingEdge);
                    hasOverlap = true;
                }
                foreach(VectorEdge e in edgeSegments)
                {
                    if(originalIncomingEdges.Contains(e))
                    {                 
                        Debug.Log("[AddShape] Add Duplicate Edge from segments");       
                        VectorEdge original = originalIncomingEdges.Find(x => x.Equals(e));
                        duplicateEdges.Add(original);
                        insideEdges.Add(original);
                    }
                    else if(!testEdges.Contains(e))
                    {                        
                        Debug.Log("[AddShape] Add normal Edge from segments");   
                        testEdges.Add(e); 
                    }
                }
                foreach(VectorEdge b in bisectedEdges)
                {
                    if(testEdges.Contains(b))
                        testEdges.Remove(b);
                }
            }
            Debug.Log("[AddShape] has overlap:" + hasOverlap);

            //If there is an overlap, get the intersections
            if(hasOverlap)
            {
                //update incoming edges and points
                _incomingPoints.AddRange(intersectionPoints);
                foreach(VectorEdge edge in bisectedEdges)
                {
                    if(_incomingEdges.Contains(edge))
                        _incomingEdges.Remove(edge);
                }
                foreach(VectorEdge edge in incomingEdgeSegments)
                {
                    if(!_incomingEdges.Contains(edge))
                        _incomingEdges.Add(edge);
                }
                foreach(VectorPoint v in intersectionPoints)
                    Debug.DrawRay(v.worldPosition, Vector3.down, Color.magenta, 5.0f);            
                Debug.Log("[AddShape] IntersectionPoints:" + intersectionPoints.Count + " Bisected Edges: " + bisectedEdges.Count);
                Debug.Log("[AddShape] Updated Edges:" + testEdges.Count);
                Debug.Log("[AddShape] Updated Incoming Edges:" + _incomingEdges.Count + " Updated Incoming Points:" + _incomingPoints.Count);

                //Check each incoming point and edge against the current edges.
                foreach(VectorPoint point in _incomingPoints)
                {
                    if(CheckInside(point, edges))
                    {
                        insidePoints.Add(point);
                    }
                }
                foreach(VectorEdge edge in _incomingEdges)
                {
                    if(bisectedEdges.Contains(edge))
                        continue;
                    if(CheckInside(edge, edges))
                    {
                        insideEdges.Add(edge);
                    }
                }

                //Check each original point and edge against the incoming edges
                foreach(VectorPoint point in points)
                {
                    if(!point.isInside && !insidePoints.Contains(point) && CheckInside(point, originalIncomingEdges))
                    {
                        insidePoints.Add(point);
                    }
                }
                foreach(VectorEdge edge in testEdges)
                {
                    if(!edge.isInside && !insideEdges.Contains(edge) && CheckInside(edge, originalIncomingEdges))
                    {
                        insideEdges.Add(edge);
                    }
                }
            }
            
            //Set the inside value of the edges and points
            foreach(VectorPoint point in insidePoints)
            {
                point.isInside = true;
            }

            foreach(VectorEdge edge in insideEdges)
            {
                edge.isInside = true;
            }
            edges = testEdges;
            AddEdges(_incomingEdges);
            foreach(VectorEdge edge in bisectedEdges)
            {
                if(edges.Contains(edge))
                    edges.Remove(edge);
            }
            foreach(VectorEdge edge in duplicateEdges)
            {
                List<VectorEdge> withoutDuplicates = new List<VectorEdge> (edges.Except(duplicateEdges));
                if(CheckInside(edge, withoutDuplicates))
                    edge.isInside = true;
            }
            AddPoints(_incomingPoints);
            Debug.Log("[AddShape] Resulting Edges:" + edges.Count + " Resulting Points:" + points.Count);
        }

        public bool ContainsEdge(VectorPoint _p1, VectorPoint _p2)
        {
            return ContainsEdge(new VectorEdge(_p1, _p2));
        }

        public bool ContainsEdge(VectorEdge _edge)
        {
            return edges.Contains(_edge);
        }

        public bool GetEdgeIntersection(VectorEdge _incomingEdge, List<VectorEdge> _edges, ref List<VectorPoint> _intersectionPoints, ref List<VectorEdge> _newEdges, ref List<VectorEdge> _bisectedEdges, ref List<VectorEdge> _ownEdgeSegments)
        {
            bool intersects = false;
            List<VectorEdge> ownEdgeSegments = new List<VectorEdge>();
            List<VectorPoint> intersectionPointsSorted = new List<VectorPoint>(); //Sorted by distance to startPoint.
            intersectionPointsSorted.Add(_incomingEdge.p1);
            foreach(VectorEdge edge in _edges)
            {
                VectorPoint intersectionPoint;
                List<VectorPoint> containedEdgePoints = new List<VectorPoint>();
                if(EdgeContainedInEdge(_incomingEdge, edge, out containedEdgePoints, ref _newEdges, ref _bisectedEdges))
                {
                    intersects= true;
                    foreach(VectorPoint p in containedEdgePoints)                    
                        intersectionPointsSorted.Add(p);
                }
                else if(LineIntersection(_incomingEdge, edge, out intersectionPoint, true))
                {
                    Debug.Log("[VectorBuilding -> GetEdgeIntersection] Standard Edge Intersection");
                    intersects= true;
                    if(!(_incomingEdge.p1 == intersectionPoint ||_incomingEdge.p2 == intersectionPoint))
                    {
                        _intersectionPoints.Add(intersectionPoint);
                    }
                    //split the edge that came in
                    _newEdges.Add(new VectorEdge( edge.p1, intersectionPoint));
                    _newEdges.Add(new VectorEdge(intersectionPoint, edge.p2));
                    //float sqrDist = (_incomingEdge.p1.worldPosition - intersectionPoint.worldPosition).sqrMagnitude;
                    //Debug.Log("[GetEdgeIntersection] + segment length" + sqrDist);
                    intersectionPointsSorted.Add(intersectionPoint);
                    if(!_bisectedEdges.Contains(edge))
                        _bisectedEdges.Add(edge);
                }
            }
            float l = (_incomingEdge.p1.worldPosition - _incomingEdge.p2.worldPosition).sqrMagnitude;
            intersectionPointsSorted.Add(_incomingEdge.p2);
            intersectionPointsSorted.Sort((x,y) => CompareSegmentLength(x,y, _incomingEdge.p1.worldPosition));

            for(int i = 0; i < intersectionPointsSorted.Count-1; i++)
            {
                ownEdgeSegments.Add(new VectorEdge(intersectionPointsSorted[i], intersectionPointsSorted[i+1]));
            }            
            _ownEdgeSegments.AddRange(ownEdgeSegments);       
            return intersects;
        }

        int CompareSegmentLength(VectorPoint _x, VectorPoint _y, Vector3 _worldPosition)
        {
            float lenX = (_x.worldPosition - _worldPosition).sqrMagnitude;
            float lenY = (_y.worldPosition - _worldPosition).sqrMagnitude;

            if(lenX == lenY) return 0;
            else if(lenX > lenY) return 1;
            else return -1;
        }

        public bool CheckInside(VectorPoint _point, List<VectorEdge> _testEdges)
        {
            if(_testEdges == null || _testEdges.Count == 0)
            {
                return false;
            }
            VectorEdge ray = new VectorEdge(_point, new VectorPoint(_point.worldPosition + Vector3.right * 1000f, this));

            int n = 0;
            foreach(VectorEdge edge in _testEdges)
            {
                if(edge.isInside)
                    continue;
                if(edge.p1 == _point || edge.p2 == _point)
                    continue;
                VectorPoint intersectPoint;
                if(LineIntersection(ray, edge, out intersectPoint))
                {
                    n++;
                }
            }
            
            if((n % 2) == 1)
            {
                //try again in a different direction
                ray = new VectorEdge(_point, new VectorPoint(_point.worldPosition + Vector3.forward * 1000f, this));

                n = 0;
                foreach(VectorEdge edge in _testEdges)
                {
                    if(edge.isInside)
                        continue;
                    if(edge.p1 == _point || edge.p2 == _point)
                        continue;
                    VectorPoint intersectPoint;
                    if(LineIntersection(ray, edge, out intersectPoint))
                    {
                        n++;
                    }
                }
            }

            

            return (n % 2) == 1;
        }
        public bool CheckInside(VectorEdge _edge, List<VectorEdge> _testEdges)
        {
            if(_testEdges == null || _testEdges.Count == 0)
            {
                return false;
            }
            VectorPoint point = new VectorPoint((_edge.p1.worldPosition + _edge.p2.worldPosition)/2.0f, this);
            VectorEdge ray = new VectorEdge(point, new VectorPoint(point.worldPosition + Vector3.right * 1000f, this));

            int n = 0;
            foreach(VectorEdge edge in _testEdges)
            {
                if(edge.isInside)
                    continue;
                if(edge == _edge)
                    continue;
                VectorPoint intersectPoint;
                if(LineIntersection(ray, edge, out intersectPoint))
                {
                    n++;
                }
            }

            return (n % 2) == 1;
        }
        bool LineSegmentContainsPoint(Vector3 lineP, Vector3 lineQ, Vector3 point)
        {
            float d2PPoint = (lineP - point).sqrMagnitude;
            float d2QPoint = (lineQ - point).sqrMagnitude;
            float d2PQ = (lineP - lineQ).sqrMagnitude;
            return d2PQ == d2PPoint + d2QPoint + 2*Mathf.Sqrt(d2PPoint * d2QPoint);
        }
        public bool LineIntersection(VectorEdge _e1, VectorEdge _e2, out VectorPoint _intersectionPoint, bool _debug = false)
        {
            Vector3 intersectionPosition = Vector3.zero;
            _intersectionPoint = new VectorPoint(Vector3.zero, null);
            
            //If these lines share a point, they do not intersect
            if(_e1.p1 == _e2.p1 || _e1.p1 == _e2.p2 || _e1.p2 == _e2.p1 || _e1.p2 == _e2.p2)
            {
                if(_debug)
                {
                    Debug.Log("[VectorBuilding -> LineIntersection] Lines that share a point never intersect");
                    Debug.DrawLine(_e1.Center, _e2.Center, Color.red, 5.0f);
                }
                return false;
            }
            Vector2 p1 = new Vector2(_e1.p1.worldPosition.x, _e1.p1.worldPosition.z);
            Vector2 p2 = new Vector2(_e1.p2.worldPosition.x, _e1.p2.worldPosition.z);
            Vector2 p3 = new Vector2(_e2.p1.worldPosition.x, _e2.p1.worldPosition.z);
            Vector2 p4 = new Vector2(_e2.p2.worldPosition.x, _e2.p2.worldPosition.z);


            float Ax,Bx,Cx,Ay,By,Cy,d,e,f,num;
            float x1lo,x1hi,y1lo,y1hi;            

            Ax = p2.x-p1.x;
            Bx = p3.x-p4.x;            

            // X bound box test/
            //Find the higher x value of line 1
            if(Ax<0) 
            {
                x1lo=p2.x; x1hi=p1.x;
            } 
            else
            {
                x1hi=p2.x; x1lo=p1.x;
            }
            // X Bounding box test
            if(Bx>0) 
            {
                if(x1hi < p4.x || p3.x < x1lo) 
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            } 
            else 
            {
                if(x1hi < p3.x || p4.x < x1lo)
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            }

            Ay = p2.y-p1.y;
            By = p3.y-p4.y;

            // Y bound box test//
            //Find the higher y value of line 1
            if(Ay<0) 
            {                
                y1lo=p2.y; y1hi=p1.y;
            } 
            else 
            {
                y1hi=p2.y; y1lo=p1.y;
            }
            // Y Bounding box test
            if(By>0) 
            {
                if(y1hi < p4.y || p3.y < y1lo)
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            } 
            else 
            {
                if(y1hi < p3.y || p4.y < y1lo)
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            }


            Cx = p1.x-p3.x;
            Cy = p1.y-p3.y;

            d = By*Cx - Bx*Cy;  // alpha numerator//
            f = Ay*Bx - Ax*By;  // both denominator//

            

            // alpha tests//
            if(f>0) 
            {
                if(d<0 || d>f)
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            } 
            else 
            {
                if(d>0 || d<f)
                {
                    if(_debug)
                    {
                        //Debug.DrawLine(_e1.Center, _e2.Center, Color.yellow, 5.0f);
                    }
                    return false;
                }
            }           

            e = Ax*Cy - Ay*Cx;  // beta numerator//
            // beta tests //
            if(f>0) 
            {                           
                if(e<0 || e>f) return false;
            } 
            else 
            {
                if(e>0 || e<f) return false;
            }

            // check if they are parallel
            if(f==0) return false;
            
            // compute intersection coordinates //
            num = d*Ax; // numerator //
            intersectionPosition.x = p1.x + num / f;
            num = d*Ay;
            intersectionPosition.z = p1.y + num / f;
            num = d*(_e1.p1.worldPosition.y - _e1.p1.worldPosition.y);
            intersectionPosition.y = _e1.p1.worldPosition.y + num / f;

            //Check if the new interactionpoint is actually one of the original 4 points
            if((intersectionPosition - _e1.p1.worldPosition).sqrMagnitude <= .001f)
            {
                if(_debug)
                    Debug.Log("[VectorBuilding -> LineIntersection] Intersection is actually e1p1");
                _intersectionPoint = _e1.p1;
            }
            else if((intersectionPosition - _e1.p2.worldPosition).sqrMagnitude <= .0001f)
            {
                if(_debug)
                    Debug.Log("[VectorBuilding -> LineIntersection] Intersection is actually e1p2");
                _intersectionPoint = _e1.p2;
            }
            else if((intersectionPosition - _e2.p1.worldPosition).sqrMagnitude <= .0001f)
            {
                if(_debug)
                    Debug.Log("[VectorBuilding -> LineIntersection] Intersection is actually e2p1");
                _intersectionPoint = _e2.p1;
            }
            else if((intersectionPosition - _e2.p2.worldPosition).sqrMagnitude <= .0001f)
            {
                if(_debug)
                    Debug.Log("[VectorBuilding -> LineIntersection] Intersection is actually e2p2");
                _intersectionPoint = _e2.p2;
            }
            else
            {
                if(_debug)
                    Debug.Log("[VectorBuilding -> LineIntersection] Intersection is new point");
                _intersectionPoint = new VectorPoint(intersectionPosition, this);
            }

            
            if(_debug)
            {
                Debug.DrawLine(_e1.Center, _e2.Center, Color.green, 5.0f);
            }
            return true;
        }
        public bool EdgeContainedInEdge(VectorEdge _e1, VectorEdge _e2, out List<VectorPoint> _intersectionPoints, ref List<VectorEdge> _newEdges, ref List<VectorEdge> _bisectedEdges)
        {
            _intersectionPoints = new List<VectorPoint>();
            Vector3 e1P1 = _e1.p1.worldPosition;
            Vector3 e1P2 = _e1.p2.worldPosition;

            Vector3 e2P1 = _e2.p1.worldPosition;
            Vector3 e2P2 = _e2.p2.worldPosition;
            //ignore this, if the edges are the same
            if(_e1.Equals(_e2))
            {
                Debug.DrawRay(_e1.Center, Vector3.up, Color.red, 5.0f);                
                return false;
            }

            //Check if the edges are in the same line, these crossproducts should be 0 in this case        
            Vector3 cross1 = Vector3.Cross(_e1.Vector, (e2P1 - e1P1));
            Vector3 cross2 = Vector3.Cross(_e1.Vector, (e2P2 - e1P1));
            if(cross1 == Vector3.zero && cross2 == Vector3.zero)
            {
                //now we know that this is the same line.
                //Let's sort the 4 points
                Vector3 direction = _e1.Vector.normalized;
                
                float e1p1T = 0;
                float e1p2T = Vector3.Dot(direction, (e1P2 - e1P1));
                float e2p1T = Vector3.Dot(direction, (e2P1 - e1P1));
                float e2p2T = Vector3.Dot(direction, (e2P2 - e1P1));
                Debug.Log("[VectorBuilding -> EdgeContainedInEdge] e1t1T:" + e1p1T + " e1p2T:" + e1p2T + " e2p1T:" + e2p1T + " e2p2T:" + e2p2T);

                Debug.DrawLine(_e1.Center, _e2.Center, Color.blue, 5.0f);

                //First check if e1 and e2 even have overlap

                SortedList<float, VectorPoint> sortedPoints = new SortedList<float, VectorPoint>();
                if(!sortedPoints.ContainsKey(e1p1T))
                    sortedPoints.Add(e1p1T, _e1.p1);
                if(!sortedPoints.ContainsKey(e1p2T))
                    sortedPoints.Add(e1p2T, _e1.p2);
                if(!sortedPoints.ContainsKey(e2p1T))
                    sortedPoints.Add(e2p1T, _e2.p1);
                if(!sortedPoints.ContainsKey(e2p2T))
                    sortedPoints.Add(e2p2T, _e2.p2);
                
                //Now just define all of the edges.
                bool e1Contained = false;
                bool e2Contained = false;
                List<VectorEdge> newEdges = new List<VectorEdge>();
                for(int i = 0; i < sortedPoints.Count-1; i++)
                {
                    VectorEdge e = new VectorEdge(sortedPoints.Values[i],sortedPoints.Values[i+1]);
                    if(e.Equals(_e1))
                    {
                        Debug.Log("[VectorBuilding -> EdgeContainedInEdge] Resulting Edges contained _e1");
                        newEdges.Add(_e1);
                        e1Contained= true;
                    }
                    else if(e.Equals(_e2))
                    {
                        Debug.Log("[VectorBuilding -> EdgeContainedInEdge] Resulting Edges contained _e2");
                        newEdges.Add(_e2);
                        e2Contained =true;
                    }
                    else
                    {
                        Debug.Log("[VectorBuilding -> EdgeContainedInEdge] Resulting Edges contained new edge");
                        newEdges.Add(e);
                    }
                }
                if(e1Contained && e2Contained)
                {
                    //These two edges are following separate from each other
                    Debug.Log("[VectorBuilding -> EdgeContainedInEdge] The two edges are parallel but don't overlap");
                    return false;
                }
                _newEdges.AddRange(newEdges);
                if(!e1Contained)
                {
                    Debug.Log("[VectorBuilding -> EdgeContainedInEdge] e1 was cut");
                    _bisectedEdges.Add(_e1);
                }
                if(!e2Contained)
                {
                    Debug.Log("[VectorBuilding -> EdgeContainedInEdge] e2 was cut");
                    _bisectedEdges.Add(_e2);
                }

                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class RoofGraph : VectorPointGraph
    {
        public List<VectorPoint> roofPoints;

        public enum RoofType
        {
            Gabled,
            Hip
        }

        public float roofHeight = 2.0f;
        public float gableInset = 2.0f;

        public RoofGraph()
        {
            points = new List<VectorPoint>();
            edges =  new List<VectorEdge>();
            graphColor = UnityEngine.Random.ColorHSV(0f,1f, 0.5f,1f,.5f,1f);
        }

        public void GenerateRoofFromRect(float _width, float _height)
        {
            //Find the longer direction of the square
            if(_width < _height)
            {
                //Debug.Log("[VectorBuilding -> RoofGraph] widthRoof " + _width + "/" + _height);
                Vector3 roofPoint1 = (points[0].worldPosition + points[1].worldPosition) /2.0f + Vector3.up * roofHeight;
                Vector3 roofPoint2 = (points[2].worldPosition + points[3].worldPosition) /2.0f + Vector3.up * roofHeight;

                roofPoint1 += gableInset * (roofPoint2-roofPoint1).normalized;
                roofPoint2 += gableInset * (roofPoint1-roofPoint2).normalized;
                
                VectorPoint vrp1 = new VectorPoint(roofPoint1, this);                
                VectorPoint vrp2 = new VectorPoint(roofPoint2, this);
                
                AddEdge(new VectorEdge(vrp1, vrp2));

                AddEdge(new VectorEdge(vrp1, points[0]));
                AddEdge(new VectorEdge(vrp1, points[1]));

                AddEdge(new VectorEdge(vrp2, points[2]));
                AddEdge(new VectorEdge(vrp2, points[3]));
                
                points.Add(vrp1);
                points.Add(vrp2);
            }
            else
            {
                //Debug.Log("[VectorBuilding -> RoofGraph] heightRoof " + _width + "/" + _height);
                Vector3 roofPoint1 = (points[0].worldPosition + points[3].worldPosition) /2.0f + Vector3.up * roofHeight;
                Vector3 roofPoint2 = (points[1].worldPosition + points[2].worldPosition) /2.0f + Vector3.up * roofHeight;

                roofPoint1 += gableInset * (roofPoint2-roofPoint1).normalized;
                roofPoint2 += gableInset * (roofPoint1-roofPoint2).normalized;
                
                VectorPoint vrp1 = new VectorPoint(roofPoint1, this);                
                VectorPoint vrp2 = new VectorPoint(roofPoint2, this);
                
                AddEdge(new VectorEdge(vrp1, vrp2));

                AddEdge(new VectorEdge(vrp1, points[0]));
                AddEdge(new VectorEdge(vrp1, points[3]));

                AddEdge(new VectorEdge(vrp2, points[1]));
                AddEdge(new VectorEdge(vrp2, points[2]));
                
                points.Add(vrp1);
                points.Add(vrp2);
            }
            
            //Place points at the center of the shorter sides

        }
    }

}
