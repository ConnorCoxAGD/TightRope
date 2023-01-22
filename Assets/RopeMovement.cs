using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeMovement : MonoBehaviour
{
    GoldPlayerController controller;
    [SerializeField]
    LayerMask ropeLayer = -1;
    bool onRope = false;
    float ropeWalkSpeed = 2.5f;
    public RopeComponent rope;
    Vector2 movement;

    private void Awake() {
        controller = GetComponent<GoldPlayerController>();
        //ropeWalkSpeed = controller.Movement.WalkingSpeeds.ForwardSpeed;
    }

    public void OnMove(InputValue input) {
        movement = input.Get<Vector2>();
    }

    public bool CheckForRope() {
        return Physics.CheckSphere(new Vector3(controller.Movement.PlayerTransform.position.x, 
            controller.Movement.PlayerTransform.transform.position.y + controller.Movement.CharacterController.radius - 0.1f, 
            controller.Movement.PlayerTransform.position.z), controller.Movement.CharacterController.radius, 
            ropeLayer, 
            QueryTriggerInteraction.Ignore);
    }

    private void Update() {
        onRope = CheckForRope();

        switch (onRope) {
            case true:
                controller.Movement.MoveSpeedMultiplier = 0;
                movement = controller.Movement.SmoothedMovementInput;
                if(rope.OnRopeCameraCheck() != null) {
                    var speed = movement.y * ropeWalkSpeed * Time.deltaTime;
                    var point = rope.OnRopeCameraCheck().position;
                    var move = Vector3.MoveTowards(controller.transform.position, new Vector3(point.x, controller.transform.position.y * 2, point.z), speed );
                    controller.transform.position = move;
                    Debug.LogFormat("Speed: {0} || Move: {1} || Controller: {2}", speed, move, controller.transform.position);
                }
                

                break;
            case false:
                controller.Movement.MoveSpeedMultiplier = 1;
                break;
        }
    }


}
