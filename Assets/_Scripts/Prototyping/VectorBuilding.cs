using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using JetBrains.Annotations;

namespace OutcastMayor
{
    public class VectorBuilding : MonoBehaviour
    {
        public Polyline polyline;
        public BasicPlayerInputManager inputManager;

        void Awake()
        {
            inputManager.onPrimaryPerformed += PlacePoint;
        }

        public void PlacePoint()
        {       
            Vector3 position = inputManager.HitInfo.point;
            polyline.AddPoint(new PolylinePoint(polyline.transform.InverseTransformDirection(position)));
        }
    }

}
