using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    public Transform weapon;
    public float offset;

    public Transform shotPoint;
    public GameObject projectile;

    public float timeBetweenShots;
    private float nextShotTime;
    public int maxHealth = 5;
    public int currentHealth;
    public Slider healthSlider;
    private bool movementEnabled = false;
    [SerializeField] private float movementDelayAfterLoad = 0.3f; // tweak if needed
    private float movementDelayTimer;
    private bool isDead = false;

    public bool IsInitialized => GameState.IsGameInitialized;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementDelayTimer = movementDelayAfterLoad;
        movementEnabled = false;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementEnabled)
        {
            movementDelayTimer -= Time.deltaTime;
            if (movementDelayTimer <= 0f)
            {
                movementEnabled = true;
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("isWalking", false);
                return; // Skip rest of Update
            }
        }
        if (Time.timeScale == 0) return;
        // Read input
        // every frame (polling)

        moveInput = Gamepad.current.leftStick.ReadValue();

        rb.velocity = moveInput * moveSpeed;

        // Animate every frame too
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (moveInput != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Weapon Rotation
        if (moveInput.sqrMagnitude > 0.01f) // ignore jitter
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            weapon.rotation = Quaternion.Euler(0f, 0f, angle + offset);
        }
        if (moveInput.magnitude < 0.2f) moveInput = Vector2.zero;
    }
    public void TakeDamage(int dmg)
    {
        if (isDead || !IsInitialized) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log($"Player died.");
            currentHealth = 0;
            Die();
        }
    }
    // Shooting
    public void Fire()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + timeBetweenShots;
            Instantiate(projectile, shotPoint.position, shotPoint.rotation);
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("☠️ Player has died.");
        Time.timeScale = 0f; // Pause the game
        GameOverManager.Instance.ShowGameOverPanel();
    }

}

