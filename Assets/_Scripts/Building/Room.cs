using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    List<Floor> floors;

    public Room()
    {
        floors = new List<Floor>();
    }

    public void AddFloor(Floor f)
    {
        floors.Add(f);
    }
}
