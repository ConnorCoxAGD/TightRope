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


        public override void PrimaryFunction(GameObject crosshair) {
            var direction = Camera.main.ScreenToWorldPoint(new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 1000));
            firePoint.LookAt(direction);
            FireRope();
        }

        public override void SecondaryFunction() {
            DisconnectRope();
        }

        private void FireRope() {

            RaycastHit hit;

            Debug.DrawRay(firePoint.position, firePoint.forward, Color.red, 5);
            if (isConnected) {
                Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistanceFromPlayer * 3);
                
            }
            else {
                Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistanceFromPlayer);
            }
            Debug.LogFormat("Object hit: {0}.", hit.collider?.gameObject);

            if (hit.collider?.gameObject?.GetComponent<AnchorComponent>()) {
                Debug.Log("Anchor Point Found!");
                var anchor = hit.collider.gameObject.GetComponent<AnchorComponent>();
                connectedAnchors.Add(anchor);
                if (!isConnected) {
                    isConnected = true;
                    line = Instantiate(linePrefab);
                    ropes.Insert(0, line.gameObject);
                    if (ropes.Count > ropeLimit) {
                        Destroy(ropes.Last<GameObject>());
                        ropes.TrimExcess();
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
                    line.SetPosition(1, connectedAnchors[0].transform.position);
                }
                else {
                    line.SetPosition(0, connectedAnchors[1].transform.position);
                    line.SetPosition(1, connectedAnchors[0].transform.position);
                    isConnected = false;
                    line = null;
                }

                if (Vector3.Distance(connectedAnchors[0].transform.position, firePoint.position) > maxDistanceFromPlayer) {
                    DisconnectRope();
                }
            }
        }

    }
}


