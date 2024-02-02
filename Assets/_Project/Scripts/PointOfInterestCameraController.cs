using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointOfInterestCameraController : MonoBehaviour
{
    // Core Components
    public POI player; // Primary point of focus
    public POI cursorTransform;
    List<POI> primaryFocusPoints;
    public List<POI> secondaryFocusPoints; // Enemies, allies, etc.
    public List<POI> pointsOfInterest; // Loot, switches, etc.

    // Camera Settings
    public float smoothSpeed = 0.125f; // Camera's smooth follow speed
    public Vector3 offset; // Offset from the calculated position
    [Range(0f, 1f)] public float focusWeight = 0.5f; // Weight for focus between player (0) and mouse (1)
    public float minFOV; // Minimum field of view
    public float maxFOV; // Maximum field of view

    // Proximity Settings
    public float innerThreshold = 5f; // Inside this threshold, proximity = 1
    public float outerThreshold = 10f; // Outside this threshold, proximity = 0

    private int entitiesWithinFOVThreshold = 0;
    private float currentFOVRatio = 0f;
    public float smoothFOVTransitionSpeed = 5f; // Adjust this value to control the speed of the transition

    Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = player.poiTransform.GetComponent<Rigidbody2D>();

        secondaryFocusPoints.AddRange(FindObjectsOfType<EnemyController>().Select(enemy => new POI { poiTransform = enemy.transform })); 
        
        foreach (POI point in secondaryFocusPoints)
        {
            Debug.Log(point.poiTransform.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        cursorTransform.poiTransform.position = cursorPosition;

        primaryFocusPoints = new List<POI> { player, cursorTransform };

        // Calculate the weighted average position
        Vector3 weightedAveragePosition = CalculateWeightedAveragePosition(primaryFocusPoints, secondaryFocusPoints, pointsOfInterest);

        // Adjust FOV for framing
        AdjustFOV();

        Vector3 velocityCorrection = CorrectForVelocity();

        // Smoothly move the camera to the weighted average position
        SmoothMoveToPosition(weightedAveragePosition + velocityCorrection);

        // Correct the position of the camera for velocity
    }

    Vector3 CalculateWeightedAveragePosition(List<POI> primaryPoints, List<POI> secondaryPoints, List<POI> poiPoints)
    {
        Vector3 weightedPosition = Vector3.zero;
        float totalProximity = 0f;

        // Calculate proximity for each primary focus point
        foreach (POI primary in primaryPoints.ToList())
        {
            if (primary.poiTransform != null)
            {
                float primaryProximity = 1f; // Always prioritize primary focus points
                weightedPosition += primary.poiTransform.position * primary.importance * primaryProximity;
                totalProximity += primaryProximity * primary.importance;
            }
            else
            {
                primaryPoints.Remove(primary);
            }
        }

        // Calculate proximity for each secondary focus point
        foreach (POI secondary in secondaryPoints.ToList())
        {
            if (secondary.poiTransform != null)
            {
                float distanceToSecondary = Vector3.Distance(primaryPoints[0].poiTransform.position, secondary.poiTransform.position); // Assuming the first primary point as reference
                float secondaryProximity = CalculateProximity(distanceToSecondary);
                weightedPosition += secondary.poiTransform.position * secondary.importance * secondaryProximity;
                totalProximity += secondaryProximity * secondary.importance;
            }
            else
            {
                secondaryPoints.Remove(secondary);
            }
        }

        // Calculate proximity for each point of interest
        foreach (POI poi in poiPoints.ToList())
        {
            if (poi.poiTransform != null)
            {
                float distanceToPOI = Vector3.Distance(primaryPoints[0].poiTransform.position, poi.poiTransform.position); // Assuming the first primary point as reference
                float poiProximity = CalculateProximity(distanceToPOI);
                weightedPosition += poi.poiTransform.position * poi.importance * poiProximity;
                totalProximity += poiProximity * poi.importance;

                
            }
            else 
            { 
                primaryPoints.Remove(poi); 
            }
        }

        return weightedPosition / totalProximity;
    }

    float CalculateProximity(float distance)
    {
        if (distance > outerThreshold)
        {
            return 0f;
        }
        else if (distance <= innerThreshold)
        {
            return 1f;
        }
        else
        {
            // Lerp between 0 and 1 based on where the distance lies between the inner and outer thresholds
            return Mathf.Lerp(0f, 1f, 1 - (distance - innerThreshold) / (outerThreshold - innerThreshold));
        }
    }

    int WithinFOVThreshold(List<POI> points)
    {
        int pointsWithinThreshold = 0;

        foreach (POI point in points)
        {
            if (point != null && Vector3.Distance(transform.position, point.poiTransform.position) <= outerThreshold)
            {
                pointsWithinThreshold++;
            }
        }

        return pointsWithinThreshold;
    }

    void AdjustFOV()
    {
        // Count the number of entities within the outerThreshold distance
        entitiesWithinFOVThreshold = 0;

        int secondaryPointCount = WithinFOVThreshold(secondaryFocusPoints);
        int poiPointCount = WithinFOVThreshold(pointsOfInterest);

        entitiesWithinFOVThreshold = secondaryPointCount + poiPointCount;

        // Define a constant that determines how quickly the ratio approaches 1 as entitiesWithinFOVThreshold increases.
        // The larger this value, the quicker the ratio will approach 1.
        const float sensitivity = 0.1f;

        // Calculate the target ratio using a logarithmic approach
        float targetRatio = Mathf.Clamp01(1 - Mathf.Exp(-sensitivity * entitiesWithinFOVThreshold));

        // Lerp the current ratio towards the target ratio over time
        currentFOVRatio = Mathf.Lerp(currentFOVRatio, targetRatio, smoothFOVTransitionSpeed * Time.deltaTime);

        // Interpolate between minFOV and maxFOV based on the current ratio
        Camera.main.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentFOVRatio);
    }


    void SmoothMoveToPosition(Vector3 targetPosition)
    {
        // Smoothly move the camera to the target position
        Vector3 desiredPosition = targetPosition + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    Vector3 CorrectForVelocity()
    {
        // Adjust the camera's position based on player's velocity
        if (playerRb != null)
        {
            return (Vector3)playerRb.velocity * Time.deltaTime;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
