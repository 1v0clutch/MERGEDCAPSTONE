using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float speed;
    Transform player;
    Rigidbody2D rb;
    public int health;
    //public GameObject deathEffect;
    private float damageInterval = 1f; // Damage every 1 second
    private float nextDamageTime = 0f;
    private PlayerMovement playerInContact = null;
    // Start is called before the first frame update

    [Header("Chase Settings")]
    public float chaseRange = 5f; // Adjustable in inspector
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;

        if (player == null)
        {
            Debug.LogError("❌ Player not found in scene!");
        }

        GetComponent<Rigidbody2D>().gravityScale = 0;
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale == 0 || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange) return;

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 avoid = Vector2.zero;

        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 1.0f, enemyLayer);
        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector2 pushDir = transform.position - col.transform.position;
                avoid += pushDir.normalized / pushDir.magnitude;
            }
        }

        Vector2 moveDir = (direction + avoid * 0.5f).normalized;
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameState.IsGameInitialized) return;
        
        if (collision.collider.CompareTag("Player"))
        {
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(1); // Initial hit
                playerInContact = player;
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("⚠️ Player collided with enemy!");
            }
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.TakeDamage(1);
                    nextDamageTime = Time.time + damageInterval;
                    Debug.Log("⏱️ Player takes damage over time!");
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInContact = null;
        }
    }

public void TakeDamage(int damageAmount)
{
    health -= damageAmount;
    Debug.Log($"⚠️ Took {damageAmount} damage. Remaining: {health}");

    if (health <= 0)
    {
        Debug.Log("☠️ Enemy destroyed.");
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        PointController.Instance?.EnemyKilled(false);
        Destroy(gameObject);
    }
}
}
