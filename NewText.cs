using UnityEngine;

public class NewText: MonoBehaviour
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
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }
}
