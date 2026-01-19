using UnityEngine;

public class MouseMovementScript : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lookSpeed = 2f;

    private float rotationX;
    private float rotationY;

    private void Update()
    {
        // Only active in Play mode inside Editor
        if (!Application.isEditor) return;

        // Mouse look
        if (Input.GetMouseButton(1)) // Hold right-click
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -80, 80);
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);
            Debug.Log("Mouse moved!!");
        }

        // WASD movement
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * moveSpeed * Time.deltaTime);
    }
}