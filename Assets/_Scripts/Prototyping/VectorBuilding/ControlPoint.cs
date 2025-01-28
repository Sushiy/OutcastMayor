using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using UnityEngine;

public class ControlPoint : ControlElement
{
    public VectorPoint vectorPoint;

    public VectorBuilding vectorBuilding;

    public void SetData(VectorPoint _vectorPoint, VectorBuilding _vectorBuilding)
    {
        vectorPoint = _vectorPoint;
        transform.position = vectorPoint.worldPosition;
        vectorBuilding = _vectorBuilding;
    }
}
