using OutcastMayor.Building;
using OutcastMayor.UtilityAI;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace OutcastMayor.Requests
{
    [System.Serializable]
    public class ValidPathGoal : RequestGoal
    {
        [SerializeField]
        private string startPositionMarkerID = "";
        private Vector3 startPosition = Vector3.zero;
        [SerializeField]
        private string targetPositionMarkerID = "";
        private Vector3 targetPosition = Vector3.zero;
        [SerializeField]
        private bool shouldGoAfterCompetion;

        public override void Init(string _npcName, System.Action _callback)
        {
            base.Init(_npcName, _callback);
            startPosition = Blackboard.GetRequestPositionMarker(startPositionMarkerID).transform.position;
            targetPosition = Blackboard.GetRequestPositionMarker(targetPositionMarkerID).transform.position;
        }

        public override void Clear(System.Action callback)
        {
            
        }

        public override bool CheckGoal()
        {
            if(startPosition == null || targetPosition == null) return false;
            if(isCompleted) return true;

            NavMeshPath path = new NavMeshPath();
            bool foundPath = NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, path);
            if(foundPath)
            {
                Color c;
                if(path.status == NavMeshPathStatus.PathComplete)
                {
                    c = Color.green;
                }
                else if(path.status == NavMeshPathStatus.PathPartial)
                {
                    c = Color.yellow;
                }
                else
                {
                    c = Color.red;
                }

                for(int i = 0; i < path.corners.Length-1;i++)
                {
                    Debug.DrawLine(path.corners[i],path.corners[i+1], c, 5.0f);
                }
                
                if(shouldGoAfterCompetion)
                {
                    NPCManager.GetNPCByName(npcName).GoTo(targetPosition);
                }

                return path.status == NavMeshPathStatus.PathComplete;
            }
            else
            {
                Debug.DrawLine(startPosition,targetPosition, Color.red, 5.0f);
            }
            return false;
        }
    }
}