using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Filo;

namespace Cox.ControllerProject.GoldPlayerAddons {
    public class RopeItemBehaviour : ToolBehaviour {
        [Tooltip("The maximum amount of ropes that can be in play at once.")]
        [SerializeField] int ropeLimit = 1;
        [Tooltip("The maximum distance away from an anchor point you can be on the first rope connection.")]
        [SerializeField] float maxDistanceFromPlayer = 3;
        [SerializeField] Transform firePoint;
        bool isConnected = false;
        [SerializeField] RopeComponent ropePrefab;
        [SerializeField] AudioClip disconnectSound, deploySound;
        [SerializeField] AudioSource audioSource;


        RopeComponent activeRope = null;
        List<Cable> ropes = new List<Cable>();

        CableSolver ropeSolver = null;

        private void Awake() {
            ropeSolver = FindObjectOfType<CableSolver>();
        }

        public override void PrimaryFunction(GameObject crosshair) {
            //Use camera and crosshair raycast for really accurate aiming
            var crosshairPos = Camera.main.ScreenToWorldPoint(new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 1));
            var distance = Camera.main.transform.forward * 1000;
            Physics.Raycast(crosshairPos, distance, out RaycastHit hit);
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
                PlayAudio(deploySound);
                isConnected = true;
                //create rope
                activeRope = Instantiate(ropePrefab);
                activeRope.name = "rope";
                ropes.Insert(0, activeRope.cable);
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
                activeRope.AttachFirstPoint(firePoint, hit.point);
            }
            else {
                PlayAudio(deploySound);
                isConnected = false;
                activeRope.AttachSecondPoint(hit.point);
                activeRope = null;
            }
        }

        private void Update() {
            //if (activeRope == null) return;
            //Debug.Log(Vector3.Distance(firePoint.position, activeRope.pointA.position));
            if(activeRope != null && Vector3.Distance(firePoint.position, activeRope.pointB.position) > maxDistanceFromPlayer) {
                DisconnectRope();
            }
        }

        void PlayAudio(AudioClip clip) {
            audioSource.clip = clip;
            audioSource.Play();
        }

        void DisconnectRope() {
            PlayAudio(disconnectSound);
            Debug.Log("Disconnected");
            isConnected = false;
            if (activeRope == null) return;
            ropes.Remove(activeRope.cable);
            ropes.TrimExcess();
            Destroy(activeRope.gameObject);
            ropeSolver.cables = ropes.ToArray();
        }
    }
}


