using OutcastMayor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This Script is just to control a basic NPC. I don't want to deal with anything else right now.
/// </summary>
public class NPC : Character
{
    [SerializeField]
    NPC_Data npcData;
    public NPC_Data NPCData => npcData;

    NavMeshMovement navhMeshMovement;

    public string CharacterName => npcData.CharacterName;

    protected override void Awake()
    {
        base.Awake();
        navhMeshMovement = GetComponent<NavMeshMovement>();
    }

    void Start()
    {        
        NPCManager.AddNPC(this);
    }

    public void GoTo(Transform _transform)
    {
        GoTo(_transform.position);
    }

    public void GoTo(Vector3 _targetPosition)
    {
        navhMeshMovement.TryMoveTo(_targetPosition, false, null);
    }
}
