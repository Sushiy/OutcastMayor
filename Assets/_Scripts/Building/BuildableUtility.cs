using UnityEngine;

namespace OutcastMayor.Building
{
    public static class BuildableUtility
    {
        public static bool CheckDirectionForBuildable(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float range, out Buildable buildable)
        {
            if (Physics.Raycast(origin, direction.normalized, out hitInfo, range, 1 << LayerConstants.BuildModeOnly))
            {
                //you hit a buildable above!
                Debug.DrawLine(origin, hitInfo.point, Color.green, 1.0f);
                buildable = hitInfo.collider.GetComponentInParent<Buildable>();
                Debug.Log($"{buildable.name} IsBlueprint: {buildable.isBlueprint}");
                return !buildable.isBlueprint;
            }
            else
            {
                //you didn't hit a buildable above!
                Debug.DrawRay(origin, direction.normalized * range, Color.red, 1.0f);
                buildable = null;
                return false;
            }
        }
        public static bool CheckCovered(Vector3 position)
        {
            RaycastHit hit;
            Buildable buildable;
            //1. Check for a roof
            if (BuildableUtility.CheckDirectionForBuildable(position, Vector3.up, out hit, 30.0f, out buildable))
            {
                return true;
            }
            return false;
        }
    }
    
}
