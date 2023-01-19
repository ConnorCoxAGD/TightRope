using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private void LateUpdate() {
        transform.position = new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2, 0);
    }
}
