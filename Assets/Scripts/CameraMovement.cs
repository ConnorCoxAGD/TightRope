using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {
        [SerializeField]
        Camera m_camera;
        [SerializeField]
        Vector3 mantleCameraRotation = new Vector3(2, 0, 15);
        [SerializeField]
        [Range(1, 65)]
        float mantleCameraSpeed = 5;
        [SerializeField]
        [Tooltip("Subtracted from to players height to give the camera a distance down to travel.")]
        float hardLandingCameraHeight = 0.2f;
        [SerializeField]
        Vector3 hardLandingRotation = Vector3.zero;

        PlayerControllerExtras controllerExtras;
        Transform headParent; //Used to manipualte the 'head' without interfering with the Gold Player Controller.

        float crouchTime;
        float defaultCrouchHeight;
        float startingHeight;
        float crouchHeight;
        Vector3 goToRotation = Vector3.zero;
        Vector3 goToPosition = Vector3.zero;

        public void Initialize(PlayerControllerExtras controller) {
            if(m_camera == null) {
                m_camera = Camera.main;
                Debug.LogWarning("No Camera set to 'm_camera' in the Camera Movement script. Camera automatically set to Camera.main");
            }
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
            if (controllerExtras.isMantling) {
                Quaternion rot = Quaternion.Euler(goToRotation);
                headParent.localRotation = Quaternion.Slerp(headParent.localRotation, rot, mantleCameraSpeed * Time.deltaTime);
                return;
            }
            if (controllerExtras.isCrouching) {
                var newPos = new Vector3(headParent.localPosition.x, defaultCrouchHeight - (defaultCrouchHeight - crouchHeight), headParent.localPosition.z);
                headParent.localPosition = Vector3.MoveTowards(headParent.localPosition, newPos, crouchTime * Time.deltaTime);
                return;
            }
            if (controllerExtras.hardLanding) {
                var newPos = new Vector3(headParent.localPosition.x, startingHeight - hardLandingCameraHeight, headParent.localPosition.z);
                headParent.localPosition = Vector3.Slerp(headParent.localPosition, newPos, 25 * Time.deltaTime);
                Quaternion rot = Quaternion.Euler(goToRotation);
                headParent.localRotation = Quaternion.Slerp(headParent.localRotation, rot, mantleCameraSpeed * 1.5f * Time.deltaTime);
                return;
            }

            //if not the others
            if (headParent.localRotation.eulerAngles != Vector3.zero) {
                Quaternion returnRot = Quaternion.identity;
                headParent.localRotation = Quaternion.Slerp(headParent.localRotation, returnRot, mantleCameraSpeed * 1.5f * Time.deltaTime);
            }
            if (headParent.transform.localPosition.y < startingHeight) {
                var upPos = new Vector3(headParent.localPosition.x, startingHeight, headParent.localPosition.z);
                headParent.localPosition = Vector3.MoveTowards(headParent.localPosition, upPos, crouchTime * Time.deltaTime);
            }

        }

        public void HardLanding() {
            goToRotation = hardLandingRotation;
        }

        public void ReturnToZero() {
            goToRotation = Vector3.zero;
        }

        public void Mantle() {
            int decide = Random.Range(0, 2);
            if(decide == 0) {
                goToRotation = mantleCameraRotation;
            }
            else {
                goToRotation = new Vector3(mantleCameraRotation.x, mantleCameraRotation.y, -mantleCameraRotation.z);
            }
            
        }

        public void Crouched(float inputCrouchHeight) {
            crouchHeight = inputCrouchHeight;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (headParent == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(headParent.position, .5f);
        }
#endif
    }
}
