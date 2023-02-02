using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerExtras : MonoBehaviour
{
    private CameraMovement cameraMovement;
    // Start is called before the first frame update
    void Awake()
    {
        cameraMovement = GetComponentInChildren<CameraMovement>();
        if(cameraMovement == null) {
            Debug.LogWarning($"No CameraMovement component found for {gameObject.name}{this}.");
        }
        cameraMovement.SetPlayerTransform(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
