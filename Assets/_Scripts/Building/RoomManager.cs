using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [SerializeField]
    private List<Room> rooms;

    [SerializeField]
    private ParticleSystem validRoomFXPrefab;

    private void Awake()
    {
        instance = this;
    }

    public static Room CreateRoom()
    {
        Room r = new Room(Random.ColorHSV(1,1,1,1));
        instance.rooms.Add(r);
        return r;
    }

    public static void RemoveRoom(Room r)
    {
        instance.rooms.Remove(r);
    }

    public static void MergeRooms(Room result, Room merger)
    {
        int count = 0;
        for (int i = merger.floors.Count - 1; i >= 0; i--)
        {
            count++;
            merger.floors[i].SetRoom(result);
        }
        RemoveRoom(merger);
    }

    public static void CheckRoomIntegrity(Room r)
    {
        instance.StartCoroutine(r.CheckFloorContinuity());
    }
    public static void CheckRoomValidity(Room r)
    {
        instance.StartCoroutine(r.CheckValidity());
    }

    public static void SpawnValidRoomFX(Vector3 position)
    {
        Instantiate<ParticleSystem>(instance.validRoomFXPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Check if there are any valid Rooms
    /// /// </summary>
    public static bool HasValidRoom()
    {
        for(int i = 0; i < instance.rooms.Count; i++)
        {
            if(instance.rooms[i].isValid)
                return true;
        }
        return false;
    }
}
