using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace OutcastMayor.Building
{
    public class FoundationConstruction : Construction
    {
        [SerializeField]
        Transform legsParent;
        protected static LayerMask groundLayer = 1 << 11;
        public void SetGroundHeight()
        {
            RaycastHit findGround;
            if (Physics.Raycast(transform.position, -transform.up, out findGround, 5, groundLayer))
            {
                float height = findGround.distance;
                legsParent.localScale = new Vector3(1, height, 1);
            }
        }

        protected override void OnSetPosition()
        {
            SetGroundHeight();
        }
    }
}