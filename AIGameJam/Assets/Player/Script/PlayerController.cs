using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]

[DisallowMultipleComponent]

public class PlayerController : MonoBehaviour
{
    public GameObject Win;
    public GameObject File;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] [Range(0, 1)] private float isGroundedRadius = 0.1f;
    [SerializeField] [Range(0, 1)] private float stopAttackTime = 0.7f;
    [SerializeField] private float jumpForceX = 5f;
    [SerializeField] private float jumpForceY = 10f;

    public float health = 100f;

    private SpriteRenderer spriteRenderer;
    private Transform groundCheck;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private float horizontalInput;
    private bool isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
            Move();
        }
        else
        {
            isRunning = false;
            animator.SetFloat("Speed", 0);
        }

        Jump();
        Health();
        Attack();
    }

    public void Bounce(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * jumpForceX, jumpForceY);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            int randomAttackIndex = Random.Range(1, 4);
            isAttacking = true;

            animator.SetInteger("Attack", randomAttackIndex);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            StartCoroutine(AttackCooldown());

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.tag == "Slime")
                {
                    enemy.GetComponent<SlimeController>().TakeDamage(5);
                }

                if (enemy.gameObject.tag == "Golem")
                {
                    enemy.GetComponent<GolemController>().TakeDamage(10);
                }

                if (enemy.gameObject.tag == "Pixie")
                {
                    enemy.GetComponent<PixeiController>().TakeDamage(10);
                }

                if (enemy.gameObject.tag == "Dragon")
                {
                    enemy.GetComponent<DragonController>().TakeDamage(20);
                }

                if (enemy.gameObject.tag == "Dwarf")
                {
                    enemy.GetComponent<DwarfController>().TakeDamage(10);
                } 

                if (enemy.gameObject.tag == "Leprikon")
                {
                    enemy.GetComponent<LeprikonController>().TakeDamage(30);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(stopAttackTime);
        isAttacking = false;
        animator.SetInteger("Attack", 0);
    }

    void Health()
    {
        if (health <= 0)
        {
            animator.SetBool("Dead", true);
            Debug.Log("Öldü");
            Invoke("Dead", 2f);
        }
    }

    public void Hurt(bool isHurt)
    {
        if (isHurt)
        {
            animator.SetBool("Hurt", true);
        }
        else
        {
            animator.SetBool("Hurt", false);
        }
    }

    void Dead()
    {
        Time.timeScale = 0f;
    }

    void Move()
    {
        float moveSpeed = isRunning ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }

        animator.SetFloat("Speed", moveSpeed);
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, isGroundedRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            animator.SetBool("Jump", true);
        }

        if (isJumping && rb.velocity.y <= 0)
        {
            isJumping = false;
            animator.SetBool("Jump", false);
        }
    }
}
