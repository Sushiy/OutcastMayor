using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player Instance;

    //PLAYER SPECIFIC
    private RequestLog questLog;
    public RequestLog QuestLog => questLog;

    private BuildingMode buildingMode;
    public BuildingMode BuildingMode => buildingMode;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are two Players");
            Destroy(this);
        }

        questLog = GetComponent<RequestLog>();
        buildingMode = GetComponent<BuildingMode>();

        base.Awake();
    }
}
