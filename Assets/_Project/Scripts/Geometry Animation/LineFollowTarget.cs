using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollowTarget : MonoBehaviour
{

    [SerializeField] private bool debugMode;

    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;

    [SerializeField] private float distanceFromTarget;
    [Range(0f, 50f)]
    [SerializeField] private float distanceFromAnchor;

    [Range(0f, 100f)]
    [SerializeField] private float outOfRangeDistance;
    [SerializeField] private float outOfRangeFadeDistance;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        float angleFromAnchorToTarget = CalculateSignedAngle(anchor, target);
        float angleFromTargetToAnchor = CalculateSignedAngle(target, anchor);

        if (debugMode)
        {
            //Debug.Log(angleFromTargetToAnchor);
        }

        Vector3 anchorRingPoint = PolarCoordinateToWorldSpace(anchor, angleFromAnchorToTarget, distanceFromAnchor);
        Vector3 targetRingPoint = PolarCoordinateToWorldSpace(target, angleFromTargetToAnchor, distanceFromTarget);

        //Debug.DrawLine(anchor.position, anchorRingPoint, Color.red);
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
        Vector3 toTarget = target.position - anchor.position;
        Vector3 toTargetOnPlane = Vector3.ProjectOnPlane(toTarget, Vector3.forward);

        float angle = Vector3.Angle(Vector3.right, toTargetOnPlane);
        float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.right, toTargetOnPlane)));

        float signedAngle = angle * sign;

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
