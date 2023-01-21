using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Filo;

public class RopeComponent : MonoBehaviour
{
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

    private IEnumerator FinishSetup() {
        yield return new WaitForEndOfFrame();
        this.gameObject.AddComponent<BoxCollider>();
    }
}
