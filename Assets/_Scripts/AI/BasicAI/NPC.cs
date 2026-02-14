using OutcastMayor;
using OutcastMayor.GameState;
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
        GameManager.Instance.BeforePartStarted += BeforePartStarted;
    }

    public void GoTo(Transform _transform)
    {
        GoTo(_transform.position);
    }

    public void GoTo(Vector3 _targetPosition)
    {
        navhMeshMovement.TryMoveTo(_targetPosition, false, null);
    }

    void BeforePartStarted(GamePartBehaviour gamePartBehaviour)
    {
        Transform spawnPosition = gamePartBehaviour.GetSpawnLocation(CharacterName);
        if(spawnPosition != null)
        {
            movement.TeleportTo(spawnPosition.position);
            movement.SnapYRotation(spawnPosition.rotation);
        }
    }
}
