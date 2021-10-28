using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [SerializeField]
    private List<Room> rooms;

    private void Awake()
    {
        instance = this;
    }

    public static Room CreateRoom()
    {
        Room r = new Room();
        instance.rooms.Add(r);
        return r;
    }

    public static void CheckRoomIntegrity(Room r)
    {
        instance.StartCoroutine(r.CheckFloorContinuity());
    }
}
