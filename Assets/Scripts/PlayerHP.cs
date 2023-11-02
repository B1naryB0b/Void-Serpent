using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    [SerializeField] private int _maxLives;

    private int _currentLives;

    [SerializeField] private GameObject _lifeIconPrefab;
    [SerializeField] private Transform _lifeIconContainer;
    [SerializeField] private float _livesUISpacingOffset;
    [SerializeField] private float _livesUIHorizontalAdjustmentOffset;


    [SerializeField] private int _maxShield;

    private int _currentShield;

    [SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private float _shieldedInvulnerabilityDuration;
    [SerializeField] private float _shieldRegenDelay;
    [SerializeField] private float _flashDuration;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private SpriteRenderer _shieldSprite;

    private bool _shielded = false;

    [SerializeField] private float _invulnerabilityDuration;

    private bool _isInvulnerable = false;

    private void Start()
    {
        _currentLives = _maxLives;
        _shielded = true;
        UpdateLifeUI();
    }

    public void TakeDamage(int damage)
    {
        ScreenShaker.Instance.Shake(damage / 2f);

        if (_isInvulnerable)
            return;

        if (_shielded)
        {
            _currentShield -= damage;
            if (_currentShield <= 0)
            {
                _shielded = false;
                StartCoroutine(Co_FlashAndFadeShield());
                StartCoroutine(Co_InvulnerabilityTime(_shieldedInvulnerabilityDuration));
                StartCoroutine(Co_RegenShield());
            }
        }
        else
        {
            _currentLives -= damage;
            UpdateLifeUI();
            Debug.Log(_currentLives);

            if (_currentLives <= 0)
            {
                Debug.Log("Game Over!");
                Instantiate(_explosionPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Co_InvulnerabilityTime(_invulnerabilityDuration));
            }
        }
    }

    private void UpdateLifeUI()
    {
        foreach (Transform child in _lifeIconContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _currentLives; i++)
        {
            GameObject lifeIcon = Instantiate(_lifeIconPrefab, _lifeIconContainer.transform);
            lifeIcon.transform.localPosition = new Vector3(i * _livesUISpacingOffset - _livesUIHorizontalAdjustmentOffset, 0, 0);
        }
    }


    private IEnumerator Co_InvulnerabilityTime(float duration)
    {
        _isInvulnerable = true;

        yield return new WaitForSeconds(duration);

        _isInvulnerable = false;
    }

    private IEnumerator Co_FlashAndFadeShield()
    {
        _shieldSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        yield return Co_ChangeSpriteColor(_shieldSprite, _fadeDuration, Color.white, Color.clear);
    }

    private IEnumerator Co_RegenShield()
    {
        yield return new WaitForSeconds(_shieldRegenDelay);
        _currentShield = _maxShield;
        _shielded = true;

        yield return Co_ChangeSpriteColor(_shieldSprite, _flashDuration, Color.clear, Color.white);

        yield return Co_ChangeSpriteColor(_shieldSprite, _fadeDuration, Color.white, Color.clear);
    }

    private IEnumerator Co_ChangeSpriteColor(SpriteRenderer spriteRenderer, float duration, Color startColor, Color endColor)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }
    }
}
