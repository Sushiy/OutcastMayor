using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
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
}