using UnityEngine;
using System.Collections;

public class ReversingLineAnimator : MonoBehaviour
{
    [SerializeField] private float animationDuration = 5f;
    [SerializeField] private bool loopAnimation = true; // Toggle looping on or off

    private LineRenderer lineRenderer;
    private Vector3[] linePoints;
    private int pointsCount;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Store a copy of lineRenderer's points in linePoints array
        pointsCount = lineRenderer.positionCount;
        linePoints = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            linePoints[i] = lineRenderer.GetPosition(i);
        }

        StartCoroutine(AnimateLine());
    }

    private IEnumerator AnimateLine()
    {
        do
        {
            for (int i = 0; i < pointsCount - 1; i++)
            {
                float segmentDuration = animationDuration / (pointsCount - 1);
                float startTime = Time.time;
                Vector3 startPosition = linePoints[i];
                Vector3 endPosition = linePoints[i + 1];

                Vector3 pos = startPosition;
                while (pos != endPosition)
                {
                    float t = (Time.time - startTime) / segmentDuration;
                    pos = Vector3.Lerp(startPosition, endPosition, t);

                    // Animate all points from index i to the end
                    for (int j = i + 1; j < pointsCount; j++)
                        lineRenderer.SetPosition(j, pos);

                    yield return null;
                }
            }

            for (int i = pointsCount - 1; i >= 0; i--)
            {
                float segmentDuration = animationDuration / (pointsCount - 1);
                float startTime = Time.time;
                Vector3 startPosition = linePoints[i]; // Starting at the current point
                Vector3 endPosition = (i > 0) ? linePoints[i - 1] : linePoints[0]; // Move towards the previous point

                Vector3 pos = startPosition;
                while (Time.time < startTime + segmentDuration)
                {
                    float t = (Time.time - startTime) / segmentDuration;
                    pos = Vector3.Lerp(startPosition, endPosition, t);

                    // Set the position of the current point to 'pos'
                    lineRenderer.SetPosition(i, pos);

                    // For all points after the current point, set their position to the end position
                    // This will make them "disappear" or "erase"
                    for (int j = i + 1; j < pointsCount; j++)
                    {
                        lineRenderer.SetPosition(j, endPosition);
                    }

                    yield return null;
                }

                // Ensure the current segment is set to the exact end position when done
                lineRenderer.SetPosition(i, endPosition);
            }


        }
        while (loopAnimation);
    }
}
