using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Buildable
{

    /// <summary>
    /// This method tries to sort Buildables into rooms. Rooms should eventually be used to validate buildings.
    /// For now it just keeps the scene hiearchy a little cleaner
    /// </summary>
    /// <param name="snappedTo"></param>
    public override void CheckForRoom(Buildable snappedTo)
    {
        //Proposed Function:
        //1. Check the buildcollider "as trigger" for all overlapping objects
        //2. Step through all of the objects and try to find out if they are from the same room/building
        //3. Add this Object to an existing room OR create a new one.
        //4. If this Object connects multiple rooms, consolidate them into one

        if (snappedTo == null || snappedTo.room == null)
        {
            GameObject g = new GameObject("Room");
            g.transform.parent = transform.parent;
            room = g.AddComponent<Room>();
            transform.parent = room.transform;
        }
        else
        {
            room = snappedTo.room;
            transform.parent = room.transform;
        }

        room.AddFloor(this);
    }
}
