using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;          // Prefab for the player's bullet
    [SerializeField] private List<Transform> bulletSpawnPoints;       // Point from where the bullet will be spawned

    [SerializeField] private float fireRate = 0.5f;            // Rate at which the player can shoot

    [SerializeField] private AudioClip fireSound;         // Sound effect when bullet is fired

    [SerializeField] private RampingController rampingController;

    private float nextFireTime = 0.0f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void Shoot()
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
}
