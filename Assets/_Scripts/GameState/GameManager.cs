using System.Collections;
using System.Collections.Generic;
using OutcastMayor.GameState;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [SerializeField]
    int currentPartID;
    [SerializeField]
    GamePartBehaviour[] gameParts;

    GamePartBehaviour currentPart;

    public System.Action<GamePartBehaviour> BeforePartStarted;
    public System.Action<GamePartBehaviour> AfterPartStarted;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }   
    }

    void Start()
    {
        StartPart(currentPartID, true);
    }

    public void NextPart()
    {
        StartPart(currentPartID+1);
    }

    public void StartPart(int i, bool sceneStartsHere = false)
    {
        currentPartID = i;
        currentPart = gameParts[i];

        BeforePartStarted?.Invoke(currentPart);
        currentPart.StartGamePart(sceneStartsHere);  
        AfterPartStarted?.Invoke(currentPart);
    }

    public void StartPart(GamePartBehaviour partBehaviour)
    {
        for(int i = 0; i < gameParts.Length; i++)
        {
            if(gameParts[i] == partBehaviour)
            {
                StartPart(i);
                break;
            }
        }
        Debug.LogWarning($"Could not find part: {partBehaviour.PartName}");
    }
}
