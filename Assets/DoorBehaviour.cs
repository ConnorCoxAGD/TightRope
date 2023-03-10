using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {
    public enum DoorStates {
        closed,
        opened,
        locked,
        latched,
        sealedShut
    }

    public class DoorBehaviour : InteractableObject {
        #region Serialized Fields
        [Space]
        [Header("State and Unlocking")]
        [Space]
        [SerializeField]
        DoorStates state = DoorStates.closed;

        [SerializeField]
        [Tooltip("Item Object used to unlock the door when set to 'locked'.")]
        ItemObject key = null;

        [SerializeField]
        bool openOnUnlock = false;

        [SerializeField]
        bool destroyKeyOnUnlock = false;
        [Space]
        [Header("Setup")]
        [Space]
        [SerializeField]
        [Tooltip("This is the object that acts as the door istelf. This object will be rotated and transformed without the triggers being moved.")]
        GameObject doorObject;
        [SerializeField]
        [Tooltip("Acts as a pivot for the door to rotate on. Create a transform and add it here to have it act as a pivot for the door.")]
        Transform doorPivot;

        [Space]
        [Header("Movement")]
        [Space]
        [SerializeField]
        Vector3 openRotation = new Vector3(0, 120, 0);
        [SerializeField]
        float openSpeed = 2.5f;
        [SerializeField]
        bool flipOpenDirection = false;
        //doorFlipsDirectionBasedOnPlayerPosition = false; IDEA FOR LATER
        [Space]
        [Space]
        [Header("Messages")]
        [SerializeField]
        string closedMessage = "Open door.";
        [SerializeField]
        string openedMessage = "Close door.";
        [SerializeField]
        string lockedMessage = "The door is locked.";
        [SerializeField]
        string latchedMessage = "The door is latched shut from the other side.";
        [SerializeField]
        string sealedMessage = "Door is sealed shut and cannot be opened.";
        #endregion

        bool isOpen = false;
        bool isMoving = false;
        Quaternion goToRotation = Quaternion.identity;

        private void Awake() {
            if (doorPivot == null) {
                Debug.LogError($"{this} is missing a door pivot. The door is unable to rotate. Create a transform and apply it to the door component's 'Door Pivot' variable in the inspector.");
                return;
            }
            if (doorObject == null) {
                Debug.LogWarning($"{this} is missing a door object. This is the object that is meant to be rotated. The script will now automatically search for a mesh to set into this spot, but for best results, manually set one yourself.");
                doorObject = GetComponentInChildren<MeshRenderer>().gameObject;
            }
            if (state == DoorStates.locked && key == null) {
                state = DoorStates.closed;
                Debug.LogWarning($"{this} is set to Locked but has no key. It's state has automatically been changed to unlocked. If you want the door to be locked with no key, set it to 'Sealed'.");
            }

            doorObject.transform.parent = doorPivot;

            if (flipOpenDirection) {
                openRotation *= -1;
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
                goToRotation = Quaternion.Euler(doorPivot.transform.localRotation.x + openRotation.x, doorPivot.transform.localRotation.y + openRotation.y, doorPivot.transform.localRotation.z + openRotation.z);
            }
        }

        public override void Interact(PlayerControllerExtras player) {
            if (!interactable) {
                FailedInteraction(player);
                return;
            }
            if (isOpen) {
                state = DoorStates.opened;
            }

            switch (state) {
                case DoorStates.closed:
                    StartMovement();
                    state = DoorStates.opened;
                    player.ClearMessage();
                    break;
                case DoorStates.opened:
                    StartMovement();
                    player.ClearMessage();
                    state = DoorStates.closed;
                    break;
                case DoorStates.locked:
                    string message = $"Door cannot be opened without {key.name}.";
                    if (player.inventory != null) {
                        if (player.inventory.CompareItem(key)) {
                            state = DoorStates.closed;
                            message = $"Door unlocked. {closedMessage}";

                            if (destroyKeyOnUnlock) {
                                player.inventory.RemoveItem(key);
                                message = $"{key.name} removed." + message;
                            }
                            if (openOnUnlock) {
                                message = message + "\nDoor opened.";
                                StartMovement();
                                state = DoorStates.opened;
                            }

                            player.InteractionMessage(message);
                            break;
                        }
                    }

                    FailedInteraction(player, message);
                    break;
                case DoorStates.latched:
                    //if player is in correct trigger? or activate latch warning trigger?
                    break;
                case DoorStates.sealedShut:
                    //Warn player the door is sealed and cannot be opened by the player through normal or any means.
                    break;
            }
        }
        public override void PrepInteraction(PlayerControllerExtras player) {
            switch (state) {
                case DoorStates.closed:
                    player.InteractionMessage(closedMessage);
                    break;
                case DoorStates.opened:
                    player.InteractionMessage(openedMessage);
                    break;
                case DoorStates.locked:
                    if (player.inventory != null) {
                        string message = lockedMessage;
                        if (player.inventory.CompareItem(key)) {
                            message += $"\n Use {key.name} to unlock.";
                            player.InteractionMessage(message);
                        }
                        else {
                            message += $"\n Requires {key.name}.";
                            player.InteractionMessage(message);
                        }
                        break;
                    }
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

        //if latched, we'll simply turn off the latched collider
    }
}
