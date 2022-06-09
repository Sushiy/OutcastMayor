using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour
{
    Item parentItem;
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Bounce(Vector3 force, Vector3 position)
    {
        rigidbody.AddForceAtPosition(force, position, ForceMode.Impulse);
        Debug.DrawRay(position, force, Color.red, 1.0f);
    }
}
