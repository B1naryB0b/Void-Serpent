using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ModePlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float brakeStrength;
    [SerializeField] private float brakeStoppingVelocity;
    [SerializeField] private float tiltClampAngle = 45f; // The maximum tilt angle along the Y-axis


    private Vector2 currentThrust = Vector2.zero;
    public Vector2 CurrentThrust => currentThrust != null ? currentThrust : Vector2.zero;

    private Rigidbody2D playerRb;

    private SimpleInertial inertialMovement;
    private Linear linearMovement;

    private Thrusters thrusters;

    #region Movement Modes
    private const int INERTIACODE = 0;
    private const int LINEARCODE = 1;

    public enum MovementMode
    {
        Inertia,
        Linear
    }

    private MovementMode movementMode;
    #endregion

    private void Start()
    {
        inertialMovement = GetComponent<SimpleInertial>();
        linearMovement = GetComponent<Linear>();

        thrusters = GetComponent<Thrusters>();

        playerRb = GetComponent<Rigidbody2D>();

        movementMode = MovementMode.Inertia;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        thrusters.UpdateThrustTrail(playerRb);
    }

    private void MovePlayer()
    {
        if (movementMode == MovementMode.Inertia)
        {
            inertialMovement.UpdateMovement(playerRb, currentThrust);
        }
        else if (movementMode == MovementMode.Linear)
        {
            linearMovement.UpdateMovement(playerRb);
        }
        else
        {
            Debug.Log("Invalid movement mode");
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Brake(playerRb);
        }
    }

    private void Brake(Rigidbody2D rb)
    {
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * 0.9f, brakeStrength * Time.fixedDeltaTime);
        if (rb.velocity.sqrMagnitude < brakeStoppingVelocity)
        {
            rb.velocity = Vector2.zero;
        }
        currentThrust = Vector2.zero;
    }

    private void RotatePlayer()
    {
        Quaternion rotationTowardsMouse = GetRotationTowardsMouse();
        Quaternion tiltRotation = GetTiltBasedOnAngleToMouse();
        Quaternion totalTargetRotation = rotationTowardsMouse * tiltRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, totalTargetRotation, Time.fixedDeltaTime);
    }

    private Quaternion GetRotationTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(-Vector3.forward, transform.position);
        if (playerPlane.Raycast(ray, out float distanceToPlane))
        {
            Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
            Vector3 direction = pointOnPlane - transform.position;

            direction.z = 0;

            Debug.DrawLine(transform.position, direction);

            return Quaternion.LookRotation(Vector3.forward, direction);
        }
        return transform.rotation;
    }

    private Quaternion GetTiltBasedOnAngleToMouse()
    {
        Vector3 mousePosition = GetMousePositionOnPlayerPlane();
        float signedAngle = CalculateSignedAngleToMouse(mousePosition);
        float yTilt = Mathf.Clamp(signedAngle / 2, -tiltClampAngle, tiltClampAngle);

        // Since we're in 2D, we rotate around the Z-axis to simulate tilt
        return Quaternion.AngleAxis(yTilt, Vector3.up);
    }

    private Vector3 GetMousePositionOnPlayerPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(-Vector3.forward, transform.position);
        playerPlane.Raycast(ray, out float distanceToPlane);
        return ray.GetPoint(distanceToPlane);
    }

    private float CalculateSignedAngleToMouse(Vector3 mousePosition)
    {
        Vector3 direction = (mousePosition - transform.position).normalized;
        Vector3 playerForward = transform.up;
        return Vector3.SignedAngle(playerForward, direction, Vector3.forward);
    }

    public void UpdateMovementMode(int modeCode)
    {
        switch (modeCode)
        {
            case LINEARCODE:
                movementMode = MovementMode.Linear;
                break;
            case INERTIACODE:
            default:
                movementMode = MovementMode.Inertia;
                break;
        }

        print(movementMode);
    }

}
