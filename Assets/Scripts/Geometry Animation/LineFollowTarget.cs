using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollowTarget : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;

    [Range(0f, 20f)]
    [SerializeField] private float distanceFromTarget;
    [Range(0f, 20f)]
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
        Vector3 displacement = target.position - anchor.position;
        Vector3 normalDisplacement = displacement.normalized;

        lineRenderer.SetPosition(0, target.position + (normalDisplacement * -distanceFromTarget));
        lineRenderer.SetPosition(1, anchor.position + (normalDisplacement * distanceFromAnchor));

        Color currentColor = new Color(255, 255, 255, Mathf.Clamp01(displacement.magnitude - (distanceFromAnchor + distanceFromTarget)));

        lineRenderer.startColor = currentColor; 
        lineRenderer.endColor = currentColor;


    }
}
