using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cox.ControllerProject.GoldPlayerAddons;
public abstract class ToolBehaviour : InteractableObject
{
    public string itemName = "New Item";
    [TextArea(3, 10)]
    public string description = "Item description.";
    public bool usesCrosshair = true;

    virtual public void PrimaryFunction() {
        Debug.Log("No primary function.");
    }
    //work towards making this unnecessary
    virtual public void PrimaryFunction(GameObject crosshair) {
        Debug.Log("No primary function.");
    }
    virtual public void SecondaryFunction() {
        Debug.Log("No secondary function.");
    }
}
