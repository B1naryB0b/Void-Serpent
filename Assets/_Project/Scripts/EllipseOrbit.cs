using UnityEngine;
using System.Collections.Generic;

public class EllipseOrbit : MonoBehaviour
{
    public Transform target;
    public float xAxis = 5f;
    public float yAxis = 3f;
    public int numberOfPoints = 20;
    public float speed = 1f;
    public float angle = 0f; // Angle of rotation in degrees
    [Range(0f, 5f)]
    public float tilt = 0f;

    private List<Vector3> ellipsePoints;
    private List<float> cumulativeDistances;
    private float totalDistance;
    private float distanceTraveled = 0f;

    void Start()
    {
        GenerateEllipsePoints();
        CalculateCumulativeDistances();
    }

    void Update()
    {
        if (ellipsePoints.Count == 0) return;

        MoveInEllipse();
        DrawEllipsePath();
    }

    void GenerateEllipsePoints()
    {
        ellipsePoints = new List<Vector3>();
        Quaternion rotation = Quaternion.Euler(tilt, 0, angle);

        for (int i = 0; i < numberOfPoints; i++)
        {
            float angleInRadians = (i / (float)numberOfPoints) * 2 * Mathf.PI;
            float x = Mathf.Cos(angleInRadians) * xAxis;
            float y = Mathf.Sin(angleInRadians) * yAxis;
            Vector3 point = new Vector3(x, y, gameObject.transform.position.z);
            point = rotation * point; // Apply rotation
            point += target.position;  // Translate to target position
            ellipsePoints.Add(point);
        }
    }

    void CalculateCumulativeDistances()
    {
        cumulativeDistances = new List<float>();
        totalDistance = 0f;

        for (int i = 0; i < ellipsePoints.Count; i++)
        {
            if (i == 0)
            {
                cumulativeDistances.Add(0f);
            }
            else
            {
                totalDistance += Vector3.Distance(ellipsePoints[i - 1], ellipsePoints[i]);
                cumulativeDistances.Add(totalDistance);
            }
        }
    }

    void MoveInEllipse()
    {
        distanceTraveled += speed * Time.deltaTime;
        distanceTraveled = distanceTraveled % totalDistance;

        // Find the current segment
        int segmentIndex = cumulativeDistances.FindIndex(dist => dist >= distanceTraveled);
        if (segmentIndex == 0 || segmentIndex == -1)
        {
            return;
        }

        Vector3 startPoint = ellipsePoints[segmentIndex - 1];
        Vector3 endPoint = ellipsePoints[segmentIndex];
        float segmentDistance = cumulativeDistances[segmentIndex] - cumulativeDistances[segmentIndex - 1];
        float segmentTravel = distanceTraveled - cumulativeDistances[segmentIndex - 1];

        float lerpValue = segmentTravel / segmentDistance;
        transform.position = Vector3.Lerp(startPoint, endPoint, lerpValue);
    }

    void DrawEllipsePath()
    {
        for (int i = 0; i < ellipsePoints.Count; i++)
        {
            Vector3 currentPoint = ellipsePoints[i];
            Vector3 nextPoint = ellipsePoints[(i + 1) % ellipsePoints.Count];
            Debug.DrawLine(currentPoint, nextPoint, Color.red);
        }
    }
}
