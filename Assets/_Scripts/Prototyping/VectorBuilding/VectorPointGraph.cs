
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OutcastMayor
{
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
}