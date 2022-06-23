using OutcastMayor.Building;
using OutcastMayor.Requests;
using OutcastMayor.Zoning;
using UnityEngine;

namespace OutcastMayor
{
    public class Player : Character
    {
        public static Player Instance;

        //PLAYER SPECIFIC
        private RequestLog questLog;
        public RequestLog QuestLog => questLog;

        private BuildingMode buildingMode;
        public BuildingMode BuildingMode => buildingMode;

        private PlayerToolManager playerToolManager;
        public PlayerToolManager PlayerToolManager => playerToolManager;

        [SerializeField]
        private ZoningMode zoningMode;
        public ZoningMode ZoningMode => zoningMode;

        private State DefaultState;
        private State InteractingState;
        private State BuildingState;
        private State ZoningState;
        private State PausedState;

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
            playerToolManager = GetComponent<PlayerToolManager>();


            //Setup states
            DefaultState = new State(null, null, null, this, "Default State");
            InteractingState = new State(null, null, null, this, "Interacting State");
            BuildingState = new State(null, null, null, this, "Building State");
            ZoningState = new State(null, null, null, this, "Zoning State");
            PausedState = new State(null, null, null, this, "Paused State");

            currentState = DefaultState;

            base.Awake();
        }

        public override void Sleep()
        {
            base.Sleep();
        }
    }
}
