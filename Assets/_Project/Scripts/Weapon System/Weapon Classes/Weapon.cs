using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : ScriptableObject
{
    public GameObject bulletPrefab;
    public List<Vector3> bulletSpawnPoints;
    public float playerShootRate;
    public float recoil;
    public AudioClip shootSound;

    /// <summary>
    /// The shoot function called by the player when they press the attack input.
    /// </summary>
    /// <param name="rampingTier">The current ramping tier.</param>
    /// <param name="playerTransform">The player transform.</param>
    public abstract void Shoot(int rampingTier, Transform playerTransform);

    protected void SpawnBullet(int index, float rotationOffset, Transform playerTransform)
    {
        if (index < 0 || index >= bulletSpawnPoints.Count)
        {
            Debug.LogError("SpawnBullet index out of range.");
            return;
        }

        Quaternion rotation = playerTransform.rotation;
        if (rotationOffset != 0)
        {
            rotation *= Quaternion.Euler(0, 0, rotationOffset);
        }
        Instantiate(bulletPrefab, playerTransform.position + bulletSpawnPoints[index], rotation);
    }
}
