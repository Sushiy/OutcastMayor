using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Interactable
{
    public float maxHealth = 10.0f;
    private float currentHealth;

    public GameObject stage2Prefab;
    public float stage2Impulse = 100.0f;

    private Collider _collider;

    public override void Interact(Interactor interactor)
    {
        Cut(100.0f);
    }

    public void Cut(float damage)
    {
        currentHealth -= damage;
        if( currentHealth <= 0.0f)
        {
            NextStage();
        }
    }

    public void NextStage()
    {
        _collider.enabled = false;
        Rigidbody r = GameObject.Instantiate(stage2Prefab, transform.position, transform.rotation).GetComponentInChildren<Rigidbody>();
        Vector2 circle = Random.insideUnitCircle * stage2Impulse;
        r.AddForce(circle.x, 0, circle.y);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        _collider = GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
