using UnityEngine;
namespace Cox.ControllerProject.GoldPlayerAddons {
    [CreateAssetMenu(fileName = "New Item", menuName = "Create Item Object")]
    public class ItemObject : ScriptableObject {
        public string ID = string.Empty;
        public ItemPickUp pickUpPrefab = null;

    }
}
