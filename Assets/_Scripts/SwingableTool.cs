using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableTool : MonoBehaviour
{
    Collider _collider;
    Rigidbody _rigidbody;

    Vector3 _oldPosition;
    Vector3 _velocity;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Equip()
    {
        gameObject.SetActive(true);

        //Set collider to inactive just in case
        _collider.enabled = false;
    }

    public void OnStartSwing()
    {
        _collider.enabled = true;
        StartCoroutine(Swinging());
    }

    public void OnEndSwing()
    {
        print("OnEndSwing");
        _collider.enabled = false;
        StopCoroutine(Swinging());
    }

    IEnumerator Swinging()
    {
        while(true)
        {
            _velocity = (_collider.transform.position - _oldPosition) / Time.deltaTime;
            _oldPosition = _collider.transform.position;
            yield return new WaitForSeconds(.25f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        print("Tool hit anything");
        if(other.gameObject.layer == 10)
        {
            print("Tool: " + gameObject.name + " hit Hittable: " + other.gameObject.name);

            other.gameObject.GetComponentInParent<IHittable>().OnHit(other.ClosestPointOnBounds(_collider.transform.position), _velocity);
        }
    }
}
