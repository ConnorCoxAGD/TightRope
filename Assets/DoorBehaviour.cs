using Cox.ControllerProject.GoldPlayerAddons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorStates {
    unlocked,
    locked,
    latched,
    sealedShut
}

public class DoorBehaviour : InteractableObject
{
    [SerializeField]
    string unlockedMessage = "This message displays when the door is unlocked.",
        lockedMessage = "This message displays when the door is locked.",
        latchedMessage = "This message displays when the door is latched.",
        sealedMessage = "This message displays when the door is sealed.";
    [SerializeField]
    DoorStates state = DoorStates.unlocked;
    [SerializeField]
    [Tooltip("Used to allow the player to open the door. Must be set to 'Trigger'.")]
    Collider frontTrigger, backTrigger;
    [SerializeField]
    bool flipOpenDirection = false, doorFlipsDirectionBasedOnPlayerPosition = false;
    [SerializeField]
    [Tooltip("Acts as a pivot for the door to rotate on. Create a transform and add it here to have it act as a pivot for the door.")]
    Transform doorPivot;
    [SerializeField]
    Vector3 openRotationOffset = new Vector3(0, 120, 0);
    [SerializeField]
    [Tooltip("This is the object that acts as the door istelf. This object will be rotated and transformed without the triggers being moved.")]
    GameObject doorObject;
    [SerializeField]
    float openSpeed = 5;
    bool isOpen = false;
    bool isMoving = false;
    Quaternion goToRotation = Quaternion.identity;

    private void Awake() {
        if(frontTrigger == null || backTrigger == null) {
            Debug.LogError($"{this} is missing one or more triggers for the door. Create a collider, set it as a trigger, and apply it to the door component's 'Front Trigger' and 'Back Trigger' variables.");
            return;
        }
        if (doorPivot == null) {
            Debug.LogError($"{this} is missing a door pivot. The door is unable to rotate. Create a transform and apply it to the door component's 'Door Pivot' variable in the inspector.");
            return;
        }
        if(doorObject == null) {
            Debug.LogWarning($"{this} is missing a door object. This is the object that is meant to be rotated. The script will now automatically search for a mesh to set into this spot, but for best results, manually set one yourself.");
            doorObject = GetComponentInChildren<MeshRenderer>().gameObject;
        }

        doorObject.transform.parent = doorPivot;

        if (flipOpenDirection) {
            openRotationOffset *= -1;
        }
    }

    private void Update() {
        if (isMoving) {
            MoveDoor();
        }
    }

    private void MoveDoor() {
        doorPivot.transform.localRotation = Quaternion.Lerp(doorPivot.transform.localRotation, goToRotation, openSpeed * Time.deltaTime);

        if (doorPivot.transform.localRotation == goToRotation) {
            isMoving = false;
            isOpen = !isOpen;
        }
    }
    private void StartMovement() {
        isMoving = true;
        if (isOpen) {
            goToRotation = Quaternion.identity;
        }
        else {
            goToRotation = Quaternion.Euler(doorPivot.transform.localRotation.x + openRotationOffset.x, doorPivot.transform.localRotation.y + openRotationOffset.y, doorPivot.transform.localRotation.z + openRotationOffset.z);
        }
    }

    public override void Interact(PlayerControllerExtras player) {
        base.Interact(player);

        switch (state) {
            case DoorStates.unlocked:
                StartMovement();
                break;
            case DoorStates.locked:

                break;
            case DoorStates.latched:

                break;
            case DoorStates.sealedShut:

                break;
        }
    }
    public override void InteractionAreaEntered(PlayerControllerExtras player, Collider colliderData) {
        //base.InteractionAreaEntered(player, colliderData);

        switch (state) {
            case DoorStates.unlocked:
                player.InteractionMessage(unlockedMessage);
                break;
            case DoorStates.locked:
                player.InteractionMessage(lockedMessage);
                break;
            case DoorStates.latched:
                player.InteractionMessage(latchedMessage);
                break;
            case DoorStates.sealedShut:
                player.InteractionMessage(sealedMessage);
                break;
        }
    }
    public override void InteractionAreaExited(PlayerControllerExtras player, Collider colliderData) {
        //base.InteractionAreaExited(player, colliderData);

        player.ClearMessage();
    }
    //if latched, we'll simply turn off the latched collider
}
