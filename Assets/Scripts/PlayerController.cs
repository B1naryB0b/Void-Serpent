using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _brakeStrength;
    [SerializeField] private float _brakeStoppingVelocity;


    private Vector2 _currentThrust = Vector2.zero;

    private Rigidbody2D _playerRb;

    private Inertial _inertialMovement;
    private Linear _linearMovement;
    private Tank _tankMovement;

    private Thrusters _thrusters;

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
        _inertialMovement = GetComponent<Inertial>();
        _linearMovement = GetComponent<Linear>();
        _tankMovement = GetComponent<Tank>();

        _thrusters = GetComponent<Thrusters>();

        _playerRb = GetComponent<Rigidbody2D>();

        movementMode = MovementMode.Linear;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotateTowardsMouse();
        _thrusters.UpdateThrustTrail(_playerRb);
    }

    private void MovePlayer()
    {
        if (movementMode == MovementMode.Inertia)
        {
            _inertialMovement.UpdateMovement(_playerRb, _currentThrust);
        }
        else if (movementMode == MovementMode.Linear)
        {
            _linearMovement.UpdateMovement(_playerRb);
        }
        else if (movementMode == MovementMode.Tank)
        {
            _tankMovement.UpdateMovement(_playerRb, _currentThrust);
        }
        else
        {
            Debug.Log("Invalid movement mode");
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Brake(_playerRb);
        }
    }

    private void Brake(Rigidbody2D rb)
    {
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * 0.9f, _brakeStrength * Time.fixedDeltaTime);
        if (rb.velocity.sqrMagnitude < _brakeStoppingVelocity)
        {
            rb.velocity = Vector2.zero;
        }
        _currentThrust = Vector2.zero;
    }

    private void RotateTowardsMouse()
    {
        if (movementMode != MovementMode.Tank)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float distanceToPlane;
            Plane playerPlane = new Plane(-Vector3.forward, transform.position);
            if (playerPlane.Raycast(ray, out distanceToPlane))
            {
                Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
                Vector3 direction = pointOnPlane - transform.position;

                direction.z = 0;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else if (movementMode == MovementMode.Tank)
        {
            transform.Rotate(new Vector3(0, 0, Input.GetAxisRaw("Horizontal") * _rotationSpeed));
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
