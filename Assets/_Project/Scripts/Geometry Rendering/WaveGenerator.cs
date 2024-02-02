using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveGenerator : MonoBehaviour
{

    public bool autoUpdate = true; // Default value can be true or false based on your preference
    public float frequency = 1.0f;
    public float amplitude = 1.0f;
    public float wavelength = 2.0f;
    public float phaseDifference = 0.0f;
    public float totalLength = 10.0f;
    public float lineWidth = 0.1f;
    public int resolution = 100;

    private LineRenderer lineRenderer;

    public void GenerateWave()
    { 
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("WaveGenerator requires a LineRenderer component.");
            return;
        }
    
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = resolution;
        float xIncrement = totalLength / resolution;

        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float x = xIncrement * i;
            float y = amplitude * Mathf.Sin((x / wavelength + phaseDifference) * frequency * 2 * Mathf.PI);
            points[i] = new Vector3(x, y, 0);
        }
        lineRenderer.SetPositions(points);
    }
}
