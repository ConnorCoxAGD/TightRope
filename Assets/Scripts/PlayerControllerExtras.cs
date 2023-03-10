using Hertzole.GoldPlayer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    [RequireComponent(typeof(CameraMovement))]
    public class PlayerControllerExtras : MonoBehaviour {
        #region Serialized Fields
        [SerializeField]
        [Tooltip("Layers that the player is on. You don't want this to be the same as ground or the mantle layer. Ideally, the player is on it's own layer.")]
        LayerMask playerLayer;
        [SerializeField]
        float interactionReach = 2.5f;

        [Header("Mantling")]
        [SerializeField]
        [Tooltip("Layers that can be mantled onto. Usually at least want your ground layer to be able to be mantled.")]
        LayerMask mantleLayers;

        [SerializeField]
        [Tooltip("The distance away a surface must be before it's able to be mantled.")]
        float mantleDistance = 1.5f;

        [SerializeField]
        [Tooltip("The maximum height from the player's feet a surface can be before it can't be mantled.")]
        float mantleHeight = 2.5f;

        [SerializeField]
        [Tooltip("The maximum angle slope the player can mantle onto.")]
        float mantleMaximumAngle = 20;

        [Header("Crouching")]
        [SerializeField]
        [Tooltip("Determines how far away the player reacts to changes in ceiling height while crouching.")]
        float crouchHeightDetectionDistance = 1.0f;

        [Header("Falling and Landing")]
        [SerializeField]
        [Tooltip("The time is takes until the players falling state changes to long fall.")]
        float timeUntilLongFall = 1f;

        [SerializeField]
        [Tooltip("The amount of time 'isHardLanding' is true after landing on the groun.")]
        float hardLandingTime = 0.25f;

        [Header("UI")]
        [SerializeField]
        Text interactionMessageText;
        [SerializeField]
        [Tooltip("The crosshair used for aiming. Tools and interaction are based on a raycast from this image's screen position.")]
        Image crosshair;

        #endregion

        #region Hidden Fields
        [HideInInspector]
        public InventoryComponent inventory = null;

        string interactionMessage = "";

        //Automated variables
        //#Critical: required for player movement.
        [HideInInspector]
        public GoldPlayerController goldPlayerController;

        float crouchOffset = .25f,
            crouchMax,
            currentCrouchHeight,
            crouchTime,
            gravity;

        CameraMovement cameraMovement;

        Vector3 goToPosition = Vector3.zero;

        //special movement states
        [HideInInspector]
        public bool hardLanding = false,
            isMantling = false,
            isLongFalling = false,
            isCrouching = false;

        bool fallCheckStarted = false;
        Coroutine longFallTimer;

        InteractableObject interactable = null;
        #endregion

        #region Methods
        #region MonoBehaviours
        void Awake() {
            goldPlayerController = GetComponent<GoldPlayerController>();
            if (goldPlayerController == null) {
                Debug.LogError($"No GoldPlayerController component found for {gameObject.name}{this}.\nThis is required for player control.");
                return;
            }
            crouchMax = goldPlayerController.Movement.CrouchHeight;
            currentCrouchHeight = crouchMax;
            crouchTime = goldPlayerController.Movement.CrouchTime + 5;
            gravity = goldPlayerController.Movement.Gravity;
            cameraMovement = GetComponent<CameraMovement>();
            if (cameraMovement == null) {
                Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
                return;
            }
            cameraMovement.Initialize(this);
            inventory = GetComponent<InventoryComponent>();
            if (inventory == null) {
                Debug.LogWarning($"{this} No inventory component. Items will be unavailable.");
            }
            if (crosshair == null) Debug.LogWarning($"{this}: Crosshair variable is null.\n Player aim will default to a cast from the center of the screen, instead of from a designated crosshair.");
            ClearMessage();
        }

        // On Late update because regular update intereferes with GoldPlayer. Ideally this can be fixed eventually.
        void LateUpdate() {
            goldPlayerController.Controller.center = new Vector3(0, goldPlayerController.Controller.height / 2, 0);
            goldPlayerController.Movement.CrouchHeight = Mathf.MoveTowards(goldPlayerController.Movement.CrouchHeight, currentCrouchHeight, crouchTime * Time.deltaTime);

            if (goldPlayerController.Movement.IsCrouching) {
                cameraMovement.Crouched(goldPlayerController.Movement.CrouchHeight);
                isCrouching = true;
                OnCrouch();
            }
            else {
                isCrouching = false;
            }
            //on late Update because it involves the camera.
            CastInteractionRay();
        }
        private void Update() {
            MantleControl();
            if (!goldPlayerController.Movement.IsGrounded) {
                CheckFall();
            }
            else {
                StopCoroutine(longFallTimer);
                if (isLongFalling) {
                    HardLanding();
                }
                fallCheckStarted = false;
            }
        }

        #endregion
        #region Interactions
        /*
        private void OnTriggerEnter(Collider other) {
            var obj = other.GetComponentInParent<InteractableObject>();
            if (obj == null) return;
            interactable = obj;
            interactable.InteractionAreaEntered(this, other);

        }
        private void OnTriggerExit(Collider other) {
            var obj = other.GetComponentInParent<InteractableObject>();
            if (obj == null) return;
            if (obj == interactable) {
                interactable.InteractionAreaExited(this, other);
                interactable = null;
            }
        }*/
        private void CastInteractionRay() {
            if (crosshair == null) return;
            RaycastHit hit;
            Vector3 start = Camera.main.ScreenToWorldPoint(crosshair.transform.position);
            Debug.DrawRay(start, Camera.main.transform.forward * interactionReach, Color.cyan);
            if (Physics.Raycast(start, Camera.main.transform.forward, out hit, interactionReach)) {
                var prevInteractable = interactable;
                if (!hit.transform.root.gameObject.TryGetComponent(out interactable) && !hit.transform.TryGetComponent(out interactable)) {
                    if(prevInteractable != interactable) Debug.Log($"Interactable object not found.");
                    ClearInteraction();
                    return;
                }
                if (interactable == prevInteractable) {
                    return;
                }
                Debug.Log($"Interact with {interactable.name}");
                interactable.PrepInteraction(this);
                return;
            }
            ClearInteraction();
            return;
        }
        public void ClearInteraction() {
            if(interactable == null) {
                ClearMessage();
                return;
            }
            interactable = null;
            ClearMessage();
        }
        public void OnInteract() {
            if (interactable == null) return;
            interactable.Interact(this);

        }
        public void InteractionMessage(string message) {
            interactionMessage = message; //may be redundant, but it was helpful for starting and may be helpful later.
            interactionMessageText.text = message;
            Debug.Log($"Recieved message: {interactionMessage}");
            //additional code to display a message.
            //we may also create a additional script to work with ui instead.
            //we may also be able to tie unity events to this to make it easy to drag, drop, and modify.
        }
        public void InteractionMessage(string message, float time) {
            InteractionMessage(message);
            StartCoroutine(TimedMessage(time));
        }
        public IEnumerator TimedMessage(float seconds) {
            yield return new WaitForSeconds(seconds);
            ClearMessage();
        }
        public void ClearMessage() {
            if (interactionMessage == string.Empty && interactionMessageText.text == string.Empty) return;
            interactionMessage = string.Empty;
            interactionMessageText.text = string.Empty;
            Debug.Log("Cleared messages.");
            //turn off the UI responsible for displaying this message.
        }



        #endregion
        #region Movement

        #region Falling and Landing
        private void HardLanding() {
            hardLanding = true;
            isLongFalling = false;
            StartCoroutine(HardLandingTimer());
            cameraMovement.HardLanding();
        }

        private void CheckFall() {
            if (fallCheckStarted) return;
            fallCheckStarted = true;
            longFallTimer = StartCoroutine(LongFallTimer());
        }

        private IEnumerator LongFallTimer() {
            yield return new WaitForSeconds(timeUntilLongFall);
            if (goldPlayerController.Movement.IsGrounded) yield break;
            isLongFalling = true;
        }

        private IEnumerator HardLandingTimer() {
            yield return new WaitForSecondsRealtime(hardLandingTime);
            hardLanding = false;
        }
        #endregion

        #region Crouching

        public void OnCrouch() {
            currentCrouchHeight = FindCrouchHeight();
        }

        float FindCrouchHeight() {
            Vector3 heightCheckPosition = transform.localPosition + transform.forward * crouchHeightDetectionDistance;
            Debug.DrawRay(heightCheckPosition, Vector3.up * 1.4f, Color.green);
            float playerCeiling = CheckCeiling();
            if (playerCeiling != currentCrouchHeight) {
                if (Physics.Raycast(heightCheckPosition, Vector3.up, out RaycastHit ceilingHit, crouchMax)) {
                    var height = Vector3.Distance(heightCheckPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                    if (height > currentCrouchHeight) {
                        if (height > playerCeiling) {
                            return playerCeiling;
                        }
                    }
                    if (height > 0.90f) return height;
                    return currentCrouchHeight;
                }
            }
            else {
                return playerCeiling;
            }
            return crouchMax;
        }
        float CheckCeiling() {
            Debug.DrawRay(transform.localPosition, Vector3.up * 3, Color.magenta, 1f);
            if (Physics.Raycast(transform.localPosition, Vector3.up, out RaycastHit ceilingHit, 5, ~playerLayer)) {
                float dist = Vector3.Distance(transform.localPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                Debug.Log($"Ceiling found: {dist}");
                return dist;
            }
            Debug.Log("No Ceiling");
            return currentCrouchHeight;
        }

        #endregion

        #region Mantling
        //The jump button is pressed
        void OnJump() {
            if (!goldPlayerController.Movement.IsCrouching) {
                if (!ObstacleCheck()) {
                    MantleCheck();
                }
            }
        }

        bool ObstacleCheck() {
            Vector3 startPoint = transform.localPosition + (transform.up * mantleHeight);
            if (!Physics.Raycast(startPoint, transform.forward, mantleDistance + 0.5f)) {
                return false;
            }
            return true;
        }

        void MantleCheck() {
            Vector3 startPoint = transform.localPosition + (transform.forward * mantleDistance) + (transform.up * mantleHeight);
            Debug.DrawRay(startPoint, Vector3.down * 1.5f, Color.white, 1f, true);
            if (Physics.SphereCast(startPoint, .5f, Vector3.down, out RaycastHit hit, mantleHeight - 1f, mantleLayers, QueryTriggerInteraction.Ignore)) {
                var angle = Vector3.Angle(Vector3.up, hit.normal);
                if (Physics.Raycast(hit.point, Vector3.up, goldPlayerController.Controller.height)) return;
                if (angle > mantleMaximumAngle) return;
                goToPosition = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);
                isMantling = true;
                goldPlayerController.Movement.CanJump = false;
                cameraMovement.Mantle();
            }
        }

        private void MantleControl() {
            if (isMantling) {
                goldPlayerController.Movement.Gravity = gravity / 3;
                goldPlayerController.Movement.CanMoveAround = false;
                goldPlayerController.Audio.StopLandSound();
                Vector3 movement;

                movement = Vector3.Lerp(transform.position, goToPosition, 6 * Time.deltaTime);
                movement.y = Mathf.Lerp(transform.position.y, goToPosition.y + 0.2f, 10 * Time.deltaTime);

                StopCoroutine(longFallTimer);
                isLongFalling = false;
                hardLanding = false;

                goldPlayerController.SetPosition(movement);

                if (Vector3.Distance(transform.position, goToPosition) <= 0.15f) {
                    goldPlayerController.Movement.Gravity = gravity;
                    cameraMovement.ReturnToZero();
                    goldPlayerController.Movement.CanMoveAround = true;
                    goToPosition = Vector3.zero;
                    isMantling = false;
                    goldPlayerController.Movement.CanJump = true;
                }
                return;
            }
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(goToPosition, .2f);
        }
#endif


        #endregion
        #endregion
    }
}
