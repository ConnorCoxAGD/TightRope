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
    public void OnJump() {
        if (onRope) {
            controller.Movement.CanMoveAround = true;
        }
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
        controller.Movement.CanMoveAround = !onRope;

        switch (onRope) {
            case true:
                if(rope.OnRopeCameraCheck() != null) {
                    var speed = movement.y * ropeWalkSpeed * Time.deltaTime;
                    var point = rope.OnRopeCameraCheck().position;
                    var move = Vector3.MoveTowards(controller.transform.position, point, speed);
                    controller.SetPosition(move); //Gold player made this a real pain for a while, but luckily they included this function.
                    Debug.LogFormat("Speed: {0} || Move: {1} || Controller: {2}", speed, move, controller.transform.position);
                }
                
                break;
            case false:
                break;
        }
    }


}
