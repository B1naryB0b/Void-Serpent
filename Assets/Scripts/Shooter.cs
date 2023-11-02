using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;   
    [SerializeField] private List<Transform> _bulletSpawnPoints;

    [SerializeField] private float _playerShootRate;

    [SerializeField] private AudioClip _shootSound;

    [SerializeField] private RampingController _rampingController;

    private float _nextShootTime = 0.0f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > _nextShootTime)
        {
            Shoot();
            _nextShootTime = Time.time + (1f/_playerShootRate);
        }
    }

    public void Shoot()
    {
        if (_bulletPrefab && transform)
        {
            AudioController.Instance.PlaySound(_shootSound, 0.5f);

            switch (_rampingController.CurrentRampingTier)
            {
                case 3:
                    SpawnBullet(0);
                    SpawnBullet(1, 10);
                    SpawnBullet(2, -10);
                    SpawnBullet(3, 20);
                    SpawnBullet(4, -20);
                    break;

                case 2:
                    SpawnBullet(0);
                    SpawnBullet(1, 10);
                    SpawnBullet(2, -10);
                    break;

                case 1:
                    SpawnBullet(3);
                    SpawnBullet(4);
                    break;

                case 0:
                default:
                    SpawnBullet(0);
                    break;
            }
        }
    }

    private void SpawnBullet(int index, float rotationOffset = 0)
    {
        Quaternion rotation = _bulletSpawnPoints[index].rotation;
        if (rotationOffset != 0)
        {
            rotation *= Quaternion.Euler(0, 0, rotationOffset);
        }
        Instantiate(_bulletPrefab, _bulletSpawnPoints[index].position, rotation);
    }

}
