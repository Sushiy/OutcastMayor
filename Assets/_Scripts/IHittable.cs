using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public void OnHit(Vector3 hitPosition, Vector3 hitForce);

    public void OnBounce(Vector3 hitPosition, Vector3 hitForce);
}
