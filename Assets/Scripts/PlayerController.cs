using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // Member Variables
    public float maxThrust = 10.0f;          // Maximum thrust force
    public float thrustIncreaseRate = 2.0f;  // Rate at which thrust increases when pressing W
    public float thrustDecreaseRate = 2.0f;  // Rate at which thrust decreases when pressing S
    public float rotationSpeed = 200.0f;     // Speed of rotation towards the mouse
    public float terminalVelocity = 20.0f;   // Maximum speed the player can reach

    public float linearVelocity = 30.0f;


    public GameObject bulletPrefab;          // Prefab for the player's bullet
    public List<Transform> bulletSpawnPoints;       // Point from where the bullet will be spawned

    public GameObject verticalThrustTrail;
    public GameObject horizontalThrustTrail;
    public float fireRate = 0.5f;            // Rate at which the player can shoot
    public float thrustTrailMaxScale = 1.0f;  // Maximum scale for the thrust trail

    private Vector2 currentThrust = Vector2.zero;      // Current thrust value

    private float nextFireTime = 0.0f;       // Time when the player can shoot next
    private Rigidbody2D rb;                  // Player's Rigidbody2D component

    public AudioSource[] thrustSounds;

    public Slider verticalThrustSlider;
    public Slider horizontalThrustSlider;

    public RampingController rampingController;

    public AudioClip fireSound;         // Sound effect when bullet is fired


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
        rb = GetComponent<Rigidbody2D>();
        print("Called start");
        movementMode = MovementMode.Linear;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
        RotateTowardsMouse();
        UpdateThrustTrail();
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        verticalThrustSlider.value = Mathf.Abs(currentThrust.y / maxThrust);
        horizontalThrustSlider.value = Mathf.Abs(currentThrust.x / maxThrust);

    }

    // Handle thrust input and apply force
    private void UpdateMovement()
    {
        //print(movementMode);
        if (movementMode == MovementMode.Inertia)
        {
            InertialMovement();
        }
        else if (movementMode == MovementMode.Linear)
        {
            LinearMovement();
        }
        else if (movementMode == MovementMode.Tank)
        {
            TankMovement();
        }
        else
        {
            Debug.Log("Invalid movement mode");
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Brake();
        }
    }

    private void Brake()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * 0.9f, thrustDecreaseRate * Time.fixedDeltaTime);
        currentThrust = Vector2.zero;
    }

    #region Movement Systems
    private void TankMovement()
    {
        /*if (Input.GetKey(KeyCode.W))
        {
            currentThrust = Mathf.Clamp(currentThrust + thrustIncreaseRate * Time.fixedDeltaTime, 0, maxThrust);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentThrust = Mathf.Clamp(currentThrust - thrustDecreaseRate * Time.fixedDeltaTime, 0, maxThrust);
        }
*/
        // Apply force in the forward direction based on current thrust
        rb.AddForce(transform.up * currentThrust);

        ClampVelocity();
    }

    private void LinearMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        // Set the velocity directly based on input and maxThrust
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(horizontalMove, verticalMove).normalized * linearVelocity, Time.deltaTime * thrustIncreaseRate);
        }

        //currentThrust = maxThrust * Mathf.Abs(horizontalMove * verticalMove);
    }

    private bool canChangeVerticalDirection = true;
    private bool canChangeHorizontalDirection = true;

    private void InertialMovement()
    {
        currentThrust.y = AdjustThrust(currentThrust.y, KeyCode.W, KeyCode.S, canChangeVerticalDirection, VerticalDirectionChangeDelay);
        currentThrust.x = AdjustThrust(currentThrust.x, KeyCode.D, KeyCode.A, canChangeHorizontalDirection, HorizontalDirectionChangeDelay);

        // Apply force based on thrust values
        rb.AddForce(transform.up * currentThrust.y + transform.right * currentThrust.x);

        ClampVelocity();
    }

    private float AdjustThrust(float current, KeyCode positiveKey, KeyCode negativeKey, bool canChangeDirection, Func<IEnumerator> delayMethod)
    {
        if (Input.GetKey(positiveKey) && canChangeDirection)
        {
            if (current < 0 && !Mathf.Approximately(current, 0f))
            {
                current = DecreaseToZero(current, delayMethod);
            }
            else
            {
                current = Mathf.Clamp(current + thrustIncreaseRate * Time.fixedDeltaTime, 0, maxThrust);
            }
        }
        else if (Input.GetKey(negativeKey) && canChangeDirection)
        {
            if (current > 0 && !Mathf.Approximately(current, 0f))
            {
                current = DecreaseToZero(current, delayMethod);
            }
            else
            {
                current = Mathf.Clamp(current - thrustIncreaseRate * Time.fixedDeltaTime, -maxThrust, 0);
            }
        }
        return current;
    }

    private float DecreaseToZero(float current, Func<IEnumerator> delayMethod)
    {
        current = Mathf.MoveTowards(current, 0, thrustDecreaseRate * Time.fixedDeltaTime);
        if (Mathf.Approximately(current, 0f))
        {
            StartCoroutine(delayMethod());
        }
        return current;
    }

    private IEnumerator VerticalDirectionChangeDelay()
    {
        canChangeVerticalDirection = false;
        yield return new WaitForSeconds(0.3f);
        canChangeVerticalDirection = true;
    }

    private IEnumerator HorizontalDirectionChangeDelay()
    {
        canChangeHorizontalDirection = false;
        yield return new WaitForSeconds(0.3f);
        canChangeHorizontalDirection = true;
    }
    #endregion

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



    // Clamp the player's velocity to the terminal velocity
    private void ClampVelocity()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalVelocity);
    }

    // Modify the UpdateThrustTrail function
    private void UpdateThrustTrail()
    {
        UpdateThrustTrailDirection(currentThrust.y, verticalThrustTrail, Vector3.up, thrustSounds[0]);
        UpdateThrustTrailDirection(currentThrust.x, horizontalThrustTrail, Vector3.right, thrustSounds[1]);
    }

    private void UpdateThrustTrailDirection(float thrustDirectionValue, GameObject thrustTrailObj, Vector3 movementDirection, AudioSource thrustAudioSource)
    {
        if (thrustTrailObj)
        {
            float currentYScale = Mathf.Abs(thrustTrailObj.transform.localScale.y);
            float targetYScale = Mathf.Lerp(0, thrustTrailMaxScale, Mathf.Abs(thrustDirectionValue) / maxThrust);

            thrustAudioSource.volume = (Mathf.Abs(thrustDirectionValue) / maxThrust) * 0.1f;

            // Calculate the change in scale
            float scaleChangeY = targetYScale - currentYScale;

            // Adjust the position of the thrust trail based on the change in scale and direction
            Vector3 adjustedPosition = thrustTrailObj.transform.localPosition;

            if (movementDirection == Vector3.up || movementDirection == Vector3.down)
            {
                adjustedPosition.y -= scaleChangeY / 2.0f * Mathf.Sign(thrustDirectionValue);
            }
            else // if movement direction is horizontal (right or left)
            {
                adjustedPosition.x -= scaleChangeY / 2.0f * Mathf.Sign(thrustDirectionValue);
            }

            thrustTrailObj.transform.localPosition = adjustedPosition;

            // Update the y-scale based on the thrust direction
            thrustTrailObj.transform.localScale = new Vector3(thrustTrailObj.transform.localScale.x, targetYScale * Mathf.Sign(thrustDirectionValue), thrustTrailObj.transform.localScale.z);

            if (movementMode == MovementMode.Linear)
            {
                // Determine the rotation angle based on movement direction
                float rotationAngle;
                if (movementDirection == Vector3.up)
                {
                    rotationAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg - 90f;
                }
                else // if movement direction is horizontal
                {
                    rotationAngle = Mathf.Atan2(-movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                }
                Quaternion desiredRotation = Quaternion.Euler(0, 0, rotationAngle);
                thrustTrailObj.transform.rotation = Quaternion.RotateTowards(thrustTrailObj.transform.rotation, desiredRotation, rotationSpeed);
            }
        }
        else
        {
            thrustAudioSource.volume = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab && transform)
        {
            float volume = 1f;
            AudioController.Instance.PlaySound(fireSound, volume);

            switch (rampingController.rampingTier)
            {
                case 3:
                    Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
                    Instantiate(bulletPrefab, bulletSpawnPoints[1].position, bulletSpawnPoints[1].rotation * Quaternion.Euler(0, 0, 10));
                    Instantiate(bulletPrefab, bulletSpawnPoints[2].position, bulletSpawnPoints[2].rotation * Quaternion.Euler(0, 0, -10));
                    Instantiate(bulletPrefab, bulletSpawnPoints[3].position, bulletSpawnPoints[3].rotation * Quaternion.Euler(0, 0, 20));
                    Instantiate(bulletPrefab, bulletSpawnPoints[4].position, bulletSpawnPoints[4].rotation * Quaternion.Euler(0, 0, -20));
                    break;

                case 2:
                    Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
                    Instantiate(bulletPrefab, bulletSpawnPoints[1].position, bulletSpawnPoints[1].rotation * Quaternion.Euler(0, 0, 10));
                    Instantiate(bulletPrefab, bulletSpawnPoints[2].position, bulletSpawnPoints[2].rotation * Quaternion.Euler(0, 0, -10));
                    break;

                case 1:
                    Instantiate(bulletPrefab, bulletSpawnPoints[3].position, bulletSpawnPoints[3].rotation);
                    Instantiate(bulletPrefab, bulletSpawnPoints[4].position, bulletSpawnPoints[4].rotation);
                    break;

                case 0:
                default:
                    Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
                    break;
            }
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
