using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RampingController : MonoBehaviour
{
    [HideInInspector] public float ramping;

    [SerializeField] private float maxRamping;
    [SerializeField] private float minRamping;
    [SerializeField] private float[] rampingDecays; 
    [SerializeField] private float[] rampingTierSizes; 

    [SerializeField] private Slider rampingSlider;
    [SerializeField] private TextMeshProUGUI rampingText;

    [HideInInspector] public int rampingTier;

    // Start is called before the first frame update
    void Start()
    {
        ramping = 0f;
        rampingSlider.maxValue = rampingTierSizes[0];
    }


    void Update()
    {
        // Clamp the ramping to its maximum value.
        ramping = Mathf.Min(ramping, maxRamping);

        // Check and update the current tier based on ramping value.
        float threshold = 0f;
        for (int i = 0; i < rampingTierSizes.Length; i++)
        {
            threshold += rampingTierSizes[i];
            if (ramping <= threshold)
            {
                rampingTier = i;
                rampingSlider.maxValue = rampingTierSizes[rampingTier];
                break;
            }
        }

        // Update the ramping text display.
        rampingText.text = Mathf.RoundToInt(ramping).ToString();

        // Update the ramping slider value based on the current tier.
        float baseThreshold = threshold - rampingTierSizes[rampingTier]; // base value for the current tier
        rampingSlider.value = ramping - baseThreshold;

        // Decay the ramping value.
        DecayRamping();
    }


    public void IncreaseRamping(float increase)
    {
        ramping += increase;
    }

    private void DecayRamping()
    {
        if (rampingTier >= rampingDecays.Length)
        {
            Debug.LogError("Ramping tier is out of bounds of the rampingDecays array. Please ensure the array is set up correctly.");
            return;
        }

        // Calculate the decay amount based on the current tier's decay rate.
        float decayAmount = rampingDecays[rampingTier] * Time.deltaTime;

        // Apply the decay while ensuring the ramping doesn't go below the minimum value.
        ramping = Mathf.Max(ramping - decayAmount, minRamping);
    }


}
