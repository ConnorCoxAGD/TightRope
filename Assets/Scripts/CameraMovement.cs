using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {
  
        PlayerControllerExtras controllerExtras;
        Transform headParent; //This is the parent of the camera head. This allows us to TRANSLATE the head alongside the Gold Players movement. DO NOT USE TO ROTATE
        [SerializeField]
        AnimationCurve curve;
        float crouchTime;
        float defaultCrouchHeight;
        float startingHeight;
        float crouchHeight;

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
            switch (controllerExtras.movementState) {
                case MovementStates.Default:
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

                    break;
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
