using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {
        [Header("Camera Controller")]
        [SerializeField]
        [Range(0f, 1f)]
        float ropeBobAmount = 0.5f;

        private void LateUpdate() {
        }

        public void RopeBob() {
            Vector3 start = transform.position;

        }
    }
}
