using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10.0f;       // Speed of the bullet
    public int damage = 1;            // Damage dealt by the bullet
    public float lifeTime = 5.0f;     // Time after which the bullet will be destroyed

    public AudioClip hitEnemySound;     // Sound effect when bullet hits an enemy
    public AudioClip hitPlayerSound;    // Sound effect when bullet hits the player

    private void Start()
    {
        // Destroy the bullet after a certain time to prevent memory leaks
        Destroy(gameObject, lifeTime);
    }


    // Update is called once per frame
    private void Update()
    {
        // Move the bullet in its forward direction
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }

        // If this bullet is a player's bullet, it should only damage enemies
        if (gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                AudioController.Instance.PlaySound(hitEnemySound);
                Destroy(gameObject);
            }
        }
        // If this bullet is an enemy's bullet, it should only damage the player
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
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

    


}
