using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image _blackoutScreen;
    [SerializeField] private TextMeshProUGUI _endScreenText;
    [SerializeField] private TextMeshProUGUI _retryButtonText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private TextMeshProUGUI _nextLevelButtonText;
    [SerializeField] private Button _nextLevelButton;

    [SerializeField] private Color _endTextColor;
    [SerializeField] private Color _retryButtonTextColor;
    [SerializeField] private Color _nextLevelButtonTextColor;


    public enum FadeState
    {
        EndScreen,
        NextLevelScreen,
    }

    private IEnumerator Co_Fade<T>(T component, Color targetColor, float duration) where T : Graphic
    {
        Color startColor = component.color;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            component.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        component.color = targetColor;
    }

    public void FadeScreen(Color targetColor, float duration)
    {
        StartCoroutine(Co_Fade(_blackoutScreen, targetColor, duration));
    }

    public void TriggerScreen(FadeState state, float duration)
    {
        switch (state)
        {
            case FadeState.EndScreen:
                FadeScreen(Color.black, duration);
                StartCoroutine(Co_Fade(_endScreenText, _endTextColor, duration));
                StartCoroutine(Co_Fade(_retryButtonText, _retryButtonTextColor, duration));
                _retryButton.interactable = true;
                break;

            case FadeState.NextLevelScreen:
                FadeScreen(Color.black, duration);
                StartCoroutine(Co_Fade(_nextLevelButtonText, _nextLevelButtonTextColor, duration));
                _nextLevelButton.interactable = true;
                break;
        }
    }

    public void NewLevel()
    {
        StartCoroutine(Co_Fade(_nextLevelButtonText, Color.clear, 0.1f));
    }

    public void RetryScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
