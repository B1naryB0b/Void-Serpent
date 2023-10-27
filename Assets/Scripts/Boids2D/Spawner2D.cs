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

    void Awake()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = new Vector3(randomInCircle.x, randomInCircle.y, 0) + transform.position;

            Boid2D boid = Instantiate(prefab);
            boid.transform.position = pos;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            boid.transform.up = new Vector3(randomDirection.x, randomDirection.y, 0);

            boid.SetColour(colour);

            // This is crucial. You must initialize each boid with the proper settings.
            Transform target = boid.transform;

            boid.Initialize(settings, target);
        }
    }

    private void OnDrawGizmos()
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
        // Drawing a semi-transparent sphere to indicate the spawn area.
        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);

        // In a 2D game, it's more common to draw a circle instead of a sphere for gizmos.
        // This represents the 2D spawn area. The 'transform.position' assumes it's at the center of the circle.
        Gizmos.DrawWireSphere(transform.position, spawnRadius); // DrawWireSphere is okay here because it helps visualize a 2D circle in the 3D space of the Unity editor.
    }
}
