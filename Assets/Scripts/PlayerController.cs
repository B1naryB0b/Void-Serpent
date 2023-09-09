using UnityEngine;
using System.Collections;

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
    public Transform bulletSpawnPoint;       // Point from where the bullet will be spawned
    public GameObject thrustTrail;           // Reference to the thrust trail GameObject
    public float fireRate = 0.5f;            // Rate at which the player can shoot
    public float thrustTrailMaxScale = 1.0f;  // Maximum scale for the thrust trail


    private float currentThrust = 0.0f;      // Current thrust value
    private float nextFireTime = 0.0f;       // Time when the player can shoot next
    private Rigidbody2D rb;                  // Player's Rigidbody2D component

    public int maxLives = 3;  // Maximum lives of the player
    private int currentLives; // Current lives of the player
    public GameObject lifeIconPrefab; // Prefab for the life icon UI
    public Transform lifeIconContainer; // UI container for the life icons
    public float livesUISpacingOffset;
    public float livesUIHorizontalAdjustmentOffset;

    public GameObject explosionPrefab;

    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 2.0f;  // Duration of invulnerability in seconds

    public AudioSource thrustSound;

    public enum MovementMode
    {
        Inertia,
        Linear,
        Tank
    }

    private MovementMode movementMode;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLives = maxLives;
        UpdateLifeUI();
        print("Called start");
        movementMode = MovementMode.Inertia;
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
    }

    // Handle thrust input and apply force
    private void UpdateMovement()
    {
        print(movementMode);
        if (movementMode != MovementMode.Linear)
        {
            if (Input.GetKey(KeyCode.W))
            {
                currentThrust = Mathf.Clamp(currentThrust + thrustIncreaseRate * Time.fixedDeltaTime, 0, maxThrust);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                currentThrust = Mathf.Clamp(currentThrust - thrustDecreaseRate * Time.fixedDeltaTime, 0, maxThrust);
            }

            // Apply force in the forward direction based on current thrust
            rb.AddForce(transform.up * currentThrust);

            ClampVelocity();
        }
        else if (movementMode == MovementMode.Linear)
        {
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            float verticalMove = Input.GetAxisRaw("Vertical");

            // Set the velocity directly based on input and maxThrust
            rb.velocity = Vector2.Lerp( rb.velocity, new Vector2(horizontalMove, verticalMove).normalized * linearVelocity, Time.deltaTime * thrustIncreaseRate);

            currentThrust = maxThrust * Mathf.Abs(horizontalMove * verticalMove);
        }
        else
        {
            Debug.Log("Invalid movement mode");
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * 0.9f, thrustDecreaseRate * Time.fixedDeltaTime);
            currentThrust = 0f;
        }
    }


    // Rotate the spaceship to face the direction of the mouse
    private void RotateTowardsMouse()
    {
        if (movementMode != MovementMode.Tank)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Subtracting 90 degrees to align with the up direction
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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

    // Update the y-scale of the thrust trail based on the current thrust
    private void UpdateThrustTrail()
    {
        if (thrustTrail)
        {
            float originalYScale = thrustTrail.transform.localScale.y;
            float scaleValue = Mathf.Lerp(0, thrustTrailMaxScale, currentThrust / maxThrust);

            thrustSound.volume = (currentThrust / maxThrust) * 0.1f;

            // Calculate the difference in scale
            float deltaYScale = scaleValue - originalYScale;

            // Adjust the position of the thrust trail based on the change in scale
            Vector3 newPosition = thrustTrail.transform.localPosition;
            newPosition.y -= deltaYScale / 2.0f;
            thrustTrail.transform.localPosition = newPosition;

            // Update the y-scale
            thrustTrail.transform.localScale = new Vector3(thrustTrail.transform.localScale.x, scaleValue, thrustTrail.transform.localScale.z);

            if (movementMode == MovementMode.Linear)
            {
                float angle = Mathf.Atan2(rb.velocity.x, rb.velocity.y) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                thrustTrail.transform.rotation = Quaternion.RotateTowards(thrustTrail.transform.rotation, targetRotation, rotationSpeed);
            }
        }
        else
        {
            thrustSound.volume = 0f;

        }
    }



    // Shoot bullets in the forward direction
    private void Shoot()
    {
        if (bulletPrefab && bulletSpawnPoint)
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    // Reduce player's health by the given damage
    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        currentLives -= damage;
        UpdateLifeUI();
        Debug.Log(currentLives);

        if (currentLives <= 0)
        {
            // Player is out of lives. Handle game over logic here.
            Debug.Log("Game Over!");
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            // Start invulnerability coroutine
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    public void UpdateMovementMode(int modeCode)
    {
        if (modeCode == 0)
        {
            movementMode = MovementMode.Inertia;
        }
        else if (modeCode == 1)
        {
            movementMode = MovementMode.Linear;
        }
        else if (modeCode == 2)
        {
            movementMode = MovementMode.Tank;
        }
        else
        {
            movementMode = MovementMode.Inertia;
        }

        print(movementMode);
    }


    private void UpdateLifeUI()
    {
        // Destroy all existing life icons
        foreach (Transform child in lifeIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate life icons based on current lives
        for (int i = 0; i < currentLives; i++)
        {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeIconContainer.transform); // Instantiates and sets the parent in one step
            lifeIcon.transform.localPosition = new Vector3(i * livesUISpacingOffset - livesUIHorizontalAdjustmentOffset, 0, 0);
        }
    }


    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        // Optionally, you can add visual feedback for invulnerability, like blinking the player sprite.

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
        // Optionally, revert any visual feedback for invulnerability.
    }


}
