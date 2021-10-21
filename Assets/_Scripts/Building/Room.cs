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
        print("Room is valid = " + CheckRoomValidity());
    }

    public void AddBuildable(Buildable b)
    {
        walls.Add(b);
        print("Room is valid = " + CheckRoomValidity());
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

    private bool CheckRoomValidity()
    {
        //1. Have a bunch of connected floors

        //2. Check each floor and check if each of the sides is connected to another floor or a wall. If that is true for all 4 the floor tile is valid.
        bool valid = true;
        for(int i = 0; i < floors.Count; i++)
        {
            if (!CheckFloorValidity(floors[i]))
                valid = false;
        }
        //3. If all connected floor tiles are valid, the room is valid
        return valid;
    }

    private bool CheckFloorValidity(Floor f)
    {
        //RaycastMethod: 1 Raycast Up, 4 raycasts at medium height, 0-4 raycasts at floor height
        RaycastHit roofCheck, northCheck, eastCheck, southCheck, westCheck;

        bool valid = true;

        //1. Check for a roof
        valid = CheckDirectionForBuildable(f.transform.position, Vector3.up, out roofCheck, 30.0f);

        //2. Check for walls (at mid height)
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), f.transform.forward, out northCheck, 1.1f);
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), f.transform.right, out eastCheck, 1.1f);
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), -f.transform.forward, out southCheck, 1.1f);
        valid = CheckDirectionForBuildable(f.transform.position + new Vector3(0, 1, 0), -f.transform.right, out westCheck, 1.1f);


        //3. Check for floors if there are any
        if (northCheck.collider == null)
        {
            valid = CheckDirectionForBuildable(f.transform.position, f.transform.forward, out northCheck, 1.1f) && northCheck.collider.GetComponentInParent<Floor>() != null;
        }
        if (eastCheck.collider == null)
        {
            valid = CheckDirectionForBuildable(f.transform.position, f.transform.right, out eastCheck, 1.1f) && eastCheck.collider.GetComponentInParent<Floor>() != null;
        }
        if (southCheck.collider == null)
        {
            valid = CheckDirectionForBuildable(f.transform.position, -f.transform.forward, out southCheck, 1.1f) && southCheck.collider.GetComponentInParent<Floor>() != null;
        }
        if (westCheck.collider == null)
        {
            valid = CheckDirectionForBuildable(f.transform.position, -f.transform.right, out westCheck, 1.1f) && westCheck.collider.GetComponentInParent<Floor>() != null;
        }

        return valid;
    }

    private bool CheckDirectionForBuildable(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float range)
    {
        if (Physics.Raycast(origin, direction, out hitInfo, range, buildLayer))
        {
            //you hit a buildable above!
            Debug.DrawLine(origin, hitInfo.point, Color.green, 5.0f);
            return true;
        }
        else
        {
            //you didn't hit a buildable above!
            Debug.DrawRay(origin, direction * range, Color.red, 5.0f);
            return false;
        }
    }
}
