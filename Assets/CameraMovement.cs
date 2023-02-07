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
        Transform head;
        GoldPlayerController goldPlayerController;
        float crouchLerp;
        float crouchHeight;

        public void Initialize(GoldPlayerController controller) {
            goldPlayerController = controller;
            crouchLerp = goldPlayerController.Movement.CrouchTime;
            crouchHeight = goldPlayerController.Movement.CrouchHeight;
        }

        private void LateUpdate() {
            if (goldPlayerController.Movement.IsCrouching) {
                //adjust head position to match the appropriate height.
            }
        }

        public void RopeBob() {
            //camera movement when landing on a rope
        }

        public void Crouch(float _crouchHeight) {
            crouchHeight = _crouchHeight;
        }
    }
}
