using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // Member Variables
      // Rate at which thrust decreases when pressing S
    [SerializeField] private float rotationSpeed = 200.0f;     // Speed of rotation towards the mouse
    [SerializeField] private float brakeStrength = 10.0f;

    private Vector2 currentThrust = Vector2.zero;

    private Rigidbody2D playerRb;                  // Player's Rigidbody2D component

    private Inertial inertialMovement;
    private Linear linearMovement;
    private Tank tankMovement;

    private Thrusters thrusters;

    #region Movement Modes
    private const int INERTIA_CODE = 0;
    private const int LINEAR_CODE = 1;
    private const int TANK_CODE = 2;

    public enum MovementMode
    {
        Inertia,
        Linear,
        Tank
    }

    private MovementMode movementMode;
    #endregion


    private void Start()
    {
        inertialMovement = GetComponent<Inertial>();
        linearMovement = GetComponent<Linear>();
        tankMovement = GetComponent<Tank>();

        thrusters = GetComponent<Thrusters>();

        playerRb = GetComponent<Rigidbody2D>();
        movementMode = MovementMode.Linear;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
        RotateTowardsMouse();
        thrusters.UpdateThrustTrail(playerRb);
    }

    // Handle thrust input and apply force
    private void MovePlayer()
    {
        //print(movementMode);
        if (movementMode == MovementMode.Inertia)
        {
            inertialMovement.UpdateMovement(playerRb, currentThrust);
        }
        else if (movementMode == MovementMode.Linear)
        {
            linearMovement.UpdateMovement(playerRb);
        }
        else if (movementMode == MovementMode.Tank)
        {
            tankMovement.UpdateMovement(playerRb, currentThrust);
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
        currentThrust = Vector2.zero;
    }

    // Rotate the spaceship to face the direction of the mouse
    private void RotateTowardsMouse()
    {
        if (movementMode != MovementMode.Tank)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Create a plane with the normal in the negative z-direction, where the player is located.
            float distanceToPlane;
            Plane playerPlane = new Plane(-Vector3.forward, transform.position);
            if (playerPlane.Raycast(ray, out distanceToPlane))
            {
                Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
                Vector3 direction = pointOnPlane - transform.position;

                direction.z = 0;  // Keep the direction on the player's plane

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else if (movementMode == MovementMode.Tank)
        {
            transform.Rotate(new Vector3(0, 0, Input.GetAxisRaw("Horizontal") * rotationSpeed));
        }
    }

    public void UpdateMovementMode(int modeCode)
    {
        switch (modeCode)
        {
            case LINEAR_CODE:
                movementMode = MovementMode.Linear;
                break;
            case TANK_CODE:
                movementMode = MovementMode.Tank;
                break;
            case INERTIA_CODE:
            default:
                movementMode = MovementMode.Inertia;
                break;
        }

        print(movementMode);
    }

}
