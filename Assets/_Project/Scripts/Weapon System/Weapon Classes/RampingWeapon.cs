using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/RampingWeapon")]
public class RampingWeapon : Weapon
{
    public override void Shoot(int rampingTier, Transform playerTransform)
    {
        if (bulletPrefab)
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