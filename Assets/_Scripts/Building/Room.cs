using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    public List<Floor> floors;
    public List<Buildable> walls;
    public LayerMask buildLayer = 1 << 7 | 1;

    public Room()
    {
        floors = new List<Floor>();
        walls = new List<Buildable>();
    }

    public void AddFloor(Floor f)
    {
        floors.Add(f);
        StartCoroutine(CheckRoomValidity(OnRoomCheckComplete));
    }

    public void AddBuildable(Buildable b)
    {
        walls.Add(b);
        StartCoroutine(CheckRoomValidity(OnRoomCheckComplete));
    }

    bool OnRoomCheckComplete(bool valid)
    {
        print("Room is valid = " + valid);
        return valid;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        //Draw a line along the snapping points of the walls to see if we get a circle!
        for(int i = 0; i < walls.Count; i++)
        {
            Gizmos.DrawLine(walls[i].StartPoint, walls[i].EndPoint);
        }
    }

    private IEnumerator CheckRoomValidity(Func<bool, bool> onComplete)
    {
        bool valid = true;
        //1. Have a bunch of connected floors

        //2. Check each floor and check if each of the sides is connected to another floor or a wall. If that is true for all 4 the floor tile is valid.
        List<Floor> uncheckedFloors = new List<Floor>(floors);
        HashSet<Floor> checkedFloors = new HashSet<Floor>();
        Queue<Floor> currentRoomFloors = new Queue<Floor>();

        int uncheckedFloorCount = floors.Count;
        int roomCounter = 0;
        int checkCounter = 0;
        while (uncheckedFloorCount > 0)
        {
            print("Unchecked Floors: " + uncheckedFloorCount + " Queue Count = " + currentRoomFloors.Count);
            //refill the queue if it is empty
            if(currentRoomFloors.Count == 0)
            {
                print("roomQueue is empty");
                roomCounter++;
                Floor f = uncheckedFloors[uncheckedFloorCount - 1];
                currentRoomFloors.Enqueue(f);
                uncheckedFloors.Remove(f);
                if(roomCounter > 1)
                    yield return new WaitForSeconds(1.0f);
            }
            //work down the queue
            valid = CheckFloorValidity(currentRoomFloors.Dequeue(), ref currentRoomFloors, ref checkedFloors);
            uncheckedFloorCount--;
            checkCounter++;

            //skip to next frame (one floor per frame)
            yield return new WaitForSeconds(.125f);
        }
        print("Found " + roomCounter + " rooms with " + checkCounter + " floors");
        UIManager.SetRoomCounter(roomCounter);
        //3. If all connected floor tiles are valid, the room is valid
        onComplete(valid);
    }

    private bool CheckFloorValidity(Floor f, ref Queue<Floor> roomFloors, ref HashSet<Floor> checkedFloors)
    {
        //RaycastMethod: 1 Raycast Up, 4 raycasts at medium height, 0-4 raycasts at floor height
        RaycastHit roofCheck, northCheck, eastCheck, southCheck, westCheck;
        bool valid = true;
        f.OnChecked?.Invoke();

        Buildable lastBuildable;

        //1. Check for a roof
        valid = CheckDirectionForBuildable(f.transform.position, Vector3.up, out roofCheck, 30.0f, out lastBuildable);

        //2. Check for walls (at mid height)
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), f.transform.forward, out northCheck, 1.1f, out lastBuildable);
        if (lastBuildable != null)
            lastBuildable.OnChecked?.Invoke();
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), f.transform.right, out eastCheck, 1.1f, out lastBuildable);
        if (lastBuildable != null)
            lastBuildable.OnChecked?.Invoke();
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), -f.transform.forward, out southCheck, 1.1f, out lastBuildable);
        if (lastBuildable != null)
            lastBuildable.OnChecked?.Invoke();
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), -f.transform.right, out westCheck, 1.1f, out lastBuildable);
        if (lastBuildable != null)
            lastBuildable.OnChecked?.Invoke();


        //3. Check for floors if there are any
        if (northCheck.collider == null)
        {
            if (CheckDirectionForBuildable(f.transform.position, f.transform.forward, out northCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = northCheck.collider.GetComponentInParent<Floor>();
                if(neighboringFloor != null)
                {
                    if(!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                        roomFloors.Enqueue(neighboringFloor);
                    valid = true;
                }
                else
                {
                    print("Found a buildable at groundLevel that is not a floor");
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
        }
        if (eastCheck.collider == null)
        {
            if (CheckDirectionForBuildable(f.transform.position, f.transform.right, out eastCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = eastCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                        roomFloors.Enqueue(neighboringFloor);
                    valid = true;
                }
                else
                {
                    print("Found a buildable at groundLevel that is not a floor");
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
        }
        if (southCheck.collider == null)
        {
            if (CheckDirectionForBuildable(f.transform.position, -f.transform.forward, out southCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = southCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                        roomFloors.Enqueue(neighboringFloor);
                    valid = true;
                }
                else
                {
                    print("Found a buildable at groundLevel that is not a floor");
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
        }
        if (westCheck.collider == null)
        {
            if (CheckDirectionForBuildable(f.transform.position, -f.transform.right, out westCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = westCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                        roomFloors.Enqueue(neighboringFloor);
                    valid = true;
                }
                else
                {
                    print("Found a buildable at groundLevel that is not a floor");
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
        }

        checkedFloors.Add(f);
        return valid;
    }

    private bool CheckDirectionForBuildable(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float range, out Buildable buildable)
    {
        if (Physics.Raycast(origin, direction, out hitInfo, range, buildLayer))
        {
            //you hit a buildable above!
            Debug.DrawLine(origin, hitInfo.point, Color.green, 1.0f);
            buildable = hitInfo.collider.GetComponentInParent<Buildable>();
            return true;
        }
        else
        {
            //you didn't hit a buildable above!
            Debug.DrawRay(origin, direction * range, Color.red, 1.0f);
            buildable = null;
            return false;
        }
    }
}
