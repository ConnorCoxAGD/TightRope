using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{
    public string itemName = "New Item";
    [TextArea(3, 10)]
    public string description = "Item description.";

    virtual public void Activate() { }

    virtual public void Deactivate() { }
}
