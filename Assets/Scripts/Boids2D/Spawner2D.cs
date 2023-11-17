using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Spawner2D : MonoBehaviour
{

    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid2D prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color colour;
    public GizmoType showSpawnRegion;

    public BoidSettings2D settings;
    public BoidManager2D manager;

    public bool randomSpawn;
    public Vector2 startDirection;

    public bool spawnRepeatedly;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnRepeatedly || !hasSpawned)
            {
                Vector2 spawnDirection = randomSpawn ? Random.insideUnitCircle.normalized : startDirection;
                Spawner(spawnDirection);
                manager.InitializeBoids();
                hasSpawned = true; // Set the flag to true after spawning
            }
        }
    }


    private void Spawner(Vector2 startDirection)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = new Vector3(randomInCircle.x, randomInCircle.y, 0) + transform.position;

            Boid2D boid = Instantiate(prefab);
            boid.transform.position = pos;

            // Set the boid's direction to the specified start direction
            boid.transform.up = new Vector3(startDirection.x, startDirection.y, 0);

            boid.SetColour(colour);

            // This is crucial. You must initialize each boid with the proper settings.
            Transform target = boid.transform;

            boid.Initialize(settings, target);
        }
    }


    void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {
        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

}
