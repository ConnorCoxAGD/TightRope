using Cox.ControllerProject.GoldPlayerAddons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorStates {
    unlockedClosed,
    opened,
    locked,
    latched,
    sealedShut
}

public class DoorBehaviour : InteractableObject
{
    [SerializeField]
    [TextArea(2, 5)]
    string unlockedClosedMessage = "This message displays when the door is unlocked but closed.",
        openedMessage = "This message displays when the door is opened.",
        lockedMessage = "This message displays when the door is locked.",
        latchedMessage = "This message displays when the door is latched.",
        sealedMessage = "This message displays when the door is sealed.";
    [SerializeField]
    DoorStates state = DoorStates.unlockedClosed;
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

    private void SetAllTriggersActiveState(bool activeState) {
        frontTrigger.gameObject.SetActive(activeState);
        backTrigger.gameObject.SetActive(activeState);
    }

    private void SetActiveTrigger(Collider trigger, bool activeState) {
        trigger.gameObject.SetActive(activeState);
    }

    private void MoveDoor() {
        doorPivot.transform.localRotation = Quaternion.Lerp(doorPivot.transform.localRotation, goToRotation, openSpeed * Time.deltaTime);

        if (doorPivot.transform.localRotation == goToRotation) {
            isMoving = false;
            isOpen = !isOpen;
            SetAllTriggersActiveState(true);
        }
    }
    private void StartMovement() {
        isMoving = true;
        SetAllTriggersActiveState(false);
        if (isOpen) {
            goToRotation = Quaternion.identity;
        }
        else {
            goToRotation = Quaternion.Euler(doorPivot.transform.localRotation.x + openRotationOffset.x, doorPivot.transform.localRotation.y + openRotationOffset.y, doorPivot.transform.localRotation.z + openRotationOffset.z);
        }
    }

    public override void Interact(PlayerControllerExtras player) {
        if (!interactable) { 
            FailedInteraction();
            return;
        }
        if (isOpen) {
            state = DoorStates.opened;
        }

        switch (state) {
            case DoorStates.unlockedClosed:
                StartMovement();
                state = DoorStates.opened;
                player.ClearMessage();
                break;
            case DoorStates.opened:
                StartMovement();
                player.ClearMessage();
                state = DoorStates.unlockedClosed;
                break;
            case DoorStates.locked:
                //if player has key, set to unlockedClosed
                break;
            case DoorStates.latched:
                //if player is in correct trigger? or activate latch warning trigger?
                break;
            case DoorStates.sealedShut:
                //Warn player the door is sealed and cannot be opened by the player through normal or any means.
                break;
        }
    }
    public override void InteractionAreaEntered(PlayerControllerExtras player, Collider colliderData) {
        //base.InteractionAreaEntered(player, colliderData);

        switch (state) {
            case DoorStates.unlockedClosed:
                player.InteractionMessage(unlockedClosedMessage);
                break;
            case DoorStates.opened:
                player.InteractionMessage(openedMessage);
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

    protected override void FailedInteraction() {
        base.FailedInteraction();
    }
    //if latched, we'll simply turn off the latched collider
}
