using UnityEngine;

public class PolygonGenerator : MonoBehaviour
{
    public int sides = 3;
    public float radius = 1f;
    public float lineWidth = 0.1f;

    public void GeneratePolygon()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();

        // Set the line width
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Set the line join and cap to make the ends sit flush
        lineRenderer.loop = true; // Ensure the line renderer forms a closed loop
        lineRenderer.useWorldSpace = false;

        Vector3[] vertices = new Vector3[sides];

        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2 / sides;
            vertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        }

        lineRenderer.positionCount = vertices.Length;
        lineRenderer.SetPositions(vertices);
    }
}