using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int lives;

    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireRateVariance;
    [SerializeField] private float _fireRange;
    [SerializeField] private float _aimErrorAngle;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _explosionClip;

    [SerializeField] private GameObject _collectablePrefab;

    [SerializeField] private AudioClip _fireSound;

    private float _nextFireTime;

    private Transform _target;

    private RampingController _rampingController;

    private bool _playerInCrosshair;

    private void Start()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (playerController != null )
        {
            _target = playerController.transform;
        }
        _rampingController = FindObjectOfType<RampingController>().GetComponent<RampingController>();

        print(_target.name);
        print(_rampingController.name);

        _nextFireTime = (1/_fireRate) + Random.Range(-_fireRateVariance, _fireRateVariance);

        StartCoroutine(Co_FireLoop());

    }

    private bool CrosshairCheck()
    {
        if (_target == null) return false;

        int layerMask = 1 << LayerMask.NameToLayer("Player");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _fireRange, layerMask);
        return hit.collider != null;
    }

    private IEnumerator Co_FireLoop()
    {
        yield return new WaitForSeconds(_nextFireTime);

        while (true)
        {
            _playerInCrosshair = CrosshairCheck();

            if (_playerInCrosshair)
            {
                Fire();
                _nextFireTime = (1 / _fireRate) + Random.Range(-_fireRateVariance, _fireRateVariance);
            }

            yield return new WaitForSeconds(_nextFireTime);
        }
    }


    private void Fire()
    {
        if (_bulletPrefab && _bulletSpawnPoint)
        {
            float errorOffset = Random.Range(-_aimErrorAngle, _aimErrorAngle);
            Quaternion bulletAngle = Quaternion.Euler(_bulletSpawnPoint.eulerAngles.x, _bulletSpawnPoint.eulerAngles.y, _bulletSpawnPoint.eulerAngles.z + errorOffset);
            Instantiate(_bulletPrefab, _bulletSpawnPoint.position, bulletAngle);

            float volume = 0.1f;
            AudioController.Instance.PlaySound(_fireSound, volume);

        }
    }


    public void TakeDamage(int damage)
    {
        lives -= damage;

        UpdateLivesDisplay();

        if (lives <= 0)
        {
            Debug.Log($"Enemy {gameObject.name} destroyed!");
            AudioController.Instance.PlaySound(_explosionClip, 0.15f);

            Instantiate(_explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Instantiate(_collectablePrefab, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Enemy {gameObject.name} took {damage} damage! Remaining lives: {lives}");
        }
    }

    private void UpdateLivesDisplay()
    {
        Debug.Log($"Enemy {gameObject.name} lives: {lives}");
    }

}
