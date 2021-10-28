using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    /// <summary>
    /// All Floors that make up the layout of the room.
    /// </summary>
    public List<Floor> floors;
    /// <summary>
    /// All the detected buildables, that make up the walls of the room. These are detected by the floors
    /// </summary>
    public List<Buildable> walls;

    public Color roomColor;
    public Room()
    {
        floors = new List<Floor>();
        walls = new List<Buildable>();
        roomColor = Random.ColorHSV(0,1,1,1,1,1);
    }

    public void AddFloor(Floor f)
    {
        floors.Add(f);
    }

    public void RemoveFloor(Floor f)
    {
        floors.Remove(f);
    }

    public void MergeRoom(Room r)
    {
        int count = 0;
        for(int i = r.floors.Count-1; i >= 0; i--)
        {
            count++;
            r.floors[i].SetRoom(this);
        }
        Debug.Log("Merged " + count + "floors");
    }

    public void SplitRoom(Floor[] separatedFloors)
    {
        Room r = RoomManager.CreateRoom();
        for(int i = 0; i < separatedFloors.Length; i++)
        {
            separatedFloors[i].SetRoom(r);
        }
        RoomManager.CheckRoomIntegrity(r);
    }

    /// <summary>
    /// Check if the room is valid. i.e.: All it's floors are valid AND the resulting walls can be traced in a line.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckValidity()
    {
        //Proposed function
        //1. Check all of your floors validities
        //2. Gather the walls from the floor validity checks
        //3. Try to find a circular path around the walls
        //4. Anything extra we might add (chimneys?)
        yield return null;
    }

    /// <summary>
    /// Check the rooms floor continuity by traversing all of the floors. if the resulting List doesn't match the current floor list, update the room
    /// </summary>
    public IEnumerator CheckFloorContinuity()
    {
        List<Floor> uncheckedFloors = new List<Floor>(floors);
        HashSet<Floor> checkedFloors = new HashSet<Floor>();
        Queue<Floor> floorQueue = new Queue<Floor>();

        int uncheckedFloorCount = floors.Count;
        int checkCounter = 0;

        floorQueue.Enqueue(uncheckedFloors[0]);
        uncheckedFloors.RemoveAt(0);
        while (floorQueue.Count > 0)
        {
            Debug.Log("Unchecked Floors: " + uncheckedFloorCount + " Queue Count = " + floorQueue.Count);

            //work down the queue
            Floor activeFloor = floorQueue.Dequeue();
            checkedFloors.Add(activeFloor);
            activeFloor.OnChecked?.Invoke();
            Buildable.OverlapResult overlapResult = activeFloor.CheckOverlap();
            for(int i = 0; i < overlapResult.touchedFloors.Count; i++)
            {
                if(overlapResult.touchedRooms.Count > 1)
                {
                    Debug.LogError("You are touching another room!");
                }
                if (!checkedFloors.Contains(overlapResult.touchedFloors[i]) && !floorQueue.Contains(overlapResult.touchedFloors[i]))
                {
                    floorQueue.Enqueue(overlapResult.touchedFloors[i]);
                    if(overlapResult.touchedFloors[i].room == this)
                    {
                        uncheckedFloors.Remove(overlapResult.touchedFloors[i]);
                    }
                    else
                    {
                        Debug.LogError("You are touching another room!");
                    }
                }
            }
            uncheckedFloorCount--;
            checkCounter++;

            //skip to next frame (one floor per frame)
            yield return new WaitForSeconds(.125f);
        }

        if(uncheckedFloorCount == 0)
        {
            Debug.Log("Room still okay");
        }
        else if( uncheckedFloorCount > 0)
        {
            //All those which were not checked are a new room!
            Debug.Log("Room needs to be split");
            SplitRoom(uncheckedFloors.ToArray());
        }
        else if(checkCounter > floors.Count)
        {
            Debug.Log("Room found additional Floors");
        }

    }
}
