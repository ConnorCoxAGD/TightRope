using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Filo;

namespace cox.tightrope {
    public class RopeItemBehaviour : ItemBehaviour {
        [SerializeField] int ropeLimit = 1;
        [SerializeField] float maxDistanceFromPlayer = 3;
        [SerializeField] Transform firePoint;
        [SerializeField] List<AnchorComponent> connectedAnchors = new List<AnchorComponent>();
        bool isConnected = false;
        [SerializeField] Cable ropePrefab;
        [SerializeField] CablePoint cableAttachPoint;
        Cable rope = null;
        List<Cable> ropes = new List<Cable>();

        CableSolver ropeSolver = null;

        GameObject crosshair;

        private void Awake() {
            ropeSolver = FindObjectOfType<CableSolver>();
        }

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
                AttachRopePart(hit);
            }
            else {
                Debug.Log("No anchor found.");
            }
        }

        void AttachRopePart(RaycastHit hit) {
            var anchor = hit.collider.gameObject.GetComponent<AnchorComponent>();
            anchor.SetAttachPoint(hit.point);
            connectedAnchors.Add(anchor);
            if (!isConnected) {
                isConnected = true;
                //create rope
                rope = Instantiate(ropePrefab);
                rope.name = "New Rope!";
                rope.dynamicSplitMerge = true;
                Debug.Log(rope.name);
                ropes.Insert(0, rope);
                if (ropes.Count > ropeLimit) {
                    //destroy excess ropes
                    var lastRope = ropes.Last<Cable>();
                    ropes.Remove(lastRope);
                    ropes.TrimExcess();
                    Destroy(lastRope.gameObject);
                    Debug.Log("destroy rope");
                }
                ropeSolver.cables = ropes.ToArray();

                Cable.Link self = new Cable.Link();
                self.body = cableAttachPoint;
                self.type = Cable.Link.LinkType.Attachment;
                rope.links.Add(self);

                Cable.Link anchorPoint = new Cable.Link();
                anchorPoint.body = anchor.cablePoint;
                anchorPoint.type = Cable.Link.LinkType.Attachment;
                rope.links.Add(anchorPoint);

            }
            else {
                isConnected = false;

                Cable.Link anchorPointOne = rope.links[1];

                Cable.Link anchorPointTwo = new Cable.Link();
                anchorPointTwo.body = anchor.cablePoint;
                anchorPointTwo.type = Cable.Link.LinkType.Attachment;

                ropes.Remove(rope);
                ropes.TrimExcess();
                Destroy(rope.gameObject);

                rope = Instantiate(ropePrefab);
                rope.name = "New Rope!";
                rope.dynamicSplitMerge = true;
                ropes.Insert(0, rope);

                rope.links.Add(anchorPointOne);
                rope.links.Add(anchorPointTwo);
                ropeSolver.cables = ropes.ToArray();
                
            }
        }

        void DisconnectRope() {
            isConnected = false;
            if (rope == null) return;
            Destroy(rope.gameObject);
            ropes.TrimExcess();
            connectedAnchors.Clear();
            connectedAnchors.TrimExcess();
            ropeSolver.cables = ropes.ToArray();
        }
    }
}


