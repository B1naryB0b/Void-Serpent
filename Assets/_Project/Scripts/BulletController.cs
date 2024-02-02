using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10.0f;
    public int damage = 1;
    public float lifeTime = 5.0f;
    [Range(0f, 1f)]
    public float lifeTimeMultiplier;
    private float activeTime;

    public AudioClip hitEnemySound;
    public AudioClip hitPlayerSound;

    public AnimationCurve speedCurve;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        activeTime = Time.time;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime * speedCurve.Evaluate((Time.time - activeTime)/(lifeTime * lifeTimeMultiplier)));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }

        if (gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            DamageEnemy(other);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            DamagePlayer(other);
        }

    }

    private void DamageEnemy(Collider2D other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            AudioController.Instance.PlaySound(hitEnemySound);
            Destroy(gameObject);
        }
    }

    private void DamagePlayer(Collider2D other)
    {
        PlayerHP player = other.gameObject.GetComponent<PlayerHP>();
        if (player != null)
        {
            player.TakeDamage(damage);
            AudioController.Instance.PlaySound(hitPlayerSound);
            Destroy(gameObject);
        }
    }

}
