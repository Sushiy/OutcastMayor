using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;
using Shapes;
using Unity.VisualScripting;

public class FloorMeshBuilding : MonoBehaviour
{
    public List<BoxCollider> colliders;

    public BoxCollider boxCollider1;
    public BoxCollider boxCollider2;

    List<Vector3> edgePoints;

    public Polyline polyLine;

    // Start is called before the first frame update
    void Start()
    {

    }

    class Edge
    {
        public Vector3 A;
        public Vector3 B;

        public Edge(Vector3 _A, Vector3 _B)
        {
            A = _A;
            B = _B;
        }

        public bool GetIntersectionPoint(Edge _otherEdge, ref Vector3 intersectionPoint)
        {
            Vector2 p1 = new Vector2(A.x, A.z);
            Vector2 p2 = new Vector2(B.x, B.z);
            Vector2 p3 = new Vector2(_otherEdge.A.x, _otherEdge.A.z);
            Vector2 p4 = new Vector2(_otherEdge.B.x, _otherEdge.B.z);
        
            float Ax,Bx,Cx,Ay,By,Cy,d,e,f,num/*,offset*/;
        
            float x1lo,x1hi,y1lo,y1hi;
        
        
        
            Ax = p2.x-p1.x;
        
            Bx = p3.x-p4.x;
        
        
        
            // X bound box test/
        
            if(Ax<0) {
        
                x1lo=p2.x; x1hi=p1.x;
        
            } else {
        
                x1hi=p2.x; x1lo=p1.x;
        
            }
        
        
        
            if(Bx>0) {
        
                if(x1hi < p4.x || p3.x < x1lo) return false;
        
            } else {
        
                if(x1hi < p3.x || p4.x < x1lo) return false;
        
            }
        
        
        
            Ay = p2.y-p1.y;
        
            By = p3.y-p4.y;
        
        
        
            // Y bound box test//
        
            if(Ay<0) {                  
        
                y1lo=p2.y; y1hi=p1.y;
        
            } else {
        
                y1hi=p2.y; y1lo=p1.y;
        
            }
        
        
        
            if(By>0) {
        
                if(y1hi < p4.y || p3.y < y1lo) return false;
        
            } else {
        
                if(y1hi < p3.y || p4.y < y1lo) return false;
        
            }
        
        
        
            Cx = p1.x-p3.x;
        
            Cy = p1.y-p3.y;
        
            d = By*Cx - Bx*Cy;  // alpha numerator//
        
            f = Ay*Bx - Ax*By;  // both denominator//
        
        
        
            // alpha tests//
        
            if(f>0) {
        
                if(d<0 || d>f) return false;
        
            } else {
        
                if(d>0 || d<f) return false;
        
            }
        
        
        
            e = Ax*Cy - Ay*Cx;  // beta numerator//
        
        
        
            // beta tests //
        
            if(f>0) {                          
        
            if(e<0 || e>f) return false;
        
            } else {
        
            if(e>0 || e<f) return false;
        
            }
        
        
        
            // check if they are parallel
        
            if(f==0) return false;
            
            // compute intersection coordinates //
        
            num = d*Ax; // numerator //
        
            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
            
            //    intersection.x = p1.x + (num+offset) / f;
            intersectionPoint.x = p1.x + num / f;
            
            
            
                num = d*Ay;
            
            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
            
            //    intersection.y = p1.y + (num+offset) / f;
            intersectionPoint.z = p1.y + num / f;
            intersectionPoint.y = A.y;
            
            
            return true;
        }
    }

    [Button]
    public void MakeContour()
    {
        Vector3 center = boxCollider1.transform.position;
        Vector3 halfSize = boxCollider1.transform.localScale/2;
        Vector3[] b1Points =
        {
            center + halfSize.x * boxCollider1.transform.right + halfSize.z * boxCollider1.transform.forward,
            center - halfSize.x * boxCollider1.transform.right + halfSize.z * boxCollider1.transform.forward,
            center - halfSize.x * boxCollider1.transform.right - halfSize.z * boxCollider1.transform.forward,
            center + halfSize.x * boxCollider1.transform.right - halfSize.z * boxCollider1.transform.forward
        };

        center = boxCollider2.transform.position;
        halfSize = boxCollider2.transform.localScale/2;
        Vector3[] b2Points =
        {
            center + halfSize.x * boxCollider2.transform.right + halfSize.z * boxCollider2.transform.forward,
            center - halfSize.x * boxCollider2.transform.right + halfSize.z * boxCollider2.transform.forward,
            center - halfSize.x * boxCollider2.transform.right - halfSize.z * boxCollider2.transform.forward,
            center + halfSize.x * boxCollider2.transform.right - halfSize.z * boxCollider2.transform.forward
        };

        //calculate graph
        {
            //Check each side against each other side.
            Edge[] polygon1 = 
            {
                new Edge(b1Points[0], b1Points[1]),
                new Edge(b1Points[1], b1Points[2]),
                new Edge(b1Points[2], b1Points[3]),
                new Edge(b1Points[3], b1Points[0])
            };
            Edge[] polygon2 = 
            {
                new Edge(b2Points[0], b2Points[1]),
                new Edge(b2Points[1], b2Points[2]),
                new Edge(b2Points[2], b2Points[3]),
                new Edge(b2Points[3], b2Points[0])
            };

            List<Vector3> polygon1Points = new List<Vector3>();
            List<Vector3> polygon2Points = new List<Vector3>();
            List<Edge> polygon1Edges = new List<Edge>();
            List<Edge> polygon2Edges = new List<Edge>();

            for(int n = 0; n < polygon1.Length; n++)
            {
                polygon1Points.Add(polygon1[n].A);
                for(int m = 0; m < polygon2.Length; m++)
                {
                    polygon2Points.Add(polygon2[m].A);
                    Vector3 intersectionPoint = Vector3.zero;
                    if(polygon1[n].GetIntersectionPoint(polygon2[m], ref intersectionPoint))
                    {
                        polygon1Points.Add(intersectionPoint);
                        polygon2Points.Add(intersectionPoint);
                        polygon1Edges.Add(new Edge(polygon1[n].A, intersectionPoint));
                        polygon1Edges.Add(new Edge(intersectionPoint, polygon1[n].B));
                        polygon2Edges.Add(new Edge(polygon2[m].A, intersectionPoint));
                        polygon2Edges.Add(new Edge(intersectionPoint, polygon2[m].B));
                    }
                    else
                    {
                        polygon1Edges.Add(polygon1[n]);
                        polygon2Edges.Add(polygon2[m]);
                    }
                    polygon2Points.Add(polygon2[n].B);
                }
                polygon1Points.Add(polygon1[n].B);
            }
            
            Vector3 startVertex = Vector3.positiveInfinity;
            for(int i = 0; i < polygon1Points.Count; i++)
            {
                if(polygon1Points[i].x < startVertex.x ||polygon1Points[i].z < startVertex.z)
                {
                    startVertex = polygon1Points[i];
                }
                Debug.DrawRay(polygon1Points[i], Vector3.up, Color.red, 1.0f);
            }
            
            for(int i = 0; i < polygon2Points.Count; i++)
            {
                if(polygon2Points[i].x < startVertex.x ||polygon2Points[i].z < startVertex.z)
                {
                    startVertex = polygon2Points[i];
                }
                Debug.DrawRay(polygon2Points[i], Vector3.up, Color.red, 1.0f);
            }

            Debug.DrawRay(startVertex, Vector3.up, Color.blue, 1.0f);

            Vector3 currentVertex = startVertex;

        }

        polyLine.transform.InverseTransformPoints(b1Points);
        polyLine.SetPoints(b1Points);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
