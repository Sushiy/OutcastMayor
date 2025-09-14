using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    public void TeleportTo(Vector3 position);

    public void SnapYRotation(Quaternion rotation);

    public void LockMovement(bool locked);
}
