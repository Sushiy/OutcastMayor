using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Buildable
{
    public struct ValidityResult
    {
        public enum Type
        {
            None,
            Floor,
            Wall
        }

        public bool isValid
        {
            get
            {
                return hasRoof && northResult != Type.None && eastResult != Type.None && southResult != Type.None && westResult != Type.None;
            }
        }

        public bool hasRoof;

        public Type northResult;
        public Type eastResult;
        public Type southResult;
        public Type westResult;
    }

    public Room room
    {
        private set;
        get;
    }

    /// <summary>
    /// This method tries to sort Buildables into rooms. Rooms should eventually be used to validate buildings.
    /// For now it just keeps the scene hiearchy a little cleaner
    /// </summary>
    /// <param name="snappedTo"></param>
    public override void CheckForRoom(Buildable snappedTo)
    {
        //1. Check the buildcollider "as trigger" for all overlapping objects
        Collider[] colliders = Physics.OverlapBox(transform.position, (buildCollider.size / 2.0f), transform.rotation, buildLayer);
        print("Touched: " + colliders.Length + " buildables");
        //2. Step through all of the colliders and try to find floors.
        List<Room> touchedRooms = new List<Room>();
        int floorCount = 0;
        for(int i = 0; i < colliders.Length; i++)
        {
            Floor f = colliders[i].GetComponentInParent<Floor>();
            if (f == this) continue;
            if (f && CheckVisibility(f))
            {
                floorCount++;
                if (!touchedRooms.Contains(f.room))
                {
                    touchedRooms.Add(f.room);
                }
            }
        }
        print("Touched: " + floorCount + " floors & " + touchedRooms.Count + " rooms");
        //3. Add this floor to an existing room OR create a new one.
        //3a. If this floor touched no room, create a new Room
        if (touchedRooms.Count == 0)
        {
            SetRoom(RoomManager.CreateRoom());
        }
        else if(touchedRooms.Count == 1)
        {
            SetRoom(touchedRooms[0]);
        }
        else
        {
            SetRoom(touchedRooms[0]);
            for (int i = 1; i < touchedRooms.Count; i++)
            {
                touchedRooms[0].MergeRoom(touchedRooms[i]);
            }
        }
        //4. If this Object connects multiple rooms, consolidate them into one

    }

    /// <summary>
    /// Set your room. Also add yourself to that rooms floors and set your rendercolor
    /// </summary>
    /// <param name="r"></param>
    public void SetRoom(Room r)
    {
        if(room != null)
        {
            room.RemoveFloor(this);
        }
        room = r;
        room.AddFloor(this);
        SetRenderColor(r.roomColor);
    }


    private bool CheckVisibility(Floor f)
    {
        RaycastHit raycastHit;
        Buildable b;
        Vector3 dir = f.transform.position - transform.position;
        return !CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), dir, out raycastHit, dir.magnitude, out b);
    }

    /// <summary>
    /// Check if this Floor piece is valid i.e. has a roof, and 4 walls or neighboring floors
    /// </summary>
    /// <param name="roomFloors"></param>
    /// <param name="checkedFloors"></param>
    /// <returns></returns>
    public ValidityResult CheckValidity(ref Queue<Floor> roomFloors, ref HashSet<Floor> checkedFloors)
    {
        ValidityResult result = new ValidityResult();

        //RaycastMethod: 1 Raycast Up, 4 raycasts at medium height, 0-4 raycasts at floor height
        RaycastHit roofCheck, northCheck, eastCheck, southCheck, westCheck;
        bool valid = true;
        OnChecked?.Invoke();

        Buildable lastBuildable;

        //1. Check for a roof
        //valid = CheckDirectionForBuildable(f.transform.position, Vector3.up, out roofCheck, 30.0f, out lastBuildable);

        //2. Check for walls (at mid height)
        valid = CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), transform.forward, out northCheck, 1.1f, out lastBuildable) && valid;
        if (lastBuildable != null)
        {
            lastBuildable.OnChecked?.Invoke();
        }
        valid = CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), transform.right, out eastCheck, 1.1f, out lastBuildable) && valid;
        if (lastBuildable != null)
        {
            lastBuildable.OnChecked?.Invoke();
        }
        valid = CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), -transform.forward, out southCheck, 1.1f, out lastBuildable) && valid;
        if (lastBuildable != null)
        {
            lastBuildable.OnChecked?.Invoke();
        }
        valid = CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), -transform.right, out westCheck, 1.1f, out lastBuildable) && valid;
        if (lastBuildable != null)
        {
            lastBuildable.OnChecked?.Invoke();
        }


        //3. Check for floors if there are any
        if (northCheck.collider == null)
        {
            if (CheckDirectionForBuildable(transform.position, transform.forward, out northCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = northCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                    {
                        roomFloors.Enqueue(neighboringFloor);
                    }
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
            if (CheckDirectionForBuildable(transform.position, transform.right, out eastCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = eastCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                    {
                        roomFloors.Enqueue(neighboringFloor);
                    }
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
            if (CheckDirectionForBuildable(transform.position, -transform.forward, out southCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = southCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                    {
                        roomFloors.Enqueue(neighboringFloor);
                    }
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
            if (CheckDirectionForBuildable(transform.position, -transform.right, out westCheck, 1.1f, out lastBuildable))
            {
                Floor neighboringFloor = westCheck.collider.GetComponentInParent<Floor>();
                if (neighboringFloor != null)
                {
                    if (!checkedFloors.Contains(neighboringFloor) && !roomFloors.Contains(neighboringFloor))
                    {
                        roomFloors.Enqueue(neighboringFloor);
                    }
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

        checkedFloors.Add(this);
        return result;
    }

    private bool CheckDirectionForBuildable(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float range, out Buildable buildable)
    {
        if (Physics.Raycast(origin, direction.normalized, out hitInfo, range, buildLayer))
        {
            //you hit a buildable above!
            Debug.DrawLine(origin, hitInfo.point, Color.green, 1.0f);
            buildable = hitInfo.collider.GetComponentInParent<Buildable>();
            return true;
        }
        else
        {
            //you didn't hit a buildable above!
            Debug.DrawRay(origin, direction.normalized * range, Color.red, 1.0f);
            buildable = null;
            return false;
        }
    }

    public override OverlapResult CheckOverlap()
    {
        OverlapResult result;
        result.touchedFloors = new List<Floor>();
        result.touchedRooms = new List<Room>();

        Collider[] colliders = Physics.OverlapBox(transform.position, buildCollider.size / 2.0f, transform.rotation, buildLayer);
        print("Touched: " + colliders.Length + " buildables");
        //2. Step through all of the objects, find floors and tell them to check their rooms!
        for (int i = 0; i < colliders.Length; i++)
        {
            Floor f = colliders[i].GetComponentInParent<Floor>();
            if (f == this) continue;
            if (f && CheckVisibility(f))
            {
                result.touchedFloors.Add(f);
                if (!result.touchedRooms.Contains(f.room))
                {
                    result.touchedRooms.Add(f.room);
                }
            }
        }
        print("Touched: " + result.touchedFloors.Count + " floors & " + result.touchedRooms.Count + " rooms");
        return result;
    }

}
