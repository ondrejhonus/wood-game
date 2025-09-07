using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float rotationX;
    float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get input from mouse using the new Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensX * Time.deltaTime;
        float mouseY = mouseDelta.y * sensY * Time.deltaTime;

        // update rotation values
        rotationY += mouseX;
        rotationX -= mouseY;

        // clamp vertical rotation to 90 degrees up and down
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // apply rotation to camera
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
}