using OutcastMayor.Building;
using System;
using UnityEngine;

namespace OutcastMayor.Requests
{
    [System.Serializable]
    public class ValidRoomGoal : RequestGoal
    {
        RoomManager roomManager;

        public ValidRoomGoal()
        {
            
        }

        public ValidRoomGoal(ValidRoomGoal _source)
        {
            description = _source.description;
        }

        public override void Init(string _npcName, Action _checkGoalCallback)
        {
            base.Init(_npcName, _checkGoalCallback);
            roomManager = RoomManager.instance;
            if(roomManager != null)
            {
                roomManager.OnRoomValidated += checkGoalCallback;
            }
        }

        public override void Clear()
        {
            if(roomManager != null)
            {
                roomManager.OnRoomValidated -= checkGoalCallback;
            }
            roomManager = null;
        }

        public override bool CheckGoal()
        {
            return RoomManager.HasValidRoom();
        }

        public override RequestGoal GetCopy()
        {
            return new ValidRoomGoal(this);
        }
    }
}