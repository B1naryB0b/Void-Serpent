using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private DataBank dataBank;
    private PlayerData playerData;
    private UIData uiData;

    [SerializeField] private SpriteRenderer shieldSprite;
    [SerializeField] private Transform lifeIconContainer;


    private int currentLives;
    private int currentShield;

    private bool shielded = false;
    private bool isInvulnerable = false;

    private void Start()
    {
        playerData = dataBank.playerData;
        uiData = dataBank.uiData;
        currentLives = playerData.maxLives;
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
                StartCoroutine(Co_InvulnerabilityTime(playerData.shieldedInvulnerabilityDuration));
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
                Instantiate(playerData.explosionPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Co_InvulnerabilityTime(playerData.invulnerabilityDuration));
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
            GameObject lifeIcon = Instantiate(uiData.lifeIconPrefab, lifeIconContainer.transform);
            lifeIcon.transform.localPosition = new Vector3(i * uiData.livesUISpacingOffset - uiData.livesUIHorizontalAdjustmentOffset, 0, 0);
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

        yield return Co_ChangeSpriteColor(shieldSprite, playerData.fadeDuration, Color.white, Color.clear);
    }

    private IEnumerator Co_RegenShield()
    {
        yield return new WaitForSeconds(playerData.shieldRegenDelay);
        currentShield = playerData.maxShield;
        shielded = true;

        yield return Co_ChangeSpriteColor(shieldSprite, playerData.flashDuration, Color.clear, Color.white);

        yield return Co_ChangeSpriteColor(shieldSprite, playerData.fadeDuration, Color.white, Color.clear);
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
