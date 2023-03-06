using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cox.ControllerProject.GoldPlayerAddons {
    public class InventoryComponent : MonoBehaviour {

        [SerializeField] 
        GameObject hand; //for a place to parent held items.
        //To permanently store items, we'll need to create a save file that get's loaded. Otherwise, changes will disappear everytime we restart.
        [SerializeField]
        [Tooltip("Items that can be handheld and used to perform actions.")]
        List<ToolBehaviour> toolsInventory = new List<ToolBehaviour>();
        [SerializeField]
        [Tooltip("Items that stay are never handheld, but allow the player to pass certain 'checks' or interact with the environment in some different way.")]
        List<ItemObject> itemInventory = new List<ItemObject>();
        int activeItem = 0;

         void Awake() {
            InstantiateItems();
        }
        void InstantiateItems() { 
            for(int i = 0; i < toolsInventory.Count; i++) {
                var instance = Instantiate(toolsInventory[i], hand.transform);
                toolsInventory[i] = instance;
                instance.gameObject.SetActive(false);
                
            }
            ItemChanged();
        }

        #region Input Actions
        public void OnPrimaryUse() {
            if (toolsInventory.Count > 0) {
                if (toolsInventory[activeItem].usesCrosshair) {
                    //toolsInventory[activeItem].PrimaryFunction(crosshair);
                }
                else {
                    toolsInventory[activeItem].PrimaryFunction();
                }
                
            }
        }
        public void OnSecondaryUse() {
            if (toolsInventory.Count > 0) {
                toolsInventory[activeItem].SecondaryFunction();
            }
        }

        public void OnChangeItem(InputValue input) {
            var value = input.Get<Vector2>().y;
            if (toolsInventory.Count > 1 && value != 0) {
                toolsInventory[activeItem].gameObject.SetActive(false);
                if (value > 0) {
                    if (activeItem < toolsInventory.Count - 1) {
                        activeItem++;
                    }
                    else {
                        activeItem = 0;
                    }
                }
                else {
                    if (activeItem >= 1) {
                        activeItem--;
                    }
                    else {
                        activeItem = toolsInventory.Count - 1;
                    }
                }
                ItemChanged();
            }
        }
        #endregion
        #region Tools
        public void ItemChanged() {
            if (toolsInventory.Count > 0) {
                toolsInventory[activeItem].gameObject.SetActive(true);
            }
        }

        public void AddTool(ToolBehaviour newItem) {
            toolsInventory.Add(newItem);
            newItem.transform.parent = hand.transform;
            newItem.transform.localPosition = Vector3.zero;

        }
        #endregion

        #region Items
        /// <summary>
        /// Adds an item to the item inventory and let's the item know if it was successfully added.
        /// </summary>
        /// <returns>True when adding an item is successful. Allows for control from the item in the event of failure.</returns>
        public bool AddItem(ItemObject item) {
            if (itemInventory == null) return false; //in the future, we can use this to check if inventory is full if we want.
            itemInventory.Add(item);
            return true;
        }
        public bool RemoveItem(ItemObject item) { 
            if(itemInventory == null) return false;
            if (!itemInventory.Remove(item)) {
                Debug.LogError($"{item} not found in {this.itemInventory}.");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Used to tell if the inventory component has a particular ItemObject.
        /// </summary>
        /// <returns>True when '_item' matches an ItemObject in the Inventory.</returns>
        public bool CompareItem(ItemObject _item) {
            foreach(var item in itemInventory) {
                if(item == _item) {
                    return true;
                }
            }
            return false;
        }
        public void CompareItemList(ItemObject[] items) { 
        //to-do
        }
        #endregion

    }
}

