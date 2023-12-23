using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    [SerializeField] private int maxLives;

    private int currentLives;

    [SerializeField] private GameObject lifeIconPrefab;
    [SerializeField] private Transform lifeIconContainer;
    [SerializeField] private float livesUISpacingOffset;
    [SerializeField] private float livesUIHorizontalAdjustmentOffset;


    [SerializeField] private int maxShield;

    private int currentShield;

    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private float shieldedInvulnerabilityDuration;
    [SerializeField] private float shieldRegenDelay;
    [SerializeField] private float flashDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private SpriteRenderer shieldSprite;

    private bool shielded = false;

    [SerializeField] private float invulnerabilityDuration;

    private bool isInvulnerable = false;

    private void Start()
    {
        currentLives = maxLives;
        shielded = true;
        UpdateLifeUI();
    }

    public void TakeDamage(int damage)
    {
        ScreenShaker.Instance.Shake(damage / 2f);

        if (isInvulnerable)
            return;

        if (shielded)
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                shielded = false;
                StartCoroutine(Co_FlashAndFadeShield());
                StartCoroutine(Co_InvulnerabilityTime(shieldedInvulnerabilityDuration));
                StartCoroutine(Co_RegenShield());
            }
        }
        else
        {
            currentLives -= damage;
            UpdateLifeUI();
            Debug.Log(currentLives);

            if (currentLives <= 0)
            {
                Debug.Log("Game Over!");
                Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Co_InvulnerabilityTime(invulnerabilityDuration));
            }
        }
    }

    private void UpdateLifeUI()
    {
        foreach (Transform child in lifeIconContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentLives; i++)
        {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeIconContainer.transform);
            lifeIcon.transform.localPosition = new Vector3(i * livesUISpacingOffset - livesUIHorizontalAdjustmentOffset, 0, 0);
        }
    }


    private IEnumerator Co_InvulnerabilityTime(float duration)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(duration);

        isInvulnerable = false;
    }

    private IEnumerator Co_FlashAndFadeShield()
    {
        shieldSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        yield return Co_ChangeSpriteColor(shieldSprite, fadeDuration, Color.white, Color.clear);
    }

    private IEnumerator Co_RegenShield()
    {
        yield return new WaitForSeconds(shieldRegenDelay);
        currentShield = maxShield;
        shielded = true;

        yield return Co_ChangeSpriteColor(shieldSprite, flashDuration, Color.clear, Color.white);

        yield return Co_ChangeSpriteColor(shieldSprite, fadeDuration, Color.white, Color.clear);
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
