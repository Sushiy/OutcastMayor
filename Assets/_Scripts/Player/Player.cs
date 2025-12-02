using OutcastMayor.Building;
using OutcastMayor.Requests;
using OutcastMayor.Zoning;
using UnityEngine;

namespace OutcastMayor
{
    /// <summary>
    /// Player States
    /// </summary>
    public class Player : Character
    {
        public static Player Instance;

        //PLAYER SPECIFIC
        private PlayerInputManager playerInputManager;
        public PlayerInputManager PlayerInputManager => playerInputManager;
        private RequestLog questLog;
        public RequestLog QuestLog => questLog;

        private PlayerToolManager playerToolManager;
        public PlayerToolManager PlayerToolManager => playerToolManager;

        [SerializeField]
        private ZoningMode zoningMode;
        public ZoningMode ZoningMode => zoningMode;

        [SerializeField]
        private BuildingMode buildingMode;
        public BuildingMode BuildingMode => buildingMode;

        public State DefaultState;
        public State InteractingState;
        public State BuildingState;
        public State ZoningState;
        public State PausedState;

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

            playerInputManager = GetComponent<PlayerInputManager>();
            buildingMode = GetComponent<BuildingMode>();

            questLog = GetComponent<RequestLog>();
            playerToolManager = GetComponent<PlayerToolManager>();


            //Setup states
            DefaultState = new State(null, null, null, this, "Default State");
            InteractingState = new State(null, null, null, this, "Interacting State");
            BuildingState = new State(BuildingStateEnter, null, BuildingStateExit, this, "Building State");
            ZoningState = new State(null, null, null, this, "Zoning State");
            PausedState = new State(null, null, null, this, "Paused State");

            currentState = DefaultState;

            base.Awake();
        }

        public void BuildingStateEnter()
        {
            playerInputManager.SetBuildMode(true);
        }
        public void BuildingStateExit(State _nextState)
        {
            playerInputManager.SetBuildMode(false);
        }

        public override void Sleep()
        {
            base.Sleep();
        }
    }
}
