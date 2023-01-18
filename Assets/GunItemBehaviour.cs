using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItemBehaviour : ItemBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float speed;
    [SerializeField] Transform firePoint;

    private void OnEnable() {
    }

    public override void PrimaryFunction(GameObject crosshair) {
        var direction = Camera.main.ScreenToWorldPoint(new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 1000));
        firePoint.LookAt(direction);
        var b = Instantiate(bullet, firePoint.position, firePoint.transform.rotation);
    }
}
