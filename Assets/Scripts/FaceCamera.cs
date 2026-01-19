using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
            targetCamera.transform.rotation * Vector3.up);
    }
}