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
            AudioController.Instance.PlaySound(fireSound, 0.5f);

            switch (rampingController.rampingTier)
            {
                case 3:
                    FireBullet(0);
                    FireBullet(1, 10);
                    FireBullet(2, -10);
                    FireBullet(3, 20);
                    FireBullet(4, -20);
                    break;

                case 2:
                    FireBullet(0);
                    FireBullet(1, 10);
                    FireBullet(2, -10);
                    break;

                case 1:
                    FireBullet(3);
                    FireBullet(4);
                    break;

                case 0:
                default:
                    FireBullet(0);
                    break;
            }
        }
    }

    private void FireBullet(int index, float rotationOffset = 0)
    {
        Quaternion rotation = bulletSpawnPoints[index].rotation;
        if (rotationOffset != 0)
        {
            rotation *= Quaternion.Euler(0, 0, rotationOffset);
        }
        Instantiate(bulletPrefab, bulletSpawnPoints[index].position, rotation);
    }

}
