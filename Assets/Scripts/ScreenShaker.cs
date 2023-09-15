using UnityEngine;
using UnityEngine.UI;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private Transform levelTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;


    [SerializeField] private float MAX_TRANSLATIONAL_SHAKE = 0.5f;
    [SerializeField] private float MAX_ROTATIONAL_SHAKE = 15.0f;

    private float currentTrauma = 0f;
    private float setTrauma = 0f;

    [SerializeField] private Slider slider;
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float intensity;


    // Singleton pattern
    public static ScreenShaker Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) 
        {
            setTrauma += 0.1f;
            setTrauma = Mathf.Clamp01(setTrauma);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            setTrauma -= 0.1f;
            setTrauma = Mathf.Clamp01(setTrauma);
        }

        //currentTrauma = setTrauma;

        if (currentTrauma > 0)
        {
            ApplyShake();
            DecayTrauma();
            slider.value = currentTrauma;
        }
    }

    public void Shake(float traumaLevel)
    {
        ApplyTrauma(traumaLevel);
    }

    private void ApplyTrauma(float trauma)
    {
        currentTrauma += trauma;
        currentTrauma = Mathf.Clamp01(currentTrauma);
    }

    private void DecayTrauma()
    {
        currentTrauma -= Time.deltaTime;
        currentTrauma = Mathf.Clamp01(currentTrauma);
    }

    private void ApplyShake()
    {
        float shakeMagnitude = currentTrauma * currentTrauma;

        float xOffset = MAX_TRANSLATIONAL_SHAKE * shakeMagnitude * (GetPerlinNoise(100, Time.time * shakeSpeed));
        float yOffset = MAX_TRANSLATIONAL_SHAKE * shakeMagnitude * (GetPerlinNoise(200, Time.time * shakeSpeed));
        float rotationalOffset = MAX_ROTATIONAL_SHAKE * shakeMagnitude * (GetPerlinNoise(300, Time.time * shakeSpeed));

        Vector3 newPos = new Vector3(cameraTransform.position.x + xOffset, cameraTransform.position.y + yOffset, cameraTransform.position.z);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPos, Time.deltaTime * intensity);
        cameraTransform.rotation = Quaternion.Euler(0, 0, rotationalOffset);
    }

    private float GetPerlinNoise(int seed, float time)
    {
        return Mathf.PerlinNoise(seed + time, 0.0f);
    }
}
