using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Windows;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    public class PlayerControllerExtras : MonoBehaviour {
        [Header("Obstacle Detection")]
        [Tooltip("Distance from the player that an 'obstacle' must be to be interacted with. Used for crouching and mantling.")]
        [SerializeField] float obstacleDetectionDistance = 1.5f;
        float modifiedCrouchHeight, presetCrouchHeight;
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
            presetCrouchHeight = goldPlayerController.Movement.CrouchHeight;
            modifiedCrouchHeight = goldPlayerController.Movement.CrouchHeight;
            cameraMovement = GetComponentInChildren<CameraMovement>();
            if (cameraMovement == null) {
                Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
            }
        }
        private void OnDestroy() {
        }

        // Update is called once per frame
        void LateUpdate() {
            goldPlayerController.Controller.center = new Vector3(0, goldPlayerController.Controller.height / 2, 0);

            if (goldPlayerController.Movement.IsCrouching) {
                StartCoroutine(CheckCrouch(0.25f));
            }
        }

        public void OnCrouch() {
            if (DetectObstacles()) {
                goldPlayerController.Movement.CrouchHeight = FindCrouchHeight();
                Debug.Log($"CrouchHeight set to {goldPlayerController.Movement.CrouchHeight}");
            }


        }

        bool DetectObstacles() {
            RaycastHit hit;
            if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + goldPlayerController.Controller.height, transform.position.z), 1, transform.forward, out hit, obstacleDetectionDistance)) {
                //Debug.Log($"Obstacle detected: {hit.collider.gameObject.name}");
                return true;

            }
            return false;
        }
        float FindCrouchHeight() {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + goldPlayerController.Controller.height / 2, transform.position.z), transform.forward + Vector3.down / 2, out RaycastHit groundHit)) {
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + goldPlayerController.Controller.height / 2, transform.position.z), transform.forward + Vector3.down / 2, Color.red);
                if (Physics.Raycast(groundHit.point, Vector3.up, out RaycastHit ceilingHit, presetCrouchHeight)) {
                    Debug.DrawRay(groundHit.point, Vector3.up, Color.green);
                    Debug.Log($"Height should be set to {Vector3.Distance(groundHit.point, ceilingHit.point)}.");
                    var height = Vector3.Distance(groundHit.point, ceilingHit.point) - 0.35f;
                    goldPlayerController.Movement.crouchCameraPosition = goldPlayerController.Camera.CameraHead.localPosition.y - (goldPlayerController.Controller.height - goldPlayerController.Movement.CrouchHeight);
                    if (height > 0.80f) return height;
                    return 0.80f;
                }
                Debug.Log($"No Ceiling: Height should be set to {presetCrouchHeight}.");
                return presetCrouchHeight;
            }
            return presetCrouchHeight;
        }

        void RecalculateCameraCrouchHeight() {

        }

        private IEnumerator CheckCrouch(float value) {
            yield return new WaitForSeconds(value);
            FindCrouchHeight();
        }
    }
}
