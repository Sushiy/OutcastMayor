using System;
using System.Collections.Generic;
using OutcastMayor.Building;
using OutcastMayor.Requests;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace OutcastMayor.Requests
{
    public class PlacedBuildableGoal : RequestGoal
    {
        public Buildable buildable;
        public int targetCount = 1;
        List<Buildable> placedBuildables;
        int currentCount = 0;

        BuildingMode buildingMode;

        public PlacedBuildableGoal()
        {
            
        }

        public PlacedBuildableGoal(PlacedBuildableGoal _source)
        {
            description = _source.description;
            buildable = _source.buildable;
            targetCount = _source.targetCount;
        }

        public override void Init(string _npcName, System.Action _callback)
        {
            base.Init(_npcName, _callback);
            placedBuildables = new List<Buildable>();
            buildingMode = Player.Instance.BuildingMode;
            buildingMode.onBuildablePlaced += OnBuildablePlaced;
        }

        void OnBuildablePlaced(Buildable placedBuildable)
        {
            if(placedBuildable.Buildpiece_ID == buildable.Buildpiece_ID)
            {
                placedBuildables.Add(placedBuildable);
                if(placedBuildable.isBlueprint)
                {
                    placedBuildable.OnBuildableCompleted += OnBuildableCompleted;
                }
                else
                {
                    Increment();
                }
            }
        }

        void OnBuildableCompleted(Buildable completedBuildable)
        {
            completedBuildable.OnBuildableCompleted -= OnBuildableCompleted;
            Increment();
        }

        void OnBuildableDestroyed(Buildable destroyedBuildable)
        {
            if(destroyedBuildable.Buildpiece_ID == buildable.Buildpiece_ID)
            {
                if(placedBuildables.Contains(destroyedBuildable))
                {
                    Decrement();
                }
                else
                {
                    Debug.LogWarning($"Destroyed {destroyedBuildable.Buildpiece_ID}, which is counted for {npcName}s request, but it was not listed");
                }
            }
        }

        void Increment()
        {
            currentCount++;
            checkGoalCallback?.Invoke();
        }

        void Decrement()
        {
            currentCount--;   
            checkGoalCallback?.Invoke();     
        }

        public override bool CheckGoal()
        {
            if(placedBuildables.Count >= targetCount)
            {
                return true;
            }
            return false;
        }

        public override void Clear()
        {
            buildingMode.onBuildablePlaced -= OnBuildablePlaced;
        }

        public override RequestGoal GetCopy()
        {
            return new PlacedBuildableGoal(this);
        }

        public override string GetGoalDescription()
        {
            return $"{description} ({currentCount}/{targetCount})";
        }
    }
}