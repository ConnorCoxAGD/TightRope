using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    /// <summary>
    /// Allows for advanced control of the camera. Works with PlayerControllerExtras.
    /// </summary>
    public class CameraMovement : MonoBehaviour {
  
        PlayerControllerExtras controllerExtras;
        Transform head; //This is the parent of the camera head. This allows us to TRANSLATE the head alongside the Gold Players movement. DO NOT USE TO ROTATE
        Animation anim;
        [SerializeField]
        AnimationClip mantleAnim;
        [SerializeField]
        AnimationCurve curve = new AnimationCurve();
        float crouchTime;
        float defaultCrouchHeight;
        float startingHeight;
        float crouchHeight;

        public void Initialize(PlayerControllerExtras controller) {
            controllerExtras = controller;
            crouchTime = controllerExtras.goldPlayerController.Movement.CrouchTime * 5;
            defaultCrouchHeight = controllerExtras.goldPlayerController.Movement.CrouchHeight;
            //Programatically setup the Head Parent
            head = new GameObject("HeadParent").transform;
            head.rotation = Quaternion.identity;
            head.SetParent(controllerExtras.goldPlayerController.gameObject.transform);
            head.position = controllerExtras.goldPlayerController.Camera.CameraHead.position;

            controllerExtras.goldPlayerController.Camera.CameraHead.SetParent(head);
            startingHeight = head.localPosition.y;
            anim = gameObject.AddComponent<Animation>();
            mantleAnim = new AnimationClip();
            mantleAnim.legacy = true;
            anim.AddClip(mantleAnim, mantleAnim.name);
            anim.clip = mantleAnim;
            mantleAnim.SetCurve("", typeof(Transform), "Rotation.x", curve);
            anim.Play(mantleAnim.name);
           
            

        }

        private void LateUpdate() {
            switch (controllerExtras.movementState) {
                case MovementStates.Default:
                    if (head.transform.localPosition.y < startingHeight) {
                        var upPos = new Vector3(head.transform.localPosition.x, startingHeight, head.transform.localPosition.z);
                        head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, upPos, crouchTime * Time.deltaTime);
                    }
                    break;
                case MovementStates.Crouching:
                    var newPos = new Vector3(head.transform.localPosition.x, defaultCrouchHeight - (defaultCrouchHeight - crouchHeight), head.transform.localPosition.z);
                    head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, newPos, crouchTime * Time.deltaTime);
                    break;
                case MovementStates.Mantling:

                    break;
            }

            if (!anim.isPlaying) {
                anim.Play(mantleAnim.name);
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
