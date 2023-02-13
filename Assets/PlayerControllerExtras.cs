using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.PlayerLoop;
using UnityEngine.Windows;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    public class PlayerControllerExtras : MonoBehaviour {
        [Header("Obstacle Detection")]
        [Tooltip("Distance from the player that an 'obstacle' must be to be interacted with. Used for crouching and mantling.")]
        [SerializeField] 
        float /*obstacleDetectionDistance = 1.5f,*/ crouchHeightDetectionDistance = 1.0f;
        float crouchMax, crouchOffset = .25f, currentCrouchHeight, crouchTime;
        GoldPlayerController goldPlayerController; //#Critical: required for player movement.
        CameraMovement cameraMovement;

        Vector2 playerMovement;
        // Start is called before the first frame update
        void Awake() {
            goldPlayerController = GetComponent<GoldPlayerController>();
            if (goldPlayerController == null) {
                Debug.LogError($"No GoldPlayerController component found for {gameObject.name}{this}.\nThis is required for player control.");
                return;
            }
            crouchMax = goldPlayerController.Movement.CrouchHeight;
            currentCrouchHeight = crouchMax;
            crouchTime = goldPlayerController.Movement.CrouchTime + 5;
            cameraMovement = GetComponentInChildren<CameraMovement>();
            if (cameraMovement == null) {
                Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
                return;
            }
            cameraMovement.Initialize(goldPlayerController);
        }

        // Update is called once per frame
        void LateUpdate() {
            goldPlayerController.Controller.center = new Vector3(0, goldPlayerController.Controller.height / 2, 0);

            goldPlayerController.Movement.CrouchHeight = Mathf.MoveTowards(goldPlayerController.Movement.CrouchHeight, currentCrouchHeight, crouchTime * Time.deltaTime);

            if (goldPlayerController.Movement.IsCrouching) {
                cameraMovement.Crouched(goldPlayerController.Movement.CrouchHeight);
                StartCoroutine(CheckCrouch(1f));
            }
        }

        public void OnCrouch() {
            
            currentCrouchHeight = FindCrouchHeight();
            //goldPlayerController.Movement.CrouchHeight = currentCrouchHeight;
            Debug.Log($"CrouchHeight set to {currentCrouchHeight}");
        }

        /*bool DetectObstacles() {
            RaycastHit hit;
            if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + goldPlayerController.Controller.height, transform.position.z), 1, transform.forward, out hit, obstacleDetectionDistance)) {
                //Debug.Log($"Obstacle detected: {hit.collider.gameObject.name}");
                return true;

            }
            return false;
        }*/

        float FindCrouchHeight() {
            Vector3 heightCheckPosition = transform.localPosition + transform.forward * crouchHeightDetectionDistance;
            Debug.DrawRay(heightCheckPosition, Vector3.up *1.4f, Color.green, .1f);
            float playerCeiling = CheckCeiling();
            if(playerCeiling != currentCrouchHeight) {
                Debug.Log($"Ceiling height changed. {playerCeiling} != {currentCrouchHeight}");
                if (Physics.Raycast(heightCheckPosition, Vector3.up, out RaycastHit ceilingHit, crouchMax)) {
                    var height = Vector3.Distance(heightCheckPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                    Debug.Log($"New height({height}) found.");

                    if (height > currentCrouchHeight) {
                        Debug.Log("Projected height greater than current crouch height.");
                        if (height > playerCeiling) {
                            Debug.Log("Height greater than current ceiling. Maintain height.");
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

        private IEnumerator CheckCrouch(float value) {
            yield return new WaitForSecondsRealtime(value);
            OnCrouch();
        }
    }
}
