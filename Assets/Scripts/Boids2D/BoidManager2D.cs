using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BoidManager2D : MonoBehaviour
{

    const int threadGroupSize = 1024;

    public BoidSettings2D settings;
    public ComputeShader compute; // Note: Compute shaders work with 3D textures and buffers, not specifically 2D or 3D game logic.
    public Transform target;
    Boid2D[] boids;

    void Start()
    {
        InitializeBoids();
    }

    public void InitializeBoids()
    {
        boids = FindObjectsOfType<Boid2D>();
        foreach (Boid2D b in boids)
        {
            b.Initialize(settings, target);
        }
    }

    void Update()
    {
        if ((boids != null) && (boids.Length > 0))
        {
            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            // Extract boid data from boid objects.
            for (int i = 0; i < boids.Length; i++)
            {
                boidData[i].position = new Vector2(boids[i].position.x, boids[i].position.y); // Adjusted to 2D
                boidData[i].direction = new Vector2(boids[i].forward.x, boids[i].forward.y); // Adjusted to 2D
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);
            // Apply computed data back to the boid objects.
            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].avgFlockHeading = new Vector3(boidData[i].flockHeading.x, boidData[i].flockHeading.y, 0); // Adjusted to 2D
                boids[i].centreOfFlockmates = new Vector3(boidData[i].flockCentre.x, boidData[i].flockCentre.y, 0); // Adjusted to 2D
                boids[i].avgAvoidanceHeading = new Vector3(boidData[i].avoidanceHeading.x, boidData[i].avoidanceHeading.y, 0); // Adjusted to 2D
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release(); // Clean up the compute buffer.
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BoidData
    {
        public Vector2 position;
        public Vector2 direction;

        public Vector2 flockHeading;
        public Vector2 flockCentre;
        public Vector2 avoidanceHeading;
        public int numFlockmates;

        // Padding to ensure 16-byte size sections (for 4 float values), which is standard for shader data structures
        public float _padding; // or more depending on your actual shader structure needs

        public static int Size
        {
            get
            {
                // Account for the new padding in the byte size calculation.
                return sizeof(float) * 2 * 5 + sizeof(int) + sizeof(float) * 1; // Adjusted for Vector2 and padding
            }
        }
    }

}
