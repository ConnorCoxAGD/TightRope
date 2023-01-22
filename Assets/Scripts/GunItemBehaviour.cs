using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunItemBehaviour : ItemBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;

    private void OnEnable() {
    }

    public override void PrimaryFunction(GameObject crosshair) {
        var crosshairPos = Camera.main.ScreenToWorldPoint(new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 1));
        var distance = Camera.main.transform.forward * 1000;
        RaycastHit hit;
        Physics.Raycast(crosshairPos, distance, out hit);
        if (hit.collider) {
            Debug.LogFormat("Hit! Point is {0} distance.", Vector3.Distance(transform.position, hit.transform.position));
            firePoint.LookAt(hit.point);
        }
        else {
            Debug.LogFormat("No hit. Point is {0} distance.", distance);
            firePoint.LookAt(distance);
        }
        var b = Instantiate(bullet, firePoint.position, firePoint.transform.rotation);
    }
}
