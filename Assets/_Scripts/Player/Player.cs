using OutcastMayor.Building;
using OutcastMayor.Requests;
using OutcastMayor.Zoning;
using UnityEditor.EditorTools;
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

        [SerializeField]
        Tool buildingTool;

        public State DefaultState;
        public State InteractingState;
        public State BuildingState;
        public State DestructionState;
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
            DestructionState = new State(DestructionStateEnter, null, DestructionStateExit, this, "Destruction State");
            ZoningState = new State(null, null, null, this, "Zoning State");
            PausedState = new State(null, null, null, this, "Paused State");

            currentState = DefaultState;

            base.Awake();
        }

        private void BuildingStateEnter()
        {
            playerInputManager.SetBuildMode(true);;
        }
        private void BuildingStateExit(State _nextState)
        {
            playerInputManager.SetBuildMode(false);
        }

        public virtual void EquipBuildTool()
        {
            //if(heldItemSlotID == slotID) return;
            //if we are already holding an item, kick it out?
            if(heldItem != null && heldItemGameObject != null)
            {
                heldItemGameObject.SetActive(false);
                heldItemGameObject = null;
                heldItem = null;
            }

            if(buildingTool != null)
            {
                heldItemGameObject = buildingTool.gameObject;
                heldItemGameObject.SetActive(true);

                heldItemGameObject.transform.parent = toolTransform;
                heldItemGameObject.transform.localPosition = Vector3.zero;
                heldItemGameObject.transform.localRotation = Quaternion.identity;
                heldItemGameObject.transform.localScale = Vector3.one;
                characterAnimation.SetCarryState(0);

                if(OnHeldItemChanged != null)
                {
                    OnHeldItemChanged?.Invoke(heldItemGameObject);
                }
            }
        }

        public void ToggleBuildMode()
        {
            if(currentState == BuildingState)
            {
                HoldItem(heldItemSlotID);
                ChangeState(DefaultState);
            }
            else if(currentState != PausedState)
            {
                EquipBuildTool();
            }
        }
        private void DestructionStateEnter()
        {
            playerInputManager.SetBuildMode(true);;
        }
        private void DestructionStateExit(State _nextState)
        {
            playerInputManager.SetBuildMode(false);
        }

        public override void Sleep()
        {
            base.Sleep();
        }
    }
}
