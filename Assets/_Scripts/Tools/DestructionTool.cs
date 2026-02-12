using OutcastMayor;
using OutcastMayor.Building;
using UnityEngine;

namespace OutcastMayor
{
    public class DestructionTool : Tool
    {
        Buildable hoveredBuildable;

        bool raycastHit;
        RaycastHit hitInfo;

        public override void OnUseToolPrimary(Character _parentCharacter)
        {
            if(hoveredBuildable != null)
            {
                Destroy(hoveredBuildable);
            }
        }

        public override bool OnProcessRaycast(Vector3 _raycastOrigin, Vector3 _raycastDirection)
        {
            Ray ray = new Ray(_raycastOrigin, _raycastDirection);
            raycastHit = Physics.Raycast(ray, out hitInfo, 10.0f, 1 << LayerConstants.BuildModeOnly);
            if(raycastHit)
            {
                Buildable b = hitInfo.collider.GetComponent<Buildable>();
                if(b != null)
                {
                    if(hoveredBuildable != null && b != hoveredBuildable)
                    {
                        hoveredBuildable.EndHover();
                    }
                    hoveredBuildable = b;
                    hoveredBuildable.StartHover();
                }
            }
            return true;   
        }
    }
    
}
