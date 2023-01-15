using Hertzole.GoldPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [SerializeField] GameObject hand; //for a place to parent held items.
    [SerializeField] List<ItemBehaviour> items = new List<ItemBehaviour>();

    private void Awake() {

    }

    

}
