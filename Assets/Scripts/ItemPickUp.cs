using UnityEngine;

namespace Cox.ControllerProject.GoldPlayerAddons {

    public class ItemPickUp : InteractableObject {
        public ItemObject itemObject;

        public override void PrepInteraction(PlayerControllerExtras player) {
            base.PrepInteraction(player);
            player.InteractionMessage($"Pick up {itemObject.name}.");
        }

        public override void Interact(PlayerControllerExtras player) {
            base.Interact(player);

            if (player.inventory.AddItem(itemObject)) {
                player.InteractionMessage($"{itemObject.name} added to inventory.", 1f);
                Destroy(gameObject);
            }
            else {
                player.InteractionMessage($"{itemObject.name} could not be added to inventory.", 1f);
            }
        }
    }
}
