using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public GameObject bulletPrefab;

    public List<Vector3> bulletSpawnPoints;

    public float playerShootRate;

    public AudioClip shootSound;

    public void Shoot(int rampingTier, Transform playerTransform)
    {
        if (bulletPrefab)
        {
            AudioController.Instance.PlaySound(shootSound, 0.5f);

            switch (rampingTier)
            {
                case 3:
                    SpawnBullet(0, 0, playerTransform);
                    SpawnBullet(1, 10, playerTransform);
                    SpawnBullet(2, -10, playerTransform);
                    SpawnBullet(3, 20, playerTransform);
                    SpawnBullet(4, -20, playerTransform);
                    break;

                case 2:
                    SpawnBullet(0, 0, playerTransform);
                    SpawnBullet(1, 10, playerTransform);
                    SpawnBullet(2, -10, playerTransform);
                    break;

                case 1:
                    SpawnBullet(3, 0, playerTransform);
                    SpawnBullet(4, 0, playerTransform);
                    break;

                case 0:
                default:
                    SpawnBullet(0, 0, playerTransform);
                    break;
            }
        }
    }

    private void SpawnBullet(int index, float rotationOffset, Transform transform)
    {
        Quaternion rotation = transform.rotation;
        if (rotationOffset != 0)
        {
            rotation *= Quaternion.Euler(0, 0, rotationOffset);
        }
        Instantiate(bulletPrefab, transform.position + bulletSpawnPoints[index], rotation);
    }
}
