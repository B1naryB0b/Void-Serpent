using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public float attractionSpeed = 10f; // Base attraction speed
    public float maxAttractionDistance = 5f; // Maximum distance at which the attraction is applied
    public LayerMask collectableLayer; // Layer of the collectable items
    public float destroyDelay = 1f; // Delay before destroying the collectable

    public AudioClip collectClip;

    public RampingController rampingController;

    public float rampingPerCollectable;

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetButton("Fire1"))
        {
            AttractCollectables();
        }
    }

    void AttractCollectables()
    {
        // Find all collectables within the max attraction distance
        Collider2D[] collectables = Physics2D.OverlapCircleAll(transform.position, maxAttractionDistance, collectableLayer);

        foreach (Collider2D collectable in collectables)
        {
            // Calculate the direction from the collectable to the player
            Vector2 directionToPlayer = (transform.position - collectable.transform.position).normalized;

            // Calculate the distance from the collectable to the player
            float distanceToPlayer = Vector2.Distance(transform.position, collectable.transform.position);

            // Calculate the attraction speed based on the distance (the closer the collectable, the faster it moves)
            float velocityMagnitude = attractionSpeed * (1 - Mathf.Pow(distanceToPlayer / maxAttractionDistance, 2));

            // Set the velocity of the collectable to move towards the player
            collectable.GetComponent<Rigidbody2D>().velocity = directionToPlayer * velocityMagnitude;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is in the collectable layer
        if (collectableLayer == (collectableLayer | (1 << collision.gameObject.layer)))
        {
            AudioController.Instance.PlaySound(collectClip, 0.2f);

            rampingController.IncreaseRamping(rampingPerCollectable);
            // Hide the sprite of the collectable
            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            if (collision != null)
            {
                collision.enabled = false;
            }

            // Start the coroutine to lerp the trail renderer time and destroy the collectable after a short delay
            StartCoroutine(LerpTrailAndDestroy(collision.gameObject, destroyDelay));
        }
    }

    IEnumerator LerpTrailAndDestroy(GameObject obj, float delay)
    {
        TrailRenderer trail = obj.GetComponent<TrailRenderer>();
        float initialTime = trail.time;
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(obj.transform.position, transform.position, elapsedTime / delay);
            trail.time = Mathf.Lerp(initialTime, 0, elapsedTime / delay);
            yield return null;
        }

        Destroy(obj);
    }

}
