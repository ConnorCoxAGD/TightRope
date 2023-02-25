using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {

        [SerializeField]
        Vector3 mantleOffset = new Vector3(2, 0, 15);
        [SerializeField]
        [Range(1, 65)]
        float mantleCameraSpeed = 5;

        PlayerControllerExtras controllerExtras;
        Transform headParent; //Used to manipualte the 'head' without interfering with the Gold Player Controller.

        float crouchTime;
        float defaultCrouchHeight;
        float startingHeight;
        float crouchHeight;
        Vector3 goToRotation = Vector3.zero;

        public void Initialize(PlayerControllerExtras controller) {
            controllerExtras = controller;
            crouchTime = controllerExtras.goldPlayerController.Movement.CrouchTime * 5;
            defaultCrouchHeight = controllerExtras.goldPlayerController.Movement.CrouchHeight;
            //Programatically setup the Head Parent
            headParent = new GameObject("HeadParent").transform;
            headParent.rotation = Quaternion.identity;
            headParent.SetParent(controllerExtras.goldPlayerController.Camera.CameraHead.parent);
            headParent.position = controllerExtras.goldPlayerController.Camera.CameraHead.position;

            controllerExtras.goldPlayerController.Camera.CameraHead.SetParent(headParent);
            startingHeight = headParent.localPosition.y;
        }

        private void LateUpdate() {
            CameraControl();            
        }

        private void CameraControl() {
            switch (controllerExtras.movementState) {
                case MovementStates.Grounded:
                    if (headParent.localRotation.eulerAngles != Vector3.zero) {
                        Quaternion returnRot = Quaternion.identity;
                        headParent.localRotation = Quaternion.Slerp(headParent.localRotation, returnRot, mantleCameraSpeed * 1.5f * Time.deltaTime);
                    }
                    if (headParent.transform.localPosition.y < startingHeight) {
                        var upPos = new Vector3(headParent.transform.localPosition.x, startingHeight, headParent.transform.localPosition.z);
                        headParent.transform.localPosition = Vector3.MoveTowards(headParent.transform.localPosition, upPos, crouchTime * Time.deltaTime);
                    }
                    break;
                case MovementStates.Crouching:
                    var newPos = new Vector3(headParent.transform.localPosition.x, defaultCrouchHeight - (defaultCrouchHeight - crouchHeight), headParent.transform.localPosition.z);
                    headParent.transform.localPosition = Vector3.MoveTowards(headParent.transform.localPosition, newPos, crouchTime * Time.deltaTime);
                    break;
                case MovementStates.Mantling:
                    Quaternion rot = Quaternion.Euler(goToRotation);
                    headParent.localRotation = Quaternion.Slerp(headParent.localRotation, rot, mantleCameraSpeed * Time.deltaTime);
                    break;
            }
        }

        public void ReturnToZero() {
            goToRotation = Vector3.zero;
        }

        public void Mantle() {
            int decide = Random.Range(0, 2);
            if(decide == 0) {
                goToRotation = mantleOffset;
            }
            else {
                goToRotation = new Vector3(mantleOffset.x, mantleOffset.y, -mantleOffset.z);
            }
            
        }

        public void Crouched(float inputCrouchHeight) {
            crouchHeight = inputCrouchHeight;
        }

        private void OnDrawGizmos() {
            if (headParent == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(headParent.position, .5f);
        }
    }
}
