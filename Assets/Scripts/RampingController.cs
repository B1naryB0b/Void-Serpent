using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RampingController : MonoBehaviour
{
    [HideInInspector] public float ramping;

    [SerializeField] private float rampingDecay;
    [SerializeField] private float maxRamping;
    [SerializeField] private float minRamping;
    [SerializeField] private float rampingTierSize;

    [SerializeField] private Slider rampingSlider;
    [SerializeField] private TextMeshProUGUI rampingText;

    [HideInInspector] public int rampingTier;

    // Start is called before the first frame update
    void Start()
    {
        ramping = 0f;   
    }

    // Update is called once per frame
    void Update()
    {
        if (ramping > maxRamping)
        {
            ramping = maxRamping;
        }

        if (ramping < (rampingTier * rampingTierSize))
        {
            rampingTier--;
        }
        if (ramping > ((rampingTier+1) * rampingTierSize))
        {
            rampingTier++;
        }

        rampingText.text = Mathf.RoundToInt(ramping).ToString();

        rampingSlider.value = ramping - (rampingTier * rampingTierSize);

        DecayRamping();

    }

    public void IncreaseRamping(float increase)
    {
        ramping += increase;
    }

    private void DecayRamping()
    {
        print(ramping - (rampingDecay * rampingTier * Time.deltaTime));
        if (ramping - (rampingDecay * rampingTier * Time.deltaTime) < minRamping)
        {
            ramping = minRamping;
        }
        else
        {
            ramping -= rampingDecay * (rampingTier + 1) * Time.deltaTime;
        }
    }
}
