using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Filo;

namespace cox.tightrope {
    public class RopeItemBehaviour : ItemBehaviour {
        [Tooltip("The maximum amount of ropes that can be in play at once.")]
        [SerializeField] int ropeLimit = 1;
        [Tooltip("The maximum distance away from an anchor point you can be on the first rope connection.")]
        [SerializeField] float maxDistanceFromPlayer = 3;
        [SerializeField] Transform firePoint;
        bool isConnected = false;
        [SerializeField] Cable ropePrefab;
        Cable rope = null;
        List<Cable> ropes = new List<Cable>();

        CableSolver ropeSolver = null;

        private void Awake() {
            ropeSolver = FindObjectOfType<CableSolver>();
        }

        public override void PrimaryFunction(GameObject crosshair) {
            //Use camera and crosshair raycast for really accurate aiming
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
                AttachRopePart(hit);
            }
            else {
                Debug.Log("No anchor found.");
            }
        }

        void AttachRopePart(RaycastHit hit) {
            var anchor = hit.collider.gameObject.GetComponent<AnchorComponent>();
            if (!isConnected) {
                isConnected = true;
                //create rope
                rope = Instantiate(ropePrefab);
                rope.name = "rope";
                ropes.Insert(0, rope);
                if (ropes.Count > ropeLimit) {
                    //destroy excess ropes
                    var lastRope = ropes.Last<Cable>();
                    ropes.Remove(lastRope);
                    ropes.TrimExcess();
                    Destroy(lastRope.gameObject);
                    Debug.Log("destroy rope");
                }
                //add ropes to solver
                ropeSolver.cables = ropes.ToArray();
                //move rope links to desired positions
                rope.links[0].body.gameObject.transform.position = firePoint.position;
                rope.links[0].body.gameObject.transform.SetParent(firePoint); //allows rope to follow player position
                rope.links[1].body.gameObject.transform.position = hit.point;
            }
            else {
                isConnected = false;
                rope.links[0].body.gameObject.transform.position = hit.point;
                rope.links[0].body.gameObject.transform.SetParent(null);
                rope = null;
            }
        }

        void DisconnectRope() {
            isConnected = false;
            if (rope == null) return;
            Destroy(rope.gameObject);
            ropes.TrimExcess();
            ropeSolver.cables = ropes.ToArray();
        }
    }
}


