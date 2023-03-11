using UnityEngine;
namespace Cox.ControllerProject.GoldPlayerAddons {
    public abstract class InteractableObject : MonoBehaviour {
        [SerializeField]
        [Tooltip("Can this object be interacted with?")]
        protected bool isInteractable = true;

        public virtual void PrepInteraction(PlayerControllerExtras player) {
            player.InteractionMessage($"Interact with {this}");
        }
        public virtual void CancelInteraction(PlayerControllerExtras player) {
            player.ClearMessage();
        }
        public virtual void Interact(PlayerControllerExtras player) {
            if (!isInteractable) {
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
