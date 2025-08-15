using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class QuestionEnemy : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 1;
    public float damageInterval = 1f;

    private Transform player;
    private Rigidbody2D rb;

    private bool isFrozen = false;
    private float nextDamageTime = 0f;
    private PlayerMovement playerInContact = null;
    [Header("Chase Settings")]
    public float chaseRange = 5f; // Adjustable in inspector
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>()?.transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0 || isFrozen || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }


    public void OnHitByProjectile()
    {
        isFrozen = true;
        QuestionManager qm = FindObjectOfType<QuestionManager>();
        if (qm != null)
        {
            qm.TriggerQuestion(OnQuestionAnswered);
        }
    }

    private void OnQuestionAnswered(bool correct)
    {
        isFrozen = false;

        if (correct)
        {
            PointController.Instance?.EnemyKilled(true);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameState.IsGameInitialized) return;
        
        if (collision.collider.CompareTag("Player"))
        {
            playerInContact = collision.collider.GetComponent<PlayerMovement>();
            if (playerInContact != null)
            {
                playerInContact.TakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("⚠️ Player collided with QuestionEnemy!");
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && playerInContact != null)
        {
            if (Time.time >= nextDamageTime)
            {
                playerInContact.TakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("⏱️ Player takes damage from QuestionEnemy over time!");
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
}
