using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace cox.tightrope {
    public class RopeItemBehaviour : ItemBehaviour {
        [SerializeField] int ropeLimit = 1;
        [SerializeField] float maxDistanceFromPlayer = 3;
        [SerializeField] Transform firePoint;
        [SerializeField] List<AnchorComponent> connectedAnchors = new List<AnchorComponent>();
        bool isConnected = false;
        [SerializeField] LineRenderer linePrefab;
        [SerializeField] LineRenderer line;
        List<GameObject> ropes = new List<GameObject>();

        GameObject crosshair;
        public override void PrimaryFunction(GameObject crosshair) {
            var crosshairPos = Camera.main.ScreenToWorldPoint(new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 1));
            var distance = Camera.main.transform.forward * 1000;
            RaycastHit hit;
            Physics.Raycast(crosshairPos, distance, out hit);
            if (hit.collider) {
                firePoint.LookAt(hit.point);
            }
            else {
                firePoint.LookAt(distance);
            }
            FireRope();
        }

        public override void SecondaryFunction() {
            DisconnectRope();
        }

        private void FireRope() {

            RaycastHit hit;

            Debug.DrawRay(firePoint.position, firePoint.forward * 10, Color.red, 5);
            if (isConnected) {
                Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistanceFromPlayer * 3);
            }
            else {
                Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistanceFromPlayer);
            }

            if (hit.collider?.gameObject?.GetComponent<AnchorComponent>()) {
                var anchor = hit.collider.gameObject.GetComponent<AnchorComponent>();
                anchor.attachPoint = hit.point;
                connectedAnchors.Add(anchor);
                if (!isConnected) {
                    isConnected = true;
                    line = Instantiate(linePrefab);
                    ropes.Insert(0, line.gameObject);
                    if (ropes.Count > ropeLimit) {
                        var lastRope = ropes.Last<GameObject>();
                        ropes.Remove(lastRope);
                        ropes.TrimExcess();
                        Destroy(lastRope);
                        Debug.Log("destroy rope");
                    }
                }
            }
            else {
                Debug.Log("No anchor found.");
            }
        }

        void DisconnectRope() {
            isConnected = false;
            Destroy(line);
            ropes.TrimExcess();
            connectedAnchors.Clear();
            connectedAnchors.TrimExcess();
        }

        private void Update() {
            if (isConnected) {
                if (connectedAnchors.Count < 2) {
                    line.SetPosition(0, firePoint.position);
                    line.SetPosition(1, connectedAnchors[0].attachPoint);
                }
                else {
                    isConnected = false;
                    line.SetPosition(0, connectedAnchors[1].attachPoint);
                    line.SetPosition(1, connectedAnchors[0].attachPoint);
                    connectedAnchors.Clear();
                    connectedAnchors.TrimExcess();
                    line = null;
                }

                if (connectedAnchors.Count > 0 && Vector3.Distance(connectedAnchors[0].attachPoint, firePoint.position) > maxDistanceFromPlayer) {
                    DisconnectRope();
                }
            }
        }

    }
}


