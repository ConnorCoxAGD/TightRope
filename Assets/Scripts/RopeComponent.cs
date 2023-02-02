using System.Collections;
using UnityEngine;
using Filo;
namespace cox.ControllerProject.GoldPlayerAddons {

    public class RopeComponent : MonoBehaviour {
        public Transform pointA, pointB;
        [HideInInspector] public Cable cable;

        private void Awake() {
            cable = GetComponent<Cable>();
        }

        public void AttachFirstPoint(Transform parentTransform, Vector3 connectedPoint) {
            pointA.position = parentTransform.position;
            pointA.SetParent(parentTransform);
            pointB.position = connectedPoint;
        }

        public void AttachSecondPoint(Vector3 connectedPoint) {
            pointA.SetParent(null);
            pointA.position = connectedPoint;

            StartCoroutine(FinishSetup());
        }

        public Transform OnRopeCameraCheck() {
            var a = Camera.main.WorldToScreenPoint(pointA.position).z;
            var b = Camera.main.WorldToScreenPoint(pointB.position).z;

            if (b < 0) { return pointA; }
            else if (a < 0) { return pointB; }
            else { return null; }
        }

        private IEnumerator FinishSetup() {
            yield return new WaitForEndOfFrame();
            this.gameObject.AddComponent<MeshCollider>();
            FindObjectOfType<RopeMovement>().rope = this; //inefficient, but easily replaced
        }
    }
}
