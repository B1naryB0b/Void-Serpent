using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings2D : ScriptableObject
{
    // Settings
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float maxSteerForce = 3;

    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float separateWeight = 1;

    public float targetWeight = 1;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f; // This might represent a 2D circle's radius if you're using circle colliders.
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5; // This distance might need adjustment based on 2D space perception.

    // You may want to add other settings specific to 2D here.
}

