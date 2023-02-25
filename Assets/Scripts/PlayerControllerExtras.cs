using Hertzole.GoldPlayer;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    [RequireComponent(typeof(CameraMovement))]
    public class PlayerControllerExtras : MonoBehaviour {
        //Editor input variables
        [SerializeField]
        [Tooltip("The distance away a surface must be before it's able to be mantled.")]
        float mantleDistance = 1.5f;
        [SerializeField]
        [Tooltip("The maximum height from the player's feet a surface can be before it can't be mantled.")]
        float mantleHeight = 2.5f;
        [SerializeField]
        [Tooltip("Determines how far away the player reacts to changes in ceiling height while crouching.")]
        float crouchHeightDetectionDistance = 1.0f;
        [SerializeField]
        [Tooltip("The time is takes until the players falling state changes to long fall.")]
        float timeUntilLongFall = 1f;
        [SerializeField]
        [Tooltip("The amount of time 'isHardLanding' is true after landing on the groun.")]
        float hardLandingTime = 0.25f;

        //Automated variables
        //#Critical: required for player movement.
        [HideInInspector]
        public GoldPlayerController goldPlayerController; 
        float crouchOffset = .25f, crouchMax, currentCrouchHeight, crouchTime;
        CameraMovement cameraMovement;
        Vector3 goToPosition = Vector3.zero;
        [HideInInspector]
        public bool hardLanding = false,
            isMantling = false,
            isLongFalling = false,
            isCrouching = false;

        bool fallCheckStarted = false;
        Coroutine longFallTimer;


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


        private void MantleControl() {
            if (isMantling) {
                //goldPlayerController.Movement.CanMoveAround = false;
                var stickValue = goldPlayerController.Movement.GroundStick;
                goldPlayerController.Movement.GroundStick = 100;
                Vector3 movement = Vector3.Slerp(transform.position, goToPosition, 6 * Time.deltaTime);
                movement.y = Mathf.Lerp(transform.position.y, goToPosition.y, 20 * Time.deltaTime);
                StopCoroutine(longFallTimer);
                isLongFalling = false;
                hardLanding = false;

                goldPlayerController.SetPosition(movement);
                if (Vector3.Distance(transform.position, goToPosition) <= 0.3f) {
                    isMantling = false;
                    cameraMovement.ReturnToZero();
                    goldPlayerController.Movement.GroundStick = stickValue;
                }
                return;
            }
            else {
                    goldPlayerController.Movement.CanMoveAround = true;
                    goToPosition = Vector3.zero;
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
        //The jump button is pressed
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
                isMantling = true;
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
