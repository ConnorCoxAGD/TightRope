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
        float crouchMax, crouchOffset = .25f;
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

            if (goldPlayerController.Movement.IsCrouching) {
                cameraMovement.Crouched(goldPlayerController.Movement.CrouchHeight);
                StartCoroutine(CheckCrouch(0.75f));
            }
        }

        public void OnCrouch() {
            goldPlayerController.Movement.CrouchHeight = FindCrouchHeight();
            Debug.Log($"CrouchHeight set to {goldPlayerController.Movement.CrouchHeight}");
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
            Debug.DrawRay(heightCheckPosition, Vector3.up *1.4f, Color.green, .5f);
            if (Physics.Raycast(heightCheckPosition, Vector3.up, out RaycastHit ceilingHit, crouchMax)) {
                var height = Vector3.Distance(heightCheckPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                //Debug.Log($"Height should be set to {height}.");
                float playerCeiling = goldPlayerController.Movement.CrouchHeight;
                if(height > goldPlayerController.Movement.CrouchHeight) {
                    playerCeiling = CheckCeiling();
                    if (height > playerCeiling) {
                        return playerCeiling;
                    }
                }
                if (height > 0.80f) return height;
                return 0.80f;
            }
            return crouchMax;
        }
        float CheckCeiling() {
            LayerMask layerMask = 1<<6; //finally understand how layermasks work as a bitmask
            Debug.DrawRay(transform.localPosition, Vector3.up*3, Color.magenta, 2f);
            if (Physics.Raycast(transform.localPosition, Vector3.up, out RaycastHit ceilingHit, 5, ~layerMask)){
                float dist = Vector3.Distance(transform.localPosition, ceilingHit.point) - goldPlayerController.Controller.skinWidth - crouchOffset;
                Debug.Log($"Ceiling height is {dist} on object {ceilingHit.collider.name}");
                return dist;
            }
            Debug.Log("No Ceiling");
            return goldPlayerController.Movement.CrouchHeight;
        }

        private IEnumerator CheckCrouch(float value) {
            yield return new WaitForSecondsRealtime(value);
            goldPlayerController.Movement.CrouchHeight = FindCrouchHeight();
        }
    }
}
