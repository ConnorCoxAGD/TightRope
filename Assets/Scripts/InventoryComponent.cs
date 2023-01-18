using Hertzole.GoldPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cox.tightrope {
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
                    Debug.Log("Increment.");
                    if (activeItem < items.Count - 1) {
                        activeItem++;
                    }
                    else {
                        activeItem = 0;
                    }
                }
                else {
                    Debug.Log("Decrement.");
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
                //items[activeItem].transform.SetPositionAndRotation(hand.transform.position, hand.transform.rotation);
                Debug.LogFormat("Active item: {0}, Total items: {1}.", activeItem, items.Count);
                items[activeItem].gameObject.SetActive(true);
            }
        }

    }
}

