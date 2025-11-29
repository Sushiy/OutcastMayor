using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
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

        /// <summary>
        /// Was the last validity check sucessful or not?
        /// </summary>
        public bool isValid = false;
        /// <summary>
        /// Has the room changed since the last validitycheck?
        /// </summary>
        public bool IsDirty = false;

        public HashSet<NPC_Data> assignedNPCs;

        public Color roomColor;

        private Vector3 roomCenter;

        public Room(Color c)
        {
            floors = new List<Floor>();
            walls = new List<Buildable>();
            roomColor = c;
        }

        public void AddFloor(Floor f)
        {
            floors.Add(f);
        }

        public void RemoveFloor(Floor f)
        {
            floors.Remove(f);
        }


        public void SplitRoom(Floor[] separatedFloors)
        {
            Room r = RoomManager.CreateRoom();
            for (int i = 0; i < separatedFloors.Length; i++)
            {
                separatedFloors[i].SetRoom(r);
            }
            RoomManager.CheckRoomIntegrity(r);
            RoomManager.CheckRoomValidity(r);
        }

        private void OnValidityCheckComplete(bool valid)
        {
            //only spawn particles if the room was not valid before!
            if (valid && !isValid)
            {
                RoomManager.SpawnValidRoomFX(roomCenter);
            }

            isValid = valid;
            Debug.Log("Validity: " + isValid);

            RoomManager.RoomCheckResult(this, valid);
        }

        /// <summary>
        /// Check if the room is valid. i.e.: All it's floors are valid AND the resulting walls can be traced in a line.
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckValidity()
        {
            bool valid = true;
            //Proposed function
            //0.Clear walls?
            walls.Clear();
            Vector3 floorPosSum = Vector3.zero;
            //1. Check all of your floors validities
            for (int i = 0; i < floors.Count; i++)
            {
                floorPosSum += floors[i].transform.position;
                floors[i].CheckValidity();
                //1b. Gather the walls from the floor validity checks
                walls.AddRange(floors[i].LastValidityResult.walls);
                //1c. If any of the floors are invalid, the room is invalid
                if (!floors[i].LastValidityResult.isValid)
                    valid = false;
                yield return null;
            }
            roomCenter = floorPosSum / floors.Count;
            //2. Try to find a circular path around the walls
            //3. Anything extra we might add (chimneys?)
            yield return null;
            OnValidityCheckComplete(valid);
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
                //Debug.Log("Unchecked Floors: " + uncheckedFloorCount + " Queue Count = " + floorQueue.Count);

                //work down the queue
                Floor activeFloor = floorQueue.Dequeue();
                checkedFloors.Add(activeFloor);
                activeFloor.OnChecked?.Invoke();
                Buildable.OverlapResult overlapResult = activeFloor.CheckRoomOverlap();
                for (int i = 0; i < overlapResult.touchedFloors.Count; i++)
                {
                    if (overlapResult.touchedRooms.Count > 1)
                    {
                        Debug.LogError("You are touching another room!");
                    }
                    if (!checkedFloors.Contains(overlapResult.touchedFloors[i]) && !floorQueue.Contains(overlapResult.touchedFloors[i]))
                    {
                        floorQueue.Enqueue(overlapResult.touchedFloors[i]);
                        if (overlapResult.touchedFloors[i].room == this)
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

            if (uncheckedFloorCount == 0)
            {
                Debug.Log("FloorContinuity: Okay");
            }
            else if (uncheckedFloorCount > 0)
            {
                //All those which were not checked are a new room!
                Debug.Log("FloorContinuity: Split");
                SplitRoom(uncheckedFloors.ToArray());
            }
            else if (checkCounter > floors.Count)
            {
                Debug.Log("FloorContinuity: Merge");
            }

        }
    }


}