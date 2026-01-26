using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

namespace OutcastMayor.Interaction
{
    public class MinigameTree : Interactable
    {
        public float maxHealth = 10.0f;
        private float currentHealth;

        public GameObject nextStagePrefab;
        public float nextStageImpulse = 100.0f;

        private Collider _collider;


        [SerializeField]
        private AudioClip[] chopSFX;
        [SerializeField]
        private AudioClip breakSFX;

        RandomAudioClip randomAudioClip;

        [Header("Minigame References")]
        [SerializeField]
        private CinemachineCamera minigameCamera;
        [SerializeField]
        private Transform teleportTarget;
        [SerializeField]
        private GameObject minigameUI;

        [SerializeField]
        private Transform aimReticle;

        [SerializeField]
        private Transform targetDisc;
        [SerializeField]
        InputActionReference inputActionReference;

        [Header("Minigame")]
        [SerializeField]
        private bool minigameActive = false;
        /// <summary>
        /// In Degrees
        /// </summary>
        [SerializeField]
        private float cursorAngle;
        private bool pauseForAnimation = false;
        [SerializeField]
        private float cursorSpeed = 1;
        private bool goingUp = true;

        [SerializeField]
        private float targetAngle = 45;
        [SerializeField]
        private float hitdelay = 0.3f;

        Character interactingCharacter;

        public struct ChopPoint
        {
            public Vector3 point;
            public Vector3 direction;

            public ChopPoint(Vector3 point, Vector3 direction)
            {
                this.point = point;
                this.direction = direction;
            }
        }

        public override void Interact(Interactor interactor)
        {
            interactingCharacter = interactor.parentCharacter;
            if (interactingCharacter is Player)
            {
                inputActionReference.action.performed += OnMinigameClick;
                minigameActive = true;
                interactingCharacter.Movement.LockMovement(true);
                minigameCamera.Priority = 10;
                interactingCharacter.Movement.TeleportTo(teleportTarget.position);
                interactingCharacter.Movement.SnapYRotation(teleportTarget.rotation);
                targetDisc.GetChild(0).localScale = new Vector3(0.1f, 1f, 1f);
                cursorAngle = -85;
                goingUp = true;
            }
        }

        void Update()
        {
            if (minigameActive && !pauseForAnimation)
            {
                cursorAngle += cursorSpeed * (goingUp ? 1 : -1) * Time.deltaTime;
                if (cursorAngle >= 85)
                {
                    goingUp = false;
                    cursorAngle = 85;
                }
                else if (cursorAngle <= -85)
                {
                    goingUp = true;
                    cursorAngle = -85;
                }
                aimReticle.localEulerAngles = new Vector3(0, 0, cursorAngle);
                targetDisc.localEulerAngles = new Vector3(0, 0, targetAngle);
            }
        }

        void OnMinigameClick(InputAction.CallbackContext ctx)
        {
            if(!pauseForAnimation)
                StartCoroutine(HitDelay());        
        }

        IEnumerator HitDelay()
        {
            pauseForAnimation = true;
            yield return new WaitForSeconds(hitdelay);

            float diff = Mathf.Abs(cursorAngle - targetAngle);
            print(diff);
            if (diff <= 8)
            {
                Cut(4.0f);
            }
            else if (diff <= 17)
            {
                Cut(2.0f);
            }
            else
            {
                Cut(1.0f);
            }
            if (currentHealth <= maxHealth / 2)
            {
                targetAngle = -45;
                cursorAngle = 85;
                goingUp = false;
            }
            else
            {
                targetAngle = 45;
                cursorAngle = -85;
                goingUp = true;
            }
            //targetAngle = cursorAngle;
            pauseForAnimation = false;
        }

        public void Cut(float damage)
        {
            randomAudioClip.PlayRandomClip(chopSFX);
            currentHealth -= damage;
            float t = 1.0f-(currentHealth / maxHealth);
            targetDisc.GetChild(0).localScale = new Vector3(t, 1, 1);
            if (currentHealth <= 0.0f)
            {
                randomAudioClip.PlayClip(breakSFX);
                NextStage();
            }            
        }

        public void NextStage()
        {
            _collider.enabled = false;
            Rigidbody r = GameObject.Instantiate(nextStagePrefab, transform.position, transform.rotation).GetComponentInChildren<Rigidbody>();
            r.AddForceAtPosition(GetChopDirection() * nextStageImpulse, transform.position + transform.up * 8.0f, ForceMode.Impulse);
            Destroy(gameObject);

            if (interactingCharacter is Player)
            {
                inputActionReference.action.performed -= OnMinigameClick;
                minigameActive = false;
                interactingCharacter.Movement.LockMovement(false);
                minigameCamera.Priority = 0;                 
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            currentHealth = maxHealth;
            _collider = GetComponentInChildren<Collider>();
            randomAudioClip = GetComponent<RandomAudioClip>();
        }

        private void OnDrawGizmos()
        {
            
        }

        public void OnBounce(Vector3 hitPosition, Vector3 hitForce)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetChopDirection()
        {
            return minigameUI.transform.right;
        }
    }

}
