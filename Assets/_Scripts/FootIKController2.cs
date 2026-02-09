using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIKController2 : MonoBehaviour
{
    Animator animator;

    [Range(0, 1f)]
    public float heelCorrection;

    [Range(0, 1f)]
    public float baseOffset;


    [Range(0, 1f)]
    public float distanceToGround;

    float maxDistanceToGround;

    [SerializeField]
    LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        transform.localPosition = new Vector3(0, -baseOffset, 0);
        maxDistanceToGround = 0.0f;

        float leftFootWeight = animator.GetFloat("IKLeftFootWeight");
        float rightFootWeight = animator.GetFloat("IKRightFootWeight");

        // Set the weights of left and right feet to the current value defined by the curve in our animations.
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);

        //Raycast down from foot
        Vector3 pos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Quaternion ikRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
        Vector3 ikForwardVector = ikRotation *Vector3.forward;
        Vector3 ikUpVector = ikRotation *Vector3.up;


        Ray ray = new Ray(pos + Vector3.up * distanceToGround, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        {
            Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
            footPosition.y += heelCorrection;

            float d = Mathf.Abs(transform.parent.position.y - footPosition.y);
            if (d > maxDistanceToGround)
            {
                maxDistanceToGround = d;
            }
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            Quaternion r = Quaternion.FromToRotation(ikUpVector, hit.normal);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(r * ikForwardVector, hit.normal));
            Debug.DrawLine(pos, footPosition, Color.cyan);

        }


        pos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        ikRotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
        ikForwardVector = ikRotation * Vector3.forward;
        ikUpVector = ikRotation * Vector3.up;

        ray = new Ray(pos + Vector3.up*distanceToGround, Vector3.down);
        if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        {
            Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
            footPosition.y += heelCorrection;

            float d = Mathf.Abs(transform.parent.position.y - footPosition.y);
            if (d > maxDistanceToGround)
            {
                maxDistanceToGround = d;
            }

            animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
            Quaternion r = Quaternion.FromToRotation(ikUpVector, hit.normal);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(r * ikForwardVector, hit.normal));
            Debug.DrawLine(pos, footPosition, Color.magenta);

        }

        if(maxDistanceToGround <= distanceToGround)
        {
            transform.localPosition = new Vector3(0, -maxDistanceToGround - baseOffset, 0);
        }
    }
}
