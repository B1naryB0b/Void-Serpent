using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : MonoBehaviour
{
    [SerializeField] private float maxLinearVelocity = 20.0f;
    [SerializeField] private float accelerationCurveMultiplier = 10.0f;
    [SerializeField] private AnimationCurve accelerationCurve; // Animation curve for acceleration
    private float currentAccelerationTime = 0.0f; // To keep track of the time for the curve
    private Vector2 previousInput; // To store the previous frame's input

    public void UpdateMovement(Rigidbody2D rb)
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 currentInput = new Vector2(horizontalMove, verticalMove);

        // Check if the input has changed
        if (currentInput != previousInput)
        {
            currentAccelerationTime = 0.0f;
        }

        // If any of the movement keys are pressed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Increase the current time for the curve
            currentAccelerationTime += Time.deltaTime;
            // Get the acceleration value from the curve
            float acceleration = accelerationCurve.Evaluate(currentAccelerationTime) * accelerationCurveMultiplier;
            rb.velocity = Vector2.Lerp(rb.velocity, currentInput.normalized * maxLinearVelocity, Time.deltaTime * acceleration);
        }
        else
        {
            // Reset the time for the curve when no keys are pressed
            currentAccelerationTime = 0.0f;
        }

        // Update the previous input for the next frame
        previousInput = currentInput;
    }
}
