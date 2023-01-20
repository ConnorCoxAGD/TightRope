using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Filo;

public class AnchorComponent : MonoBehaviour
{
    [HideInInspector] public CablePoint cablePoint;
    private void Awake() {
        cablePoint = GetComponentInChildren<CablePoint>();
    }
    public void SetAttachPoint(Vector3 position) {
        cablePoint.gameObject.transform.position = position;
    }
}
