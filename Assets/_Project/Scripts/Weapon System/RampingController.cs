using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RampingController : MonoBehaviour
{
    [HideInInspector] public float _ramping;

    [SerializeField] private float _maxRamping;
    [SerializeField] private float _minRamping;
    [SerializeField] private float[] _rampingDecayPerTier; 
    [SerializeField] private float[] _rampingTierSizes; 

    [SerializeField] private Slider _rampingSlider;
    [SerializeField] private TextMeshProUGUI _rampingText;

    [HideInInspector] public int CurrentRampingTier;

    void Start()
    {
        _ramping = 0f;
        _rampingSlider.maxValue = _rampingTierSizes[0];
    }


    void Update()
    {
        _ramping = Mathf.Min(_ramping, _maxRamping);

        float threshold = 0f;
        for (int i = 0; i < _rampingTierSizes.Length; i++)
        {
            threshold += _rampingTierSizes[i];
            if (_ramping <= threshold)
            {
                CurrentRampingTier = i;
                _rampingSlider.maxValue = _rampingTierSizes[CurrentRampingTier];
                break;
            }
        }

        _rampingText.text = Mathf.RoundToInt(_ramping).ToString();

        float baseThreshold = threshold - _rampingTierSizes[CurrentRampingTier]; // base value for the current tier
        _rampingSlider.value = _ramping - baseThreshold;

        DecayRamping();
    }


    public void IncreaseRamping(float increase)
    {
        _ramping += increase;
    }

    private void DecayRamping()
    {
        if (CurrentRampingTier >= _rampingDecayPerTier.Length)
        {
            Debug.LogError("Ramping tier is out of bounds of the rampingDecays array. Please ensure the array is set up correctly.");
            return;
        }

        float decayAmount = _rampingDecayPerTier[CurrentRampingTier] * Time.deltaTime;

        _ramping = Mathf.Max(_ramping - decayAmount, _minRamping);
    }


}
