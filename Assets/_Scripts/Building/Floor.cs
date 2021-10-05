using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Buildable
{
    public override void CheckForRoom(Buildable snappedTo)
    {
        if (snappedTo == null || snappedTo.room == null)
        {
            GameObject g = Instantiate<GameObject>(new GameObject("Room"), transform.parent);
            room = g.AddComponent<Room>();
            room.AddFloor(this);
            transform.parent = room.transform;
        }
        else
        {
            room = snappedTo.room;
            room.AddFloor(this);
            transform.parent = room.transform;
        }
    }
}
