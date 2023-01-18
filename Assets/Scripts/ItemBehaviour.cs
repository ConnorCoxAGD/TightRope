using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{
    public string itemName = "New Item";
    [TextArea(3, 10)]
    public string description = "Item description.";
    public bool usesCrosshair = true;

    virtual public void PrimaryFunction() {
        Debug.Log("No primary function.");
    }
    virtual public void PrimaryFunction(GameObject crosshair) {
        Debug.Log("No primary function.");
    }
    virtual public void SecondaryFunction() {
        Debug.Log("No secondary function.");
    }
}
