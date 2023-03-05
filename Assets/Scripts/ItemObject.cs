using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemObject : ScriptableObject {
    public string ID = string.Empty;
    public bool isInventoryOnly = true;
    public GameObject prefab; //for physical objects used by the player.

}
