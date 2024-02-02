using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private int _frameRange;

    [SerializeField] private TextMeshProUGUI _fpsDisplayText;

    private int[] _fpsBuffer;
    private int _fpsBufferIndex;

    private int _highestFPS;
    private int _lowestFPS;

    private void InitializeBuffer()
    {
        if (_frameRange <= 0)
        {
            _frameRange = 1;
        }
        _fpsBuffer = new int[_frameRange];
        _fpsBufferIndex = 0;
    }

    private void Update()
    {
        if (_fpsBuffer == null || _fpsBuffer.Length != _frameRange)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();

        if (_fpsDisplayText != null)
        {
            _fpsDisplayText.text = $"Average FPS: {GetAverageFPS()} \n Highest: {_highestFPS} \n Lowest: {_lowestFPS}";
        }
    }

    private void UpdateBuffer()
    {
        _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (_fpsBufferIndex >= _frameRange)
        {
            _fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < _frameRange; i++)
        {
            int fps = _fpsBuffer[i];
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
        _highestFPS = highest;
        _lowestFPS = lowest == int.MaxValue ? 0 : lowest;
    }

    private int GetAverageFPS()
    {
        int sum = 0;
        for (int i = 0; i < _frameRange; i++)
        {
            sum += _fpsBuffer[i];
        }
        return (int)((float)sum / _frameRange);
    }
}
