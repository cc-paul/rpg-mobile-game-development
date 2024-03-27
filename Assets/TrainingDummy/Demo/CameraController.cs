using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5.0f; // Speed of camera movement
    public float rotationSpeed = 3.0f; // Speed of camera rotation

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Update()
    {
        // Camera Movement Controls
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput) * movementSpeed * Time.deltaTime;
        transform.Translate(moveDirection);

        // Camera Rotation Controls
        
            rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Clamp vertical rotation

            transform.rotation = Quaternion.Euler(0, rotationX, 0);
        
    }
}
