using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Controller")]
    [SerializeField] float obstacleDetectionDistance = 1.5f;
    bool obstacleFound = false;
    Transform playerTransform;
    public void SetPlayerTransform(Transform _playerTransform) {
        playerTransform = _playerTransform;
    }
    private void LateUpdate() {
        DetectObstacle();
    }
    private void DetectObstacle() {
        if(playerTransform == null) {
            Debug.LogWarning($"{this.gameObject.name}: {this} has no playerTransform to use for obstacle detection.");
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z), playerTransform.forward, out hit, obstacleDetectionDistance)) {
            obstacleFound = true;
            Debug.Log($"Obstacle detected: {hit.collider.gameObject.name}");
        }
        else {
            obstacleFound = false;
        }
        
    }
}
