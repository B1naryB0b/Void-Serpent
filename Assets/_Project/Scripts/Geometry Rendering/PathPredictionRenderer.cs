using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PathPredictionRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Rigidbody2D rb;
    public int lineSegmentCount = 20;
    public float predictionTime = 2.0f;

    private PlayerController playerController;

    private void Awake() // Changed from Reset to Awake
    {
        // Ensure the line renderer and rigidbody are set up
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        // Make sure the PlayerController component exists
        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on " + gameObject.name);
        }
    }

    private void Start()
    {
        lineRenderer.positionCount = lineSegmentCount;
    }

    private void Update()
    {
        DrawPath();
    }

    private void DrawPath()
    {
        Vector3[] linePoints = new Vector3[lineSegmentCount];
        Vector2 currentVelocity = rb.velocity; // Start with the current velocity
        Vector2 thrustAcceleration = Vector2.zero; // Initialize thrust acceleration

        // Check if the player is applying thrust
        if (Input.GetKey(KeyCode.W) && playerController != null)
        {
            // Convert thrust force to acceleration (Force divided by Mass)
            thrustAcceleration = playerController.CurrentThrust / rb.mass;
        }

        // Start at the current position
        Vector2 currentPosition = transform.position;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            // Calculate the time step for this segment
            float simulationStep = (i + 1) / (float)lineSegmentCount * predictionTime;

            // Apply the acceleration to the velocity
            currentVelocity += thrustAcceleration * Time.fixedDeltaTime; // Assuming a fixed time step for simulation

            // Calculate the new position based on the current velocity and time step
            currentPosition += currentVelocity * Time.fixedDeltaTime + 0.5f * thrustAcceleration * Mathf.Pow(Time.fixedDeltaTime, 2);

            // Add the new position to the line points array
            linePoints[i] = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
        }

        // Set the positions on the line renderer
        lineRenderer.SetPositions(linePoints);
    }





}
