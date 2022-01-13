using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Orient the AR text towards the camera
 */ 
public class RotateARPanel : MonoBehaviour
{
    Camera cameraToLookAt;

    // Use this for initialization
    void Start()
    {
        cameraToLookAt = Camera.current; //Camera.main does not work
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(cameraToLookAt.transform);
        Vector3 lookAt = cameraToLookAt.transform.forward;
        transform.rotation = Quaternion.LookRotation(lookAt);
    }
}
