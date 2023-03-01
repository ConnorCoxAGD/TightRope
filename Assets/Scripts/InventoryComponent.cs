using Hertzole.GoldPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cox.ControllerProject.GoldPlayerAddons {
    public class InventoryComponent : MonoBehaviour {
        [SerializeField] 
        GameObject hand; //for a place to parent held items.
        [SerializeField] 
        GameObject crosshair;
        //To permanently store items, we'll need to create a save file that get's loaded. Otherwise, changes will disappear everytime we restart.
        [SerializeField] 
        List<ItemBehaviour> handHeldItems = new List<ItemBehaviour>();
        [SerializeField]
        List<ItemBehaviour> inventoryOnlyItems = new List<ItemBehaviour>();
        int activeItem = 0;

        PlayerInput input;

         void Awake() {
            InstantiateItems();
        }
        void InstantiateItems() { 
            for(int i = 0; i < handHeldItems.Count; i++) {
                var instance = Instantiate(handHeldItems[i], hand.transform);
                handHeldItems[i] = instance;
                instance.gameObject.SetActive(false);
                
            }
            ItemChanged();
        }

        #region Input Actions
        public void OnPrimaryUse() {
            if (handHeldItems.Count > 0) {
                if (handHeldItems[activeItem].usesCrosshair) {
                    handHeldItems[activeItem].PrimaryFunction(crosshair);
                }
                else {
                    handHeldItems[activeItem].PrimaryFunction();
                }
                
            }
        }
        public void OnSecondaryUse() {
            if (handHeldItems.Count > 0) {
                handHeldItems[activeItem].SecondaryFunction();
            }
        }

        public void OnChangeItem(InputValue input) {
            var value = input.Get<Vector2>().y;
            if (handHeldItems.Count > 1 && value != 0) {
                handHeldItems[activeItem].gameObject.SetActive(false);
                if (value > 0) {
                    if (activeItem < handHeldItems.Count - 1) {
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
                        activeItem = handHeldItems.Count - 1;
                    }
                }
                ItemChanged();
            }
        }
        #endregion

        public void ItemChanged() {
            if (handHeldItems.Count > 0) {
                handHeldItems[activeItem].gameObject.SetActive(true);
            }
        }

        public void AddHandHeldItem(ItemBehaviour newItem) {
            handHeldItems.Add(newItem);
            newItem.transform.parent = hand.transform;
            newItem.transform.localPosition = Vector3.zero;

        }

    }
}

