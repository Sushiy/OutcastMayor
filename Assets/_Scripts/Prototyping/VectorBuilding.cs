using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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
            Rectangle,
            Roof
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

        public List<RoofGraph> roofGraphs = new List<RoofGraph>();
        
        public RoofGraph currentRoofGraph;

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

            ControlPoint clickedPoint = inputManager.HitInfo.collider.GetComponent<ControlPoint>();

            if(buildMode == BuildMode.Line)
            {
                ClickRectangleMode(clickPosition,clickedPoint);
            }
            if(buildMode == BuildMode.Rectangle)
            {
                ClickRectangleMode(clickPosition, clickedPoint);
            }
            if(buildMode == BuildMode.Roof)
            {
                ClickRoofMode(clickPosition, clickedPoint);
            }
        }

        public void ClickLineMode(Vector3 clickPosition, ControlPoint clickedPoint)
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

        public void ClickRectangleMode(Vector3 _clickPosition, ControlPoint _clickedPoint)
        {
                if(_clickedPoint != null)
                {
                    if(currentVectorPointGraph == null)
                    {
                        currentVectorPointGraph = _clickedPoint.vectorPoint.vectorPointGraph;
                        currentRectangle.Color = currentVectorPointGraph.graphColor;
                    }

                    if(rectState == 0)
                    {
                        rectP1 = _clickedPoint.vectorPoint;
                        currentRectangleParent.position = _clickedPoint.vectorPoint.worldPosition;
                        rectState = 1;
                    }
                    else if(rectState == 1)
                    {
                        rectP2 = _clickedPoint.vectorPoint;
                        currentRectangleParent.transform.LookAt(rectP2.worldPosition, Vector3.up);
                        currentRectangle.Width = Vector3.Distance(rectP1.worldPosition, rectP2.worldPosition);
                        rectState = 2;
                    }
                    else if(rectState == 2)
                    {
                        Vector3 toClick = _clickPosition - rectP2.worldPosition;
                        float height = Vector3.Dot(toClick, currentRectangleParent.right);
                        if(Vector3.Dot(toClick.normalized, currentRectangleParent.right) == 1)
                        {
                            //you actually hit the point with the corner :o
                            rectP3 = _clickedPoint.vectorPoint;
                        }
                        else
                        {
                            Vector3 p3 = rectP2.worldPosition + height * currentRectangleParent.right;
                            rectP3 = new VectorPoint(p3, rectP2, currentVectorPointGraph);

                        }

                        rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), rectP3, currentVectorPointGraph);
                        
                        bool isOpposite = height > 0;
                        currentRectangle.Height = height;
                        if(isOpposite)
                            currentRectangle.transform.localScale = new Vector3(1,-1,1);
                        else
                            currentRectangle.transform.localScale = new Vector3(1,1,1);

                        //Add finished points
                        
                        rectP1.AddPoint(rectP2);
                        rectP2.AddPoint(rectP1);

                        rectP2.AddPoint(rectP3);
                        rectP3.AddPoint(rectP2);

                        rectP3.AddPoint(rectP4);
                        rectP4.AddPoint(rectP3);

                        rectP4.AddPoint(rectP1);
                        rectP1.AddPoint(rectP4);

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
                        rectP1 = new VectorPoint(_clickPosition, null, currentVectorPointGraph);
                        currentRectangleParent.position = _clickPosition;
                        rectState = 1;
                    }
                    else if(rectState == 1)
                    {
                        rectP2 = new VectorPoint(_clickPosition, rectP1, currentVectorPointGraph);
                        
                        Vector3 dir = rectP2.worldPosition - rectP1.worldPosition;
                        currentRectangleParent.LookAt(rectP2.worldPosition, Vector3.up);
                        currentRectangle.Width = dir.magnitude * Vector3.Dot(dir.normalized, currentRectangleParent.forward);
                        rectState = 2;
                    }
                    else if(rectState == 2)
                    {
                        float height = Vector3.Dot(_clickPosition - rectP2.worldPosition, currentRectangleParent.right);
                        Vector3 p3 = rectP2.worldPosition + height * currentRectangleParent.right;
                        rectP3 = new VectorPoint(p3, rectP2, currentVectorPointGraph);

                        rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), rectP3, currentVectorPointGraph);
                        
                        bool isOpposite = height > 0;
                        currentRectangle.Height = height;
                        if(isOpposite)
                            currentRectangle.transform.localScale = new Vector3(1,-1,1);
                        else
                            currentRectangle.transform.localScale = new Vector3(1,1,1);

                        //Add finished points
                        rectP1.AddPoint(rectP2);
                        rectP2.AddPoint(rectP1);

                        rectP2.AddPoint(rectP3);
                        rectP3.AddPoint(rectP2);

                        rectP3.AddPoint(rectP4);
                        rectP4.AddPoint(rectP3);

                        rectP4.AddPoint(rectP1);
                        rectP1.AddPoint(rectP4);
                        
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

        public void ClickRoofMode(Vector3 _clickPosition, ControlPoint _clickedPoint)
        {
            if(_clickedPoint != null)
            {
                
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
                    rectP1 = new VectorPoint(_clickPosition, null, currentRoofGraph);
                    currentRectangleParent.position = _clickPosition;
                    rectState = 1;
                }
                else if(rectState == 1)
                {
                    rectP2 = new VectorPoint(_clickPosition, rectP1, currentRoofGraph);
                    
                    Vector3 dir = rectP2.worldPosition - rectP1.worldPosition;
                    currentRectangleParent.LookAt(rectP2.worldPosition, Vector3.up);
                    currentRectangle.Width = dir.magnitude * Vector3.Dot(dir.normalized, currentRectangleParent.forward);
                    rectState = 2;
                }
                else if(rectState == 2)
                {
                    float height = Vector3.Dot(_clickPosition - rectP2.worldPosition, currentRectangleParent.right);
                    Vector3 p3 = rectP2.worldPosition + height * currentRectangleParent.right;
                    rectP3 = new VectorPoint(p3, rectP2, currentRoofGraph);

                    rectP4 = new VectorPoint(rectP3.worldPosition + (rectP1.worldPosition - rectP2.worldPosition), rectP3, currentRoofGraph);
                    
                    bool isOpposite = height > 0;
                    currentRectangle.Height = height;
                    if(isOpposite)
                        currentRectangle.transform.localScale = new Vector3(1,-1,1);
                    else
                        currentRectangle.transform.localScale = new Vector3(1,1,1);

                    //Add finished points
                    rectP1.AddPoint(rectP2);
                    rectP2.AddPoint(rectP1);

                    rectP2.AddPoint(rectP3);
                    rectP3.AddPoint(rectP2);

                    rectP3.AddPoint(rectP4);
                    rectP4.AddPoint(rectP3);

                    rectP4.AddPoint(rectP1);
                    rectP1.AddPoint(rectP4);
                    
                    currentRoofGraph.AddPoint(rectP1);
                    currentRoofGraph.AddPoint(rectP2);
                    currentRoofGraph.AddPoint(rectP3);
                    currentRoofGraph.AddPoint(rectP4);
                    currentRoofGraph.GenerateRoofFromRect(currentRectangle.Width, currentRectangle.Width);
                    rectState = 0;
                    currentRoofGraph = null;
                }
            }
        }

        public void AddControlPoint(VectorPoint _point)
        {
            ControlPoint newControlPoint = Instantiate(controlPoint, transform);
            newControlPoint.transform.position = _point.worldPosition;
            newControlPoint.SetData(_point, this);
            ControlPoint newUpperControlPoint = Instantiate(controlPoint, transform);
            newUpperControlPoint.transform.position = _point.upperWorldPosition;
            newUpperControlPoint.SetData(_point, this);
            controlPoints.Add(newControlPoint);
            controlPoints.Add(newUpperControlPoint);
        } 
    }

    public class VectorPoint
    {
        public Vector3 worldPosition;

        public Vector3 upperWorldPosition;

        [DoNotSerialize]
        public List<VectorPoint> connectedPoints;

        public int connectedPointCount = 0;

        [HideInInspector]
        public VectorPointGraph vectorPointGraph;

        public VectorPoint(Vector3 _worldPosition, VectorPoint _previousPoint, VectorPointGraph _vectorPointGraph)
        {
            worldPosition = _worldPosition;
            upperWorldPosition = _worldPosition + Vector3.up * 2f;
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
            includedPoints = new List<VectorPoint>();
            graphColor = UnityEngine.Random.ColorHSV(0f,1f, 0.5f,1f,.5f,1f);
        }

        public void GenerateRoofFromRect(float _width, float _height)
        {
            //Find the longer direction of the square
            if(_width >= _height)
            {
                Vector3 roofPoint1 = (includedPoints[0].worldPosition + includedPoints[1].worldPosition) /2.0f + Vector3.up * roofHeight;
                Vector3 roofPoint2 = (includedPoints[2].worldPosition + includedPoints[3].worldPosition) /2.0f + Vector3.up * roofHeight;

                roofPoint1 += gableInset * (roofPoint2-roofPoint1).normalized;
                roofPoint2 += gableInset * (roofPoint1-roofPoint2).normalized;
                
                VectorPoint vrp1 = new VectorPoint(roofPoint1, includedPoints[0], this);
                VectorPoint vrp2 = new VectorPoint(roofPoint2, vrp1, this);
                vrp1.AddPoint(vrp2);
                vrp1.AddPoint(includedPoints[1]);
                vrp2.AddPoint(includedPoints[2]);
                vrp2.AddPoint(includedPoints[3]);

                includedPoints[0].AddPoint(vrp1);
                includedPoints[1].AddPoint(vrp1);
                includedPoints[2].AddPoint(vrp2);
                includedPoints[3].AddPoint(vrp2);
                
                includedPoints.Add(vrp1);
                includedPoints.Add(vrp2);
            }
            
            //Place points at the center of the shorter sides

        }
    }

}
