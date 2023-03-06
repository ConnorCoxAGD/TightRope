using UnityEngine;
namespace Cox.ControllerProject.GoldPlayerAddons {
    public abstract class InteractableObject : MonoBehaviour {
        [Header("Interactions")]
        [SerializeField]
        [Tooltip("Can this object be interacted with?")]
        protected bool interactable = true;
        [SerializeField]
        [Tooltip("The layer that the object interacts with. Should be the layer that the player is on.")]
        protected LayerMask playerLayer;

        public virtual void InteractionAreaEntered(PlayerControllerExtras player, Collider colliderData) {
            Debug.Log($"{this} interaction area entered {colliderData} collider.");
            player.InteractionMessage($"Interact with {this}");
        }
        public virtual void InteractionAreaExited(PlayerControllerExtras player, Collider colliderData) {
            Debug.Log($"{this} interaction area exited {colliderData} collider.");
            player.ClearMessage();
        }
        public virtual void Interact(PlayerControllerExtras player) {
            if (!interactable) {
                FailedInteraction(player);
                return;
            }
            Debug.Log($"{this} interaction performed by {player}.");
        }

        protected virtual void FailedInteraction(PlayerControllerExtras player) {
            Debug.Log($"Player cannot interact with {this} right now.");
            player.InteractionMessage($"Player cannot interact with {name} right now.", 2f);
        }

        protected virtual void FailedInteraction(PlayerControllerExtras player, string message) {
            Debug.Log($"Player cannot interact with {this} right now.");
            player.InteractionMessage(message, 2f);
        }
    }
}
