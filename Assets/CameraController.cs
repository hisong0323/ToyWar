using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int moveSpeed = 10;
    private int zoomSpeed = 1000;

    private void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * moveSpeed * Time.deltaTime);
    }

    private void Zoom()
    {
        float wheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        Debug.Log(wheelInput);
        transform.Translate(transform.forward * zoomSpeed * wheelInput * Time.deltaTime);
    }
}
