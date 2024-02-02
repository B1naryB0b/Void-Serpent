using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    private enum FollowType
    {
        Disabled,
        Snap,
        Smooth
    }

    [SerializeField] private FollowType followType;
    [SerializeField] private Transform target;

    [SerializeField] private float smoothFollowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followType == FollowType.Disabled) return;

        if (followType == FollowType.Snap) SnapFollow();

        if (followType == FollowType.Smooth) SmoothFollow();
    }

    private void SnapFollow()
    {
        gameObject.transform.position = target.position;
    }

    private void SmoothFollow()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.position, Time.deltaTime * smoothFollowSpeed);
    }
}
