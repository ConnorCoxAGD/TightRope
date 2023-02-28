using Cox.ControllerProject.GoldPlayerAddons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Can this object be interacted with?")]
    protected bool interactable = true;
    [SerializeField]
    [Tooltip("The layer that the object interacts with. Should be the layer that the player is on.")]
    protected LayerMask playerLayer;
    [SerializeField]
    string interactionMessage = "Explanation of interaction.";

    public virtual void InteractionAreaEntered(PlayerControllerExtras player, Collider colliderData) {
        Debug.Log($"{this} interaction area entered {colliderData} collider.");
    }
    public virtual void InteractionAreaExited(PlayerControllerExtras player, Collider colliderData) {
        Debug.Log($"{this} interaction area exited {colliderData} collider.");
    }
    public virtual void Interact(PlayerControllerExtras player) {
        Debug.Log($"{this} interaction performed by {player}.");
    }
}
