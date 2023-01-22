using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    Vector2 smoothMove;
    Vector2 movement;
    Vector2 prevMove = Vector2.zero;
    float originalJumpHeight;

    private void Awake() {
        controller = GetComponent<GoldPlayerController>();
        originalJumpHeight = controller.Movement.JumpHeight;
        //ropeWalkSpeed = controller.Movement.WalkingSpeeds.ForwardSpeed;
    }

    public void OnMove(InputValue input) {
        movement = input.Get<Vector2>();
        
    }
    public void OnJump() {
        if (onRope) {
            controller.Movement.CanMoveAround = true;
        }
    }

    public bool CheckForRope() {
        //allow for changing active rope
        var overlap = Physics.OverlapSphere(new Vector3(controller.Movement.PlayerTransform.position.x,
            controller.Movement.PlayerTransform.transform.position.y + controller.Movement.CharacterController.radius - 0.1f,
            controller.Movement.PlayerTransform.position.z), controller.Movement.CharacterController.radius,
            ropeLayer,
            QueryTriggerInteraction.Ignore);
        foreach(var item in overlap) {
            rope = item.GetComponent<RopeComponent>();
        }
        //detect if grounded to a rope
        return Physics.CheckSphere(new Vector3(controller.Movement.PlayerTransform.position.x, 
            controller.Movement.PlayerTransform.transform.position.y + controller.Movement.CharacterController.radius - 0.1f, 
            controller.Movement.PlayerTransform.position.z), controller.Movement.CharacterController.radius, 
            ropeLayer, 
            QueryTriggerInteraction.Ignore);
    }

    private void Update() {
        onRope = CheckForRope();
        controller.Movement.CanMoveAround = !onRope;

        switch (onRope) {
            case true:
                if(rope.OnRopeCameraCheck() != null) {
                    controller.Movement.JumpHeight = originalJumpHeight * 1.5f;
                    smoothMove = Vector2.Lerp(prevMove, movement, 3* Time.deltaTime);
                    prevMove = smoothMove;
                    var speed = smoothMove.y * ropeWalkSpeed * Time.deltaTime;
                    var point = rope.OnRopeCameraCheck().position;
                    var move = Vector3.MoveTowards(controller.transform.position, point, speed);
                    controller.SetPosition(move); //Gold player made this a real pain for a while, but luckily they included this function.
                    //Debug.LogFormat("Speed: {0} || Move: {1} || Controller: {2}", speed, move, controller.transform.position);
                }
                
                break;
            case false:
                controller.Movement.JumpHeight = originalJumpHeight;
                break;
        }
    }


}
