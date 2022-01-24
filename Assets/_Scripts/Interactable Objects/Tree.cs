using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Interactable, IHittable
{
    public float maxHealth = 10.0f;
    private float currentHealth;

    public GameObject nextStagePrefab;
    public float nextStageImpulse = 100.0f;

    private Collider _collider;


    [SerializeField]
    private AudioClip[] chopSFX;
    [SerializeField]
    private AudioClip breakSFX;

    RandomAudioClip randomAudioClip;

    public struct ChopPoint
    {
        public Vector3 point;
        public Vector3 direction;

        public ChopPoint(Vector3 point, Vector3 direction)
        {
            this.point = point;
            this.direction = direction;
        }
    }

    public List<ChopPoint> chopPoints;

    public override void Interact(Interactor interactor)
    {

    }

    public void Cut(float damage)
    {
        currentHealth -= damage;
        if( currentHealth <= 0.0f)
        {
            randomAudioClip.PlayClip(breakSFX);
            NextStage();
        }
    }

    public void NextStage()
    {
        _collider.enabled = false;
        Rigidbody r = GameObject.Instantiate(nextStagePrefab, transform.position, transform.rotation).GetComponentInChildren<Rigidbody>();
        r.AddForce(GetChopDirection() * nextStageImpulse);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        _collider = GetComponentInChildren<Collider>();
        chopPoints = new List<ChopPoint>();
        randomAudioClip = GetComponent<RandomAudioClip>();
    }

    public void OnHit(Vector3 hitPosition, Vector3 hitForce)
    {
        randomAudioClip.PlayRandomClip(chopSFX);
        chopPoints.Add(new ChopPoint(hitPosition, hitForce));
        Cut(4.0f);
    }

    private void OnDrawGizmos()
    {
        if(chopPoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < chopPoints.Count; i++)
            {
                Vector3 dir = chopPoints[i].point - transform.position;
                dir.y = 0;
                dir.Normalize();
                Gizmos.DrawRay(chopPoints[i].point, dir);
            }
        }
    }

    public void OnBounce(Vector3 hitPosition, Vector3 hitForce)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// Calculate average of choppoints, to determine where the tree should fall to
    /// This should be replaced by proper logging behaviour! Just for fun! :D
    /// </summary>
    public Vector3 GetChopDirection()
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < chopPoints.Count; i++)
        {
            Vector3 dir = chopPoints[i].point - transform.position;
            dir.y = 0;
            dir.Normalize();
            sum += dir;
        }
        sum /= chopPoints.Count;
        return sum.normalized;
    }
}
