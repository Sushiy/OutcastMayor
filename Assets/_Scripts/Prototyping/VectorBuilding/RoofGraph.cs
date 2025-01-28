using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OutcastMayor
{
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
