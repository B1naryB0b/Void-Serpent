using UnityEngine;
using UnityEngine.UI;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private float _maxTranslationalShake;
    [SerializeField] private float _maxRotationalShake;

    [SerializeField] private bool _useSetTrauma;

    private float _currentTrauma;
    private float _setTrauma;

    [SerializeField] private Slider _shakeTraumaSlider;
    [SerializeField] private float _shakeSpeed;
    [SerializeField] private float _shakeIntensity;

    public float traumaDecaySpeed = 1f;

    public static ScreenShaker Instance { get; private set; }

    private void Awake()
    {
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
        if (_useSetTrauma)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _setTrauma += 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                _setTrauma -= 0.1f;
            }

            _setTrauma = Mathf.Clamp01(_setTrauma);

            _currentTrauma = _setTrauma; 
        }

        if (_currentTrauma > 0)
        {
            ApplyShake();
            DecayTrauma();
            _shakeTraumaSlider.value = _currentTrauma;
        }
    }

    public float GetCurrentTrauma()
    {
        return _currentTrauma;
    }

    public void Shake(float traumaLevel)
    {
        ApplyTrauma(traumaLevel);
    }

    private void ApplyTrauma(float trauma)
    {
        _currentTrauma += trauma;
        _currentTrauma = Mathf.Clamp01(_currentTrauma);
    }

    private void DecayTrauma()
    {
        _currentTrauma -= traumaDecaySpeed * Time.deltaTime;
        _currentTrauma = Mathf.Clamp01(_currentTrauma);
    }

    private void ApplyShake()
    {
        float shakeMagnitude = _currentTrauma * _currentTrauma;

        float xOffset = _maxTranslationalShake * shakeMagnitude * (GetPerlinNoise(Random.Range(0, 100), Time.time * _shakeSpeed));
        float yOffset = _maxTranslationalShake * shakeMagnitude * (GetPerlinNoise(Random.Range(100, 200), Time.time * _shakeSpeed));

        float rotationalOffset = _maxRotationalShake * shakeMagnitude * (GetPerlinNoise(Random.Range(200, 300), Time.time * _shakeSpeed));

        Vector3 newPos = new Vector3(_cameraTransform.position.x + xOffset, _cameraTransform.position.y + yOffset, _cameraTransform.position.z);

        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, newPos, Time.deltaTime * _shakeIntensity);
        _cameraTransform.rotation = Quaternion.Euler(0, 0, rotationalOffset);
    }

    private float GetPerlinNoise(int seed, float time)
    {
        return Mathf.PerlinNoise(seed + time, 0.0f);
    }
}
