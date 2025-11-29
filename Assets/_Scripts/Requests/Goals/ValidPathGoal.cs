using OutcastMayor.Building;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace OutcastMayor.Requests
{
    [System.Serializable]
    public class ValidPathGoal : RequestGoal
    {
        [SerializeField]
        private Transform startPosition;
        [SerializeField]
        private Transform targetPosition;
        [SerializeField]
        private bool shouldGoAfterCompetion;

    

        public override void Clear(Action callback)
        {
            
        }

        public override bool CheckGoal()
        {
            if(startPosition == null || targetPosition == null) return false;
            if(isCompleted) return true;

            NavMeshPath path = new NavMeshPath();
            bool foundPath = NavMesh.CalculatePath(startPosition.position, targetPosition.position, NavMesh.AllAreas, path);
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
                Debug.DrawLine(startPosition.position,targetPosition.position, Color.red, 5.0f);
            }
            return false;
        }
    }
}