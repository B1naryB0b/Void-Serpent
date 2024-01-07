using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollowTarget : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;

    [SerializeField] private float distanceFromTarget;
    [Range(0f, 50f)]
    [SerializeField] private float distanceFromAnchor;

    [Range(0f, 100f)]
    [SerializeField] private float outOfRangeDistance;
    [SerializeField] private float outOfRangeFadeDistance;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float angleFromAnchorToTarget = CalculateSignedAngle(anchor, target);
        float angleFromTargetToAnchor = CalculateSignedAngle(target, anchor);

        Vector3 anchorRingPoint = PolarCoordinateToWorldSpace(anchor, angleFromAnchorToTarget, distanceFromAnchor);
        Vector3 targetRingPoint = PolarCoordinateToWorldSpace(target, angleFromTargetToAnchor, distanceFromTarget);

        Debug.DrawLine(anchor.position, anchorRingPoint, Color.red);
        Debug.DrawLine(target.position, targetRingPoint, Color.blue);

        Vector3 displacement = anchorRingPoint - targetRingPoint;
        Vector3 normalDisplacement = displacement.normalized;

        lineRenderer.SetPosition(0, targetRingPoint);
        lineRenderer.SetPosition(1, anchorRingPoint);


        float distance = displacement.magnitude;
        float alpha = Mathf.InverseLerp(outOfRangeDistance + outOfRangeFadeDistance, outOfRangeDistance, distance);
        Color currentColor = new Color(1, 1, 1, alpha);

        lineRenderer.startColor = currentColor; 
        lineRenderer.endColor = currentColor;


    }

    private float CalculateSignedAngle(Transform anchor, Transform target)
    {
        Vector3 directionToTarget = (target.position - anchor.position).normalized;
        Vector3 objectUp = transform.up;
        float signedAngle = Vector3.SignedAngle(objectUp, directionToTarget, Vector3.forward);

        // Adjust the angle by 90 degrees
        signedAngle += 90.0f;

        return signedAngle;
    }


    private Vector3 PolarCoordinateToWorldSpace(Transform origin, float angle, float distance)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        Vector3 newVector = new Vector3(
            origin.position.x + distance * Mathf.Cos(angleInRadians),
            origin.position.y + distance * Mathf.Sin(angleInRadians),
            origin.position.z
        );

        return newVector;
    }



}
