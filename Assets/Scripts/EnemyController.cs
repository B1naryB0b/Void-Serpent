using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    // Member Variables
    public int lives = 3; // Number of lives the enemy has

    public float fireRate = 1.0f;            // Rate at which the enemy can shoot\
    public float fireRateVariance = 0.1f;
    public float fireRange;
    public float aimError;

    public GameObject bulletPrefab;          // Prefab for the enemy's bullet
    public Transform bulletSpawnPoint;       // Point from where the bullet will be spawned
    private Transform target;                 // Reference to the player (as the target)

    public GameObject explosionPrefab;
    public AudioClip explosionClip;

    public GameObject collectablePrefab;

    public AudioClip fireSound;         // Sound effect when bullet is fired

    private float nextFireTime;      // Time when the enemy can shoot next


    private RampingController rampingController;

    private bool playerInCrosshair;

    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        rampingController = FindObjectOfType<RampingController>().GetComponent<RampingController>();

        print(target.name);
        print(rampingController.name);

        nextFireTime = (1/fireRate) + Random.Range(-fireRateVariance, fireRateVariance);

        StartCoroutine(ShootingRoutine());

    }

    // Rotate towards the player with a degree of error and smooth rotation
    private bool CrosshairCheck()
    {
        if (target == null) return false;

        // Correcting the layer mask usage. This line was potentially causing the issue.
        int layerMask = 1 << LayerMask.NameToLayer("Player"); // it's crucial to shift bits for the mask, not direct use.

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, fireRange, layerMask);
        return hit.collider != null; // ensure we hit something.
    }

    private IEnumerator ShootingRoutine()
    {
        // Initial wait before the loop starts.
        yield return new WaitForSeconds(nextFireTime);

        while (true) // creates an endless loop until the enemy is destroyed or the game ends.
        {
            playerInCrosshair = CrosshairCheck();

            // If the player is in crosshair, shoot.
            if (playerInCrosshair)
            {
                Shoot();
                // Calculate the next fire time with variance.
                nextFireTime = (1 / fireRate) + Random.Range(-fireRateVariance, fireRateVariance);
            }

            // Wait for the calculated time until the next shot.
            yield return new WaitForSeconds(nextFireTime);
        }
    }


    private void Shoot()
    {
        if (bulletPrefab && bulletSpawnPoint)
        {
            // Introduce aiming error
            float errorOffset = Random.Range(-aimError, aimError);
            Quaternion bulletAngle = Quaternion.Euler(bulletSpawnPoint.eulerAngles.x, bulletSpawnPoint.eulerAngles.y, bulletSpawnPoint.eulerAngles.z + errorOffset);
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletAngle);

            float volume = 0.1f;
            AudioController.Instance.PlaySound(fireSound, volume);

        }
    }



    // Reduce enemy's health by the given damage
    public void TakeDamage(int damage)
    {
        lives -= damage;

        //rampingController.IncreaseRamping(damage);

        // Update the visual representation of lives (if any)
        UpdateLivesDisplay();

        // Check if the enemy is out of lives
        if (lives <= 0)
        {
            Debug.Log($"Enemy {gameObject.name} destroyed!");
            AudioController.Instance.PlaySound(explosionClip, 0.15f);
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Instantiate(collectablePrefab, gameObject.transform.position, Quaternion.identity );
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
