using Hertzole.GoldPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cox.ControllerProject.GoldPlayerAddons {
    public class InventoryComponent : MonoBehaviour {
        [SerializeField] GameObject hand; //for a place to parent held items.
        [SerializeField] GameObject crosshair;
        [SerializeField] List<ItemBehaviour> items = new List<ItemBehaviour>();
        int activeItem = 0;

        PlayerInput input;

         void Awake() {
            InstantiateItems();
        }
        void InstantiateItems() { 
            for(int i = 0; i < items.Count; i++) {
                var instance = Instantiate(items[i], hand.transform);
                items[i] = instance;
                instance.gameObject.SetActive(false);
                
            }
            ItemChanged();
        }
        public void OnPrimaryUse() {
            if (items.Count > 0) {
                if (items[activeItem].usesCrosshair) {
                    items[activeItem].PrimaryFunction(crosshair);
                }
                else {
                    items[activeItem].PrimaryFunction();
                }
                
            }
        }
        public void OnSecondaryUse() {
            if (items.Count > 0) {
                items[activeItem].SecondaryFunction();
            }
        }

        public void OnInteract() {
            Debug.Log("Interaction Performed.");
        }

        public void OnChangeItem(InputValue input) {
            var value = input.Get<Vector2>().y;
            if (items.Count > 1 && value != 0) {
                items[activeItem].gameObject.SetActive(false);
                if (value > 0) {
                    if (activeItem < items.Count - 1) {
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
                        activeItem = items.Count - 1;
                    }
                }
                ItemChanged();
            }
        }

        public void ItemChanged() {
            if (items.Count > 0) {
                items[activeItem].gameObject.SetActive(true);
            }
        }

    }
}

