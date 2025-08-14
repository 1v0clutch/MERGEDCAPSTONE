using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public int maxBounces = 3;

    private Rigidbody2D rb;
    private int bounceCount = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
        }

        rb.velocity = transform.up * speed;
        Destroy(gameObject, 5f); // Timeout
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        QuestionEnemy questionEnemy = collision.collider.GetComponent<QuestionEnemy>();
        if (questionEnemy != null)
        {
            questionEnemy.OnHitByProjectile();
            Destroy(gameObject);
            return;
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"💥 Hit enemy: {enemy.name}");
            }

            Destroy(gameObject); // Kill projectile on enemy hit
            return;
        }

        // Bounce logic
        Vector2 normal = collision.contacts[0].normal;
        rb.velocity = Vector2.Reflect(rb.velocity, normal);

        bounceCount++;
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
}
