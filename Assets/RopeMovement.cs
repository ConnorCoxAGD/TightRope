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

    private void Awake() {
        controller = GetComponent<GoldPlayerController>();
    }

    public void OnMove(InputValue input) {

        switch (onRope) {
            case true:
                var value = input.Get<Vector2>().y;
                if(value > 0) {
                    controller.Movement.CanMoveAround = false;
                    controller.SetPosition(Vector3.MoveTowards(transform.position, rope.pointA.position, ropeWalkSpeed));
                }

                break;
            case false:
                controller.Movement.CanMoveAround = true;


                break;
        }

        
    }

    public bool CheckForRope() {
        return Physics.CheckSphere(new Vector3(controller.movement.PlayerTransform.position.x, 
            controller.movement.PlayerTransform.position.y + controller.movement.CharacterController.radius - 0.1f, 
            controller.movement.PlayerTransform.position.z), controller.movement.CharacterController.radius, 
            ropeLayer, 
            QueryTriggerInteraction.Ignore);
    }

    private void Update() {
        onRope = CheckForRope();
    }

}
