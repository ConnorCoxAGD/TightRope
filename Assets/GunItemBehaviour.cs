using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItemBehaviour : ItemBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float speed;
    [SerializeField] Transform shotPoint;

    private void OnEnable() {
    }

    public override void Activate() {
        var b = Instantiate(bullet, shotPoint.position, shotPoint.rotation);
    }
}
