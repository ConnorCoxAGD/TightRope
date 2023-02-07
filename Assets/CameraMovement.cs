using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {
        Transform head; //This is the parent of the camera head. This allows us to move the head alongside the Gold Players movement.
        GoldPlayerController goldPlayerController;
        float crouchTime;
        float defaultCrouchHeight;
        float startingHeight;
        float crouchHeight;

        public void Initialize(GoldPlayerController controller) {
            goldPlayerController = controller;
            crouchTime = goldPlayerController.Movement.CrouchTime * 17;
            defaultCrouchHeight = goldPlayerController.Movement.CrouchHeight;
            //Programatically setup the Head Parent
            head = new GameObject("HeadParent").transform;
            head.SetParent(goldPlayerController.gameObject.transform);
            head.position = goldPlayerController.Camera.CameraHead.position;
            goldPlayerController.Camera.CameraHead.SetParent(head);
            startingHeight = head.localPosition.y;
        }

        private void LateUpdate() {
            if (goldPlayerController.Movement.IsCrouching) {
                
                var newPos = new Vector3(head.transform.localPosition.x, defaultCrouchHeight - (defaultCrouchHeight - crouchHeight), head.transform.localPosition.z);
                head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, newPos, crouchTime * Time.deltaTime);
            }
            else if (head.transform.localPosition.y < startingHeight) {
                var newPos = new Vector3(head.transform.localPosition.x, startingHeight, head.transform.localPosition.z);
                head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, newPos, crouchTime * Time.deltaTime);
            }
        }

        public void Crouched(float inputCrouchHeight) {
            crouchHeight = inputCrouchHeight;
        }

        private void OnDrawGizmos() {
            if (head == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(head.position, .5f);
        }
    }
}
