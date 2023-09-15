using UnityEngine;

public class AdvancedCameraController : MonoBehaviour
{
    public Transform player;            // The player that the camera should consider
    public float smoothSpeed = 0.125f;  // The speed of the camera's smooth follow
    public Vector3 offset;              // Offset from the calculated position
    [Range(0f, 1f)] public float focusWeight = 0.5f; // Weight for focus between player (0) and mouse (1)

    private void LateUpdate()
    {
        if (player == null) return;

        // Get the world position of the mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // Calculate a position between the player and the mouse based on focusWeight
        Vector3 focusPosition = Vector3.Lerp(player.position, mousePosition, focusWeight);

        // Calculate desired position with offset
        Vector3 desiredPosition = focusPosition + offset;

        // Lerp between the current camera position and the desired position for smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Update the camera's position
        transform.position = smoothedPosition;
    }

}
