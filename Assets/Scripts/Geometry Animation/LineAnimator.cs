using UnityEngine;
using System.Collections;

public class LineAnimator : MonoBehaviour
{
    [SerializeField] private float animationDuration = 5f;
    [SerializeField] private bool loopAnimation = true; // Toggle looping on or off
    [SerializeField] private float randomDelay;

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

            yield return new WaitForSeconds(Random.Range(0, randomDelay));

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
                    for (int j = 0; j < i + 1; j++)
                        lineRenderer.SetPosition(j, pos);

                    yield return null;
                }
            }

            for (int i = 0; i < pointsCount; i++)
            {
                lineRenderer.SetPosition(i, linePoints[i]);
            }

        }
        while (loopAnimation);
    }
}
