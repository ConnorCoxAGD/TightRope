using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BulletItemBehaviour : ItemBehaviour
{
    [SerializeField] float force;
    Rigidbody rb;
    private void OnEnable() {
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * force);
        Destroy(this.gameObject, 8);
    }

}
