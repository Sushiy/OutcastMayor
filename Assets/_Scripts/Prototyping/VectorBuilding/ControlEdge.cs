using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using UnityEngine;

public class ControlEdge : ControlElement
{
    public VectorEdge vectorEdge;

    public VectorBuilding vectorBuilding;

    public void SetData(VectorEdge _vectorEdge, VectorBuilding _vectorBuilding)
    {
        vectorEdge = _vectorEdge;

        transform.position = vectorEdge.Center;
        transform.localScale = new Vector3(1,1, vectorEdge.Length);
        transform.LookAt(vectorEdge.p2.worldPosition);
        vectorBuilding = _vectorBuilding;
    }
}
