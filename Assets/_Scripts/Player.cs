using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private RequestLog questLog;
    public RequestLog QuestLog => questLog;
    private Inventory inventory;
    public Inventory Inventory => inventory;
    private BuildingMode buildingMode;
    public BuildingMode BuildingMode => buildingMode;
    private Interactor interactor;
    public Interactor Interactor => interactor;

    public bool isSleeping = false;

    private void Awake()
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
        inventory = GetComponent<Inventory>();
        buildingMode = GetComponent<BuildingMode>();
        interactor = GetComponent<Interactor>();
    }
}
