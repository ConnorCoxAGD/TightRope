using Hertzole.GoldPlayer;
using System.Collections;
using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {

    public enum MovementStates {
        Grounded,
        Mantling,
        Crouching,
        Jumping,
        Falling,
        LongFalling,
        Landing
    }

    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    [RequireComponent(typeof(CameraMovement))]
    public class PlayerControllerExtras : MonoBehaviour {
        //Editor input variables
        public MovementStates movementState;
        [SerializeField]
        [Tooltip("")]
        float mantleDistance = 1.5f;
        [SerializeField]
        [Tooltip("")]
        float mantleHeight = 2.5f;
        [SerializeField]
        [Tooltip("")]
        float crouchHeightDetectionDistance = 1.0f;
        //Automated variables
        //#Critical: required for player movement.
        [HideInInspector]
        public GoldPlayerController goldPlayerController; 
        float crouchOffset = .25f, crouchMax, currentCrouchHeight, crouchTime;
        CameraMovement cameraMovement;
        Vector3 goToPosition = Vector3.zero;

        bool hardLanding = false;


        void Awake() {
            goldPlayerController = GetComponent<GoldPlayerController>();
            if (goldPlayerController == null) {
                Debug.LogError($"No GoldPlayerController component found for {gameObject.name}{this}.\nThis is required for player control.");
                return;
            }
            crouchMax = goldPlayerController.Movement.CrouchHeight;
            currentCrouchHeight = crouchMax;
            crouchTime = goldPlayerController.Movement.CrouchTime + 5;
            cameraMovement = GetComponent<CameraMovement>();
            if (cameraMovement == null) {
                Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
                return;
            }
            cameraMovement.Initialize(this);
        }

        // Update is called once per frame
        void LateUpdate() {
            goldPlayerController.Controller.center = new Vector3(0, goldPlayerController.Controller.height / 2, 0);

            goldPlayerController.Movement.CrouchHeight = Mathf.MoveTowards(goldPlayerController.Movement.CrouchHeight, currentCrouchHeight, crouchTime * Time.deltaTime);

            if (goldPlayerController.Movement.IsCrouching) {
                cameraMovement.Crouched(goldPlayerController.Movement.CrouchHeight);
                OnCrouch();
            }
        }
        private void Update() {
            ControlMovement();
        }

        private void ControlMovement() {
            switch (movementState) {
                case MovementStates.Mantling:
                    //goldPlayerController.Movement.CanMoveAround = false;
                    var stickValue = goldPlayerController.Movement.GroundStick;
                    goldPlayerController.Movement.GroundStick = 100;
                    Vector3 movement = Vector3.Slerp(transform.position, goToPosition, 6 * Time.deltaTime);
                    movement.y = Mathf.Lerp(transform.position.y, goToPosition.y, 20 * Time.deltaTime);

                    goldPlayerController.SetPosition(movement);
                    if (Vector3.Distance(transform.position, goToPosition) <= 0.3f) {
                        movementState = MovementStates.Grounded;
                        cameraMovement.ReturnToZero();
                        goldPlayerController.Movement.GroundStick = stickValue;

                    }
                    break;
                case MovementStates.Grounded:
                    goldPlayerController.Movement.CanMoveAround = true;
                    goToPosition = Vector3.zero;
                    break;

            }
        }

        #region Crouching

        public void OnCrouch() {
            
            currentCrouchHeight = FindCrouchHeight();
            //goldPlayerController.Movement.CrouchHeight = currentCrouchHeight;
            //Debug.Log($"CrouchHeight set to {currentCrouchHeight}");
        }

        float FindCrouchHeight() {
            Vector3 heightCheckPosition = transform.localPosition + transform.forward * crouchHeightDetectionDistance;
            Debug.DrawRay(heightCheckPosition, Vector3.up *1.4f, Color.green);
            float playerCeiling = CheckCeiling();
            if(playerCeiling != currentCrouchHeight) {
                //Debug.Log($"Ceiling height changed. {playerCeiling} != {currentCrouchHeight}");
                if (Physics.Raycast(heightCheckPosition, Vector3.up, out RaycastHit ceilingHit, crouchMax)) {
                    var height = Vector3.Distance(heightCheckPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                    //Debug.Log($"New height({height}) found.");

                    if (height > currentCrouchHeight) {
                        //Debug.Log("Projected height greater than current crouch height.");
                        if (height > playerCeiling) {
                            //Debug.Log("Height greater than current ceiling. Maintain height.");
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
            LayerMask layerMask = 1<<6; //finally understand how layermasks work as a bitmask
            //This can probably be made to be modifiable in the inspector given a little extra time in polish phase
            Debug.DrawRay(transform.localPosition, Vector3.up*3, Color.magenta, 1f);
            if (Physics.Raycast(transform.localPosition, Vector3.up, out RaycastHit ceilingHit, 5, ~layerMask)){
                float dist = Vector3.Distance(transform.localPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                Debug.Log($"Ceiling found: {dist}");
                return dist;
            }
            Debug.Log("No Ceiling");
            return currentCrouchHeight;
        }

        #endregion

        #region Mantling

        void OnJump() {
            if (!goldPlayerController.Movement.IsCrouching) {
                if (!ObstacleCheck()) {
                    MantleCheck();
                }
            }
        }

        bool ObstacleCheck() {
            LayerMask layerMask = 1 << 6;
            Vector3 startPoint = transform.localPosition + (transform.up * mantleHeight);
            if(!Physics.Raycast(startPoint, transform.forward, mantleDistance + 0.5f)) {
                return false;
            }
            return true;
        }

        void MantleCheck() {
            //point a raycast down towards a ground.
            LayerMask layerMask = 1<<6;
            Vector3 startPoint = transform.localPosition + (transform.forward * mantleDistance) + (transform.up * mantleHeight);
            Debug.DrawRay(startPoint, Vector3.down * 1.5f, Color.white, 1f, true);
            if (Physics.SphereCast(startPoint, .5f, Vector3.down, out RaycastHit hit, mantleHeight - 0.5f, ~layerMask, QueryTriggerInteraction.Ignore)) {
                goToPosition = hit.point;
                movementState = MovementStates.Mantling;
                cameraMovement.Mantle();
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(goToPosition, .2f);
        }
#endif


#endregion
    }
}
