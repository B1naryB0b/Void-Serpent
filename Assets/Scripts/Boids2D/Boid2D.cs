using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid2D : MonoBehaviour
{

    // Settings and State
    BoidSettings2D settings;

    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector2 velocity;

    // To update:
    Vector2 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    void Awake()
    {
        material = transform.GetComponentInChildren<SpriteRenderer>().material;
        cachedTransform = transform;
        

    }

    public void Initialize(BoidSettings2D settings, Transform target)
    {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        if (material != null)
        {
            material.color = col;
        }
    }

/*    private void Update()
    {
        Vector2[] rayDirections = BoidHelper2D.directions;
        int totalDirections = rayDirections.Length;

        for (int i = 0; i < totalDirections; i++)
        {
            // Calculate the correct index based on the current iteration.
            int index = i % 2 == 0 ? i / 2 : totalDirections - 1 - (i / 2);

            Vector2 dir = cachedTransform.TransformDirection(rayDirections[index]);
            dir.Normalize();

            // For debugging: draw a line to show the ray. Color it black by default.
            Debug.DrawLine(position, position + new Vector3(dir.x, dir.y, 0) * settings.collisionAvoidDst, Color.black);

            // Perform the sphere cast and check if there's no collision.
            RaycastHit2D hit = Physics2D.CircleCast(position, settings.boundsRadius, dir, settings.collisionAvoidDst, settings.obstacleMask);
            

            if (hit.collider == null)
            {
                // For debugging: draw a line in a different color (e.g., green) to indicate a safe direction.
                Debug.DrawLine(position, position + new Vector3(dir.x, dir.y, 0) * settings.collisionAvoidDst, Color.green);

            }
        }

    }*/

    public void UpdateBoid()
    {
        if (cachedTransform == null) return;

        acceleration = Vector2.zero;

        if (target != null)
        {
            Vector2 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget * (1 + offsetToTarget.sqrMagnitude)) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;
            centreOfFlockmates.z = 0;  // Restricting calculation to the x-z plane

            Vector2 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            Vector2 alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            Vector2 cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            Vector2 separationForce = SteerTowards(avgAvoidanceHeading) * settings.separateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += separationForce;
        }

        // You will need to decide how to handle collisions in 2D. The following is based on your 3D world, and you may need to adjust accordingly.
        if (IsHeadingForCollision())
        {
            Vector2 collisionAvoidDir = ObstacleRays();
            Vector2 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector2 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        cachedTransform.up = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision()
    {
        // Cast a circle in the forward direction to detect obstacles
        RaycastHit2D hit = Physics2D.CircleCast(position, settings.boundsRadius, forward, settings.collisionAvoidDst, settings.obstacleMask);
        return hit.collider != null;
    }

    Vector2 ObstacleRays()
    {
        Vector2[] rayDirections = BoidHelper2D.directions;
        int totalDirections = rayDirections.Length;

        for (int i = 0; i < totalDirections; i++)
        {
            //This is to alternate ray directions to either side of the forward direction
            int index = i % 2 == 0 ? i / 2 : totalDirections - 1 - (i / 2);

            Vector2 dir = cachedTransform.TransformDirection(rayDirections[index]);
            dir.Normalize();
            

            // Use CircleCast for 2D raycasting, which simulates casting a circle along a line in 2D space
            RaycastHit2D hit = Physics2D.CircleCast(position, settings.boundsRadius, dir, settings.collisionAvoidDst, settings.obstacleMask);
            if (hit.collider == null)  // No collision means a safe direction
            {
                return dir;
            }
        }

        return forward; // If no safe directions, default to current forward direction
    }

    Vector2 SteerTowards(Vector2 vector)
    {
        Vector2 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector2.ClampMagnitude(v, settings.maxSteerForce);
    }
}