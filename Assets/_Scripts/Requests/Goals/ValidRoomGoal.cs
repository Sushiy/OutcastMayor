using OutcastMayor.Building;
using System;
using UnityEngine;

namespace OutcastMayor.Requests
{
    [System.Serializable]
    public class ValidRoomGoal : RequestGoal
    {
        RoomManager roomManager;

        public override void Init(string _npcName, Action _callback)
        {
            base.Init(_npcName, _callback);
            roomManager = RoomManager.instance;
            if(roomManager != null)
            {
                roomManager.OnRoomValidated += _callback;
            }
        }

        public override void Clear(Action _callback)
        {
            if(roomManager != null)
            {
                roomManager.OnRoomValidated -= _callback;
            }
            roomManager = null;
        }

        public override bool CheckGoal()
        {
            return RoomManager.HasValidRoom();
        }
    }
}