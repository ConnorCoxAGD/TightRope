using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Add-on component that allows creation of advanced controls for the Gold Player Character Controller.
    /// </summary>
    public class PlayerControllerExtras : MonoBehaviour {
        [Header("Obstacle Detection")]
        [Tooltip("Distance from the player that an 'obstacle' must be to be interacted with. Used for crouching and mantling.")]
        [SerializeField] float obstacleDetectionDistance = 1.5f;
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
            cameraMovement = GetComponentInChildren<CameraMovement>();
            if (cameraMovement == null) {
                Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
            }
        }

        // Update is called once per frame
        void Update() {
            DetectObstacles();
        }

        bool DetectObstacles() {
            RaycastHit hit;
            if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y, transform.position.z), 2, transform.forward, out hit, obstacleDetectionDistance)) {
                Debug.Log($"Obstacle detected: {hit.collider.gameObject.name}");
                return true;

            }
            return false;
        }
    }
}
