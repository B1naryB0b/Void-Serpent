using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Member Variables
    public int lives = 3; // Number of lives the enemy has

    public float speed = 3.0f;               // Speed of the enemy spaceship
    public float fireRate = 1.0f;            // Rate at which the enemy can shoot\
    public float fireRateVariance = 0.1f;
    public float aimError = 5.0f;  // Maximum degrees of aiming error

    public GameObject bulletPrefab;          // Prefab for the enemy's bullet
    public Transform bulletSpawnPoint;       // Point from where the bullet will be spawned
    public Transform target;                 // Reference to the player (as the target)
    public float rotationLerpSpeed = 0.1f;  // Speed of rotation smoothing
    public GameObject explosionPrefab;
    public AudioClip explosionClip;


    // Movement pattern variables
    public float amplitude = 1.0f;           // Amplitude of the sinusoidal movement
    public float frequency = 1.0f;           // Frequency of the sinusoidal movement


    private float nextFireTime;      // Time when the enemy can shoot next
    private Vector3 initialPosition;         // Initial position for sinusoidal movement

    private void Start()
    {
        initialPosition = transform.position;
        nextFireTime = fireRate + Random.Range(-fireRateVariance, fireRateVariance);
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsTarget();
        Move();
        if (Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate + Random.Range(-fireRateVariance, fireRateVariance);
        }
    }

    // Rotate towards the player with a degree of error and smooth rotation
    private void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Subtracting 90 degrees to align with the up direction

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    }



    // Complex AI movement, possibly random or following a pattern
    private void Move()
    {
        if (target == null) return;

        // Move forward in the direction the enemy is facing
        transform.position += transform.up * speed * Time.deltaTime;

        // Apply sinusoidal movement along the Y-axis
        transform.position = new Vector3(transform.position.x, initialPosition.y + amplitude * Mathf.Sin(Time.time * frequency), transform.position.z);
    }

    private void Shoot()
    {
        if (bulletPrefab && bulletSpawnPoint)
        {
            // Introduce aiming error
            float errorOffset = Random.Range(-aimError, aimError);
            Quaternion bulletAngle = Quaternion.Euler(bulletSpawnPoint.eulerAngles.x, bulletSpawnPoint.eulerAngles.y, bulletSpawnPoint.eulerAngles.z + errorOffset);
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletAngle);
        }
    }



    // Reduce enemy's health by the given damage
    public void TakeDamage(int damage)
    {
        lives -= damage;

        // Update the visual representation of lives (if any)
        UpdateLivesDisplay();

        // Check if the enemy is out of lives
        if (lives <= 0)
        {
            Debug.Log($"Enemy {gameObject.name} destroyed!");
            AudioController.Instance.PlaySound(explosionClip, 0.5f);
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Enemy {gameObject.name} took {damage} damage! Remaining lives: {lives}");
        }
    }

    private void UpdateLivesDisplay()
    {
        // For now, we'll just log the remaining lives
        Debug.Log($"Enemy {gameObject.name} lives: {lives}");
    }

}
