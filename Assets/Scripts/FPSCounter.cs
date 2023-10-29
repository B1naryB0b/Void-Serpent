using UnityEngine;
using UnityEngine.UI; // Make sure to add this using if you plan to output text to a UI component.
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public int frameRange = 60; // Number of frames to measure for the average count.

    public TextMeshProUGUI displayText; // Optional: assign a UI Text component to output the FPS values.

    private int[] fpsBuffer;
    private int fpsBufferIndex;

    private int highestFPS;
    private int lowestFPS;

    private void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    private void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();

        // Optional: Display it on a UI text element.
        if (displayText != null)
        {
            displayText.text = $"Average FPS: {GetAverageFPS()} \n Highest: {highestFPS} \n Lowest: {lowestFPS}";
        }
    }

    private void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < frameRange; i++)
        {
            int fps = fpsBuffer[i];
            sum += fps;
            if (fps > highest)
            {
                highest = fps;
            }
            if (fps < lowest)
            {
                lowest = fps;
            }
        }
        highestFPS = highest;
        lowestFPS = lowest == int.MaxValue ? 0 : lowest; // In case we don't have enough frames accumulated.
    }

    private int GetAverageFPS()
    {
        int sum = 0;
        for (int i = 0; i < frameRange; i++)
        {
            sum += fpsBuffer[i];
        }
        return (int)((float)sum / frameRange);
    }
}
