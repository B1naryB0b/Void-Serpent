using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image blackoutScreen;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private TextMeshProUGUI retryButtonText;
    [SerializeField] private Button retryButton;

    [SerializeField] private TextMeshProUGUI nextLevelButtonText;
    [SerializeField] private Button nextLevelButton;


    [SerializeField] private Color endTextColor;
    [SerializeField] private Color retryButtonTextColor;
    [SerializeField] private Color nextLevelButtonTextColor;

    private void Start()
    {

    }

    // Coroutine for fading the screen
    private IEnumerator FadeScreenCoroutine(Color targetColor, float duration)
    {
        Color startColor = blackoutScreen.color;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            blackoutScreen.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        blackoutScreen.color = targetColor; // Ensure final color is reached
    }

    // Coroutine for fading the text element
    private IEnumerator FadeTextCoroutine(TextMeshProUGUI textElement, Color targetColor, float duration)
    {
        Color startColor = textElement.color;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            textElement.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        textElement.color = targetColor; // Ensure final color is reached
    }

    // Method to fade the image to black
    public void FadeToBlack(float duration)
    {
        StartCoroutine(FadeScreenCoroutine(Color.black, duration));
    }

    // Method to fade the image to clear
    public void FadeToClear(float duration)
    {
        StartCoroutine(FadeScreenCoroutine(Color.clear, duration));
    }

    // Method to trigger the end screen
    public void TriggerEndScreen(float duration)
    {
        // Start fading the screen to black and text elements to their target color
        StartCoroutine(FadeScreenCoroutine(Color.black, duration));
        StartCoroutine(FadeTextCoroutine(endText, endTextColor, duration));
        StartCoroutine(FadeTextCoroutine(retryButtonText, retryButtonTextColor, duration));

        // Make the "Retry" button interactible
        retryButton.interactable = true;
    }

    // Method to trigger the end screen
    public void TriggerNextLevelScreen(float duration)
    {
        // Start fading the screen to black and text elements to their target color
        StartCoroutine(FadeScreenCoroutine(Color.black, duration));
        StartCoroutine(FadeTextCoroutine(nextLevelButtonText, nextLevelButtonTextColor, duration));

        // Make the "Retry" button interactible
        nextLevelButton.interactable = true;
    }

    public void NewLevel()
    {
        StartCoroutine(FadeTextCoroutine(nextLevelButtonText, Color.clear, 0.1f));
    }

    // Method to retry the scene
    public void RetryScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
