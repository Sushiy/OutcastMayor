using System.Collections;
using System.Collections.Generic;
using OutcastMayor.GameState;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GamePartBehaviour startingPart;

    void Start()
    {
        startingPart.StartGamePart();
    }
}
