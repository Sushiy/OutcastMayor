using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
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
                    return /*hasRoof && */northResult != Type.None && eastResult != Type.None && southResult != Type.None && westResult != Type.None;
                }
            }

            public bool hasRoof;

            public Type northResult;
            public Type eastResult;
            public Type southResult;
            public Type westResult;

            public Buildable roof;
            public List<Buildable> walls;
        }

        protected ValidityResult validityResult;
        public ValidityResult LastValidityResult
        {
            get
            {
                return validityResult;
            }
        }

        /// <summary>
        /// This method tries to sort Buildables into rooms. Rooms should eventually be used to validate buildings.
        /// For now it just keeps the scene hiearchy a little cleaner
        /// </summary>
        public override void CheckForRoom()
        {
            if (isBlueprint) return;
            //1. Check the buildcollider "as trigger" for all overlapping objects
            Collider[] colliders = Physics.OverlapBox(transform.position, (buildCollider.size / 2.0f), transform.rotation, LayerConstants.BuildModeOnly);
            //print("Touched: " + colliders.Length + " buildables");
            //2. Step through all of the colliders and try to find floors.
            List<Room> touchedRooms = new List<Room>();
            int floorCount = 0;
            for (int i = 0; i < colliders.Length; i++)
            {
                Floor f = colliders[i].GetComponentInParent<Floor>();
                if (f == this) continue;
                if (f && !f.isBlueprint && CheckVisibility(f))
                {
                    floorCount++;
                    if (!touchedRooms.Contains(f.room))
                    {
                        touchedRooms.Add(f.room);
                    }
                }
            }
            //print("Touched: " + floorCount + " floors & " + touchedRooms.Count + " rooms");
            //3. Add this floor to an existing room OR create a new one.
            if (touchedRooms.Count == 0)
            {
                //3a. If this floor belongs to no room, create a new Room
                SetRoom(RoomManager.CreateRoom());
            }
            else if (touchedRooms.Count == 1)
            {
                //3b. If this floor  belongs to only one room, join that room
                SetRoom(touchedRooms[0]);
            }
            else
            {
                //3c. If this floor  belongs to multiple rooms, join the first and merge them to one room
                SetRoom(touchedRooms[0]);
                for (int i = 1; i < touchedRooms.Count; i++)
                {
                    RoomManager.MergeRooms(touchedRooms[0], touchedRooms[i]);
                }
            }

            //4. Check the room you just joined!
            RoomManager.CheckRoomIntegrity(room);
            RoomManager.CheckRoomValidity(room);
        }

        /// <summary>
        /// Set your room. Also add yourself to that rooms floors and set your rendercolor
        /// </summary>
        /// <param name="r"></param>
        public void SetRoom(Room r)
        {
            if (room != null)
            {
                room.RemoveFloor(this);
            }
            room = r;
            room.AddFloor(this);
            //SetRenderColor(r.roomColor);
        }


        protected bool CheckVisibility(Floor f)
        {
            RaycastHit raycastHit;
            Buildable b;
            Vector3 dir = f.transform.position - transform.position;
            return !BuildableUtility.CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), dir, out raycastHit, dir.magnitude, out b);
        }

        /// <summary>
        /// Check if this Floor piece is valid i.e. has a roof, and 4 walls or neighboring floors
        /// </summary>
        /// <param name="roomFloors"></param>
        /// <param name="checkedFloors"></param>
        /// <returns></returns>
        public void CheckValidity()
        {
            validityResult = new ValidityResult();
            validityResult.walls = new List<Buildable>();
            Buildable lastBuildable;

            //RaycastMethod: 1 Raycast Up, 4 raycasts at medium height, 0-4 raycasts at floor height
            RaycastHit roofCheck, northCheck, eastCheck, southCheck, westCheck;
            OnChecked?.Invoke();

            //1. Check for a roof
            if (BuildableUtility.CheckDirectionForBuildable(transform.position, Vector3.up, out roofCheck, 30.0f, out lastBuildable))
            {
                validityResult.hasRoof = true;
                validityResult.roof = lastBuildable;
            }

            //2. Check for walls (at mid height)
            if (BuildableUtility.CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), transform.forward, out northCheck, 1.1f, out lastBuildable))
            {
                validityResult.northResult = ValidityResult.Type.Wall;
                validityResult.walls.Add(lastBuildable);
                lastBuildable.OnChecked?.Invoke();
            }
            if (BuildableUtility.CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), transform.right, out eastCheck, 1.1f, out lastBuildable))
            {
                validityResult.eastResult = ValidityResult.Type.Wall;
                validityResult.walls.Add(lastBuildable);
                lastBuildable.OnChecked?.Invoke();
            }
            if (BuildableUtility.CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), -transform.forward, out southCheck, 1.1f, out lastBuildable))
            {
                validityResult.southResult = ValidityResult.Type.Wall;
                validityResult.walls.Add(lastBuildable);
                lastBuildable.OnChecked?.Invoke();
            }
            if (BuildableUtility.CheckDirectionForBuildable(transform.position + new Vector3(0, 1, 0), -transform.right, out westCheck, 1.1f, out lastBuildable))
            {
                validityResult.westResult = ValidityResult.Type.Wall;
                validityResult.walls.Add(lastBuildable);
                lastBuildable.OnChecked?.Invoke();
            }

            //3. Check for floors if there are any
            if (northCheck.collider == null)
            {
                if (BuildableUtility.CheckDirectionForBuildable(transform.position, transform.forward, out northCheck, 1.1f, out lastBuildable))
                {
                    Floor neighboringFloor = northCheck.collider.GetComponentInParent<Floor>();
                    if (neighboringFloor != null)
                    {
                        validityResult.northResult = ValidityResult.Type.Floor;
                    }
                }
            }
            if (eastCheck.collider == null)
            {
                if (BuildableUtility.CheckDirectionForBuildable(transform.position, transform.right, out eastCheck, 1.1f, out lastBuildable))
                {
                    Floor neighboringFloor = eastCheck.collider.GetComponentInParent<Floor>();
                    if (neighboringFloor != null)
                    {
                        validityResult.eastResult = ValidityResult.Type.Floor;
                    }
                }
            }
            if (southCheck.collider == null)
            {
                if (BuildableUtility.CheckDirectionForBuildable(transform.position, -transform.forward, out southCheck, 1.1f, out lastBuildable))
                {
                    Floor neighboringFloor = southCheck.collider.GetComponentInParent<Floor>();
                    if (neighboringFloor != null)
                    {
                        validityResult.southResult = ValidityResult.Type.Floor;
                    }
                }
            }
            if (westCheck.collider == null)
            {
                if (BuildableUtility.CheckDirectionForBuildable(transform.position, -transform.right, out westCheck, 1.1f, out lastBuildable))
                {
                    Floor neighboringFloor = westCheck.collider.GetComponentInParent<Floor>();
                    if (neighboringFloor != null)
                    {
                        validityResult.westResult = ValidityResult.Type.Floor;
                    }
                }
            }
        }

        public override OverlapResult CheckRoomOverlap()
        {
            OverlapResult result;
            result.touchedFloors = new List<Floor>();
            result.touchedRooms = new List<Room>();

            Collider[] colliders = Physics.OverlapBox(transform.position, buildCollider.size / 2.0f, transform.rotation, LayerConstants.BuildModeOnly);
            print("[Floor->CheckRoomOverlap] Overlapped: " + colliders.Length + " buildables");
            //2. Step through all of the objects, find floors and tell them to check their rooms!
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == buildCollider) 
                {
                    continue;
                }
                Floor f = colliders[i].GetComponentInParent<Floor>();
                if (f == this) 
                {
                    continue;
                }
                if (f && CheckVisibility(f))
                {
                    result.touchedFloors.Add(f);
                    if (!result.touchedRooms.Contains(f.room))
                    {
                        result.touchedRooms.Add(f.room);
                    }
                }
            }
            print("[Floor->CheckRoomOverlap] Overlapped: " + result.touchedFloors.Count + " floors & " + result.touchedRooms.Count + " rooms");
            return result;
        }

    }


}