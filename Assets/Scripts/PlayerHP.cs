using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class PlayerHP : MonoBehaviour
{

    public int maxLives = 3;  // Maximum lives of the player
    private int currentLives; // Current lives of the player
    public GameObject lifeIconPrefab; // Prefab for the life icon UI
    public Transform lifeIconContainer; // UI container for the life icons
    public float livesUISpacingOffset;
    public float livesUIHorizontalAdjustmentOffset;


    public int maxShield = 1;
    private int currentShield;

    public GameObject explosionPrefab;

    private bool shielded = false;
    public float shieldedInvulnerabilityDuration = 0.5f;  // Duration of invulnerability in seconds
    public float shieldRegenDelay = 5f; // Time in seconds before the shield regenerates. Adjust as needed.
    public float flashDuration = 0.5f; // Duration for the shield to lerp from clear to white. Adjust as needed.
    public float fadeDuration = 0.5f; // Duration for the shield to fade from white to clear. Adjust as needed.
    public SpriteRenderer shieldSprite;

    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 2.0f;  // Duration of invulnerability in seconds

    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;
        shielded = true;
        UpdateLifeUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Reduce player's health by the given damage
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
                StartCoroutine(FlashAndFadeShield());
                StartCoroutine(InvulnerabilityCoroutine(shieldedInvulnerabilityDuration));
                StartCoroutine(RegenShield());
            }
        }
        else
        {
            currentLives -= damage;
            UpdateLifeUI();
            Debug.Log(currentLives);

            if (currentLives <= 0)
            {
                // Player is out of lives. Handle game over logic here.
                Debug.Log("Game Over!");
                Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                // Start invulnerability coroutine
                StartCoroutine(InvulnerabilityCoroutine(invulnerabilityDuration));
            }
        }
    }

    private void UpdateLifeUI()
    {
        // Destroy all existing life icons
        foreach (Transform child in lifeIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate life icons based on current lives
        for (int i = 0; i < currentLives; i++)
        {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeIconContainer.transform); // Instantiates and sets the parent in one step
            lifeIcon.transform.localPosition = new Vector3(i * livesUISpacingOffset - livesUIHorizontalAdjustmentOffset, 0, 0);
        }
    }


    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        // Optionally, you can add visual feedback for invulnerability, like blinking the player sprite.

        yield return new WaitForSeconds(duration);

        isInvulnerable = false;
        // Optionally, revert any visual feedback for invulnerability.
    }

    IEnumerator FlashAndFadeShield()
    {
        // Flash the shield color to white
        shieldSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f); // Duration of the white flash

        // Lerp the shield color to clear
        float duration = 0.5f; // Duration of the fade to clear
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            shieldSprite.color = Color.Lerp(Color.white, Color.clear, elapsedTime / duration);
            yield return null;
        }
    }

    IEnumerator RegenShield()
    {
        yield return new WaitForSeconds(shieldRegenDelay);
        currentShield = maxShield;
        shielded = true;

        // Lerp shield color from clear to white
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            shieldSprite.color = Color.Lerp(Color.clear, Color.white, elapsedTime / flashDuration);
            yield return null;
        }

        // Reset elapsedTime for the fade back to clear
        elapsedTime = 0f;

        // Lerp shield color from white to clear
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            shieldSprite.color = Color.Lerp(Color.white, Color.clear, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
