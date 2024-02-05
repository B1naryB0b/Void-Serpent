using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shotgun")]
public class ShotgunScript : Weapon
{
    public override void Shoot(int rampingTier, Transform playerTransform)
    {
        if(bulletPrefab)
        {
            SpawnBulletsByRampingTier(rampingTier, playerTransform);
        }
    }

    private void SpawnBulletsByRampingTier(int rampingTier, Transform playerTransform)
    {
        AudioController.Instance.PlaySound(shootSound, 0.5f);

        switch (rampingTier)
        {
            case 3:
                SpawnBullet(0, 0, playerTransform);
                SpawnBullet(1, 5, playerTransform);
                SpawnBullet(2, -5, playerTransform);
                SpawnBullet(3, 10, playerTransform);
                SpawnBullet(4, -10, playerTransform);
                SpawnBullet(3, 15, playerTransform);
                SpawnBullet(4, -15, playerTransform);
                break;

            case 2:
                SpawnBullet(0, 0, playerTransform);
                SpawnBullet(1, 5, playerTransform);
                SpawnBullet(2, -5, playerTransform);
                SpawnBullet(3, 10, playerTransform);
                SpawnBullet(4, -10, playerTransform);
                break;

            case 1:
                SpawnBullet(0, 0, playerTransform);
                SpawnBullet(3, 5, playerTransform);
                SpawnBullet(4, -5, playerTransform);
                break;

            case 0:
            default:
                SpawnBullet(3, 5, playerTransform);
                SpawnBullet(4, -5, playerTransform);
                break;
        }
    }
}
