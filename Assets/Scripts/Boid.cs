using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Boid : MonoBehaviour
{
    [Header("Boid Parameters")]
    [SerializeField] private float visionThreshold;
    [SerializeField] private float speed;
    [SerializeField] private float separationWeight;
    [SerializeField] private float alignmentWeight;
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float pullSpeed;

    [Header("Boundary Parameters")]
    [SerializeField] private float xBound;
    [SerializeField] private float yBound;

    [Header("Collision Detection")]
    [SerializeField] private float collisionDetectionRadius;
    [SerializeField] private LayerMask colliderLayer;

    private List<Transform> localBoidList = new List<Transform>();
    private List<Transform> visibleBoidList = new List<Transform>();
    private Vector3 boidDirection;
    private Rigidbody2D boidRb;

    private void Start()
    {
        boidRb = GetComponent<Rigidbody2D>();
        boidRb.velocity = transform.up * speed;
    }

    private void FixedUpdate()
    {
        HitBoundingBox();

        boidDirection = transform.up;
        transform.up = boidRb.velocity.normalized;

        if (localBoidList.Count > 20)
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
        }

        foreach (Transform boid in localBoidList)
        {
            if (InFOV(boid.position))
            {
                /*print(gameObject.name + " can see " + boid.name);
                Debug.DrawLine(transform.position, boid.position);*/

                visibleBoidList.Add(boid);
            }
            else
            {
                visibleBoidList.Remove(boid);
            }
        }



        Vector3 steer = SteerSeparation() + Alignment() + Cohesion();
        boidRb.velocity = Vector3.Lerp(boidRb.velocity, ((Vector3)boidRb.velocity + steer).normalized * speed, Time.fixedDeltaTime * smoothSpeed);
    }

    void HitBoundingBox()
    {
        Vector2 pullDirection = Vector2.zero;

        if (Mathf.Abs(transform.position.x) > xBound)
        {
            pullDirection.x = -transform.position.x;
        }

        if (Mathf.Abs(transform.position.y) > yBound)
        {
            pullDirection.y = -transform.position.y;
        }

        boidRb.velocity += pullDirection.normalized * pullSpeed * Time.fixedDeltaTime;
    }

    private Vector3 SteerSeparation()
    {
        Vector3 direction = Vector3.zero;

        foreach (Transform boid in visibleBoidList)
        {
            float ratio = Mathf.Clamp01((boid.transform.position - transform.position).magnitude / separationWeight);
            direction -= ratio * (boid.transform.position - transform.position);
        }

        return direction.normalized;
    }

    private Vector3 Alignment()
    {
        Vector3 averageVelocity = Vector3.zero;

        if (visibleBoidList.Count == 0)
            return averageVelocity;

        foreach (Transform boid in visibleBoidList)
        {
            Rigidbody2D boidRb = boid.GetComponent<Rigidbody2D>();
            if (boidRb != null)
            {
                averageVelocity += (Vector3)boidRb.velocity;
            }
        }

        averageVelocity /= visibleBoidList.Count;
        Vector3 steerDirection = averageVelocity - (Vector3)boidRb.velocity;

        return steerDirection.normalized * alignmentWeight;
    }

    private Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;

        if (visibleBoidList.Count == 0)
            return centerOfMass;

        foreach (Transform boid in visibleBoidList)
        {
            centerOfMass += boid.position;
        }

        centerOfMass /= visibleBoidList.Count;
        Vector3 steerDirection = centerOfMass - transform.position;

        return steerDirection.normalized * cohesionWeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision != null) && (collision.CompareTag("Boid")))
        {
            localBoidList.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision != null) && (collision.CompareTag("Boid")))
        {
            localBoidList.Remove(collision.transform);
        }
    }

    bool InFOV(Vector3 position)
    {
        Vector3 otherBoidDirection = transform.position - position;
        return Vector3.Dot(boidDirection, otherBoidDirection) > visionThreshold;
    }
}
