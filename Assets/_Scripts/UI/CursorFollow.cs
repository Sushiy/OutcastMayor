using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CursorFollow : MonoBehaviour
{
    public Vector2 offset;
    Vector2 position;
    RectTransform rTransform;

    private void OnEnable()
    {
        rTransform = GetComponent<RectTransform>();
    }

    public void OnPosition(CallbackContext c)
    {
        if(gameObject.activeInHierarchy)
        {
            position = c.ReadValue<Vector2>();
            rTransform.anchoredPosition = position + offset;
        }
    }
}
