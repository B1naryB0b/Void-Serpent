using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private float _collectableAttractionSpeed; 
    [SerializeField] private float _maxAttractionDistance; 
    [SerializeField] private LayerMask _collectableLayer; 
    [SerializeField] private float _destroyCollectableDelay; 

    [SerializeField] private AudioClip _collectAudioClip;

    [SerializeField] private RampingController _rampingController;

    [SerializeField] private float _rampingPerCollectable;

    void Update()
    {
        Collider2D[] collectables = CheckForCollectables();
        AttractCollectables(collectables);
    }

    private Collider2D[] CheckForCollectables()
    {
        Collider2D[] collectables = Physics2D.OverlapCircleAll(transform.position, _maxAttractionDistance, _collectableLayer);
        if (collectables.Length <= 0) return null;
        return collectables;
    }

    private void AttractCollectables(Collider2D[] collectables)
    {
        if (collectables == null) return;

        foreach (Collider2D collectable in collectables)
        {
            Vector2 directionToPlayer = (transform.position - collectable.transform.position).normalized;

            float distanceToPlayer = Vector2.Distance(transform.position, collectable.transform.position);

            float velocityMagnitude = _collectableAttractionSpeed * (1 - Mathf.Pow(distanceToPlayer / _maxAttractionDistance, 2));

            collectable.GetComponent<Rigidbody2D>().velocity = directionToPlayer * velocityMagnitude;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_collectableLayer == (_collectableLayer | (1 << collision.gameObject.layer)))
        {
            AudioController.Instance.PlaySound(_collectAudioClip, 0.2f);

            _rampingController.IncreaseRamping(_rampingPerCollectable);

            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            if (collision != null)
            {
                collision.enabled = false;
            }

            StartCoroutine(Co_LerpTrailAndDestroy(collision.gameObject, _destroyCollectableDelay));
        }
    }

    private IEnumerator Co_LerpTrailAndDestroy(GameObject obj, float delay)
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
