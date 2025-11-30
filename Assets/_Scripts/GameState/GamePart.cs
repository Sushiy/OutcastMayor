using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OutcastMayor.GameState
{
    public class GamePartBehaviour : MonoBehaviour
    {
        [SerializeField]
        UnityEvent OnGamePartStart;

        public void StartGamePart()
        {
            OnGamePartStart?.Invoke();            
        }
    }
    
}
