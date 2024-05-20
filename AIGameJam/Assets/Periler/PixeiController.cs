using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]

[DisallowMultipleComponent]

public class PixeiController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private string playerTag = "Player";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float targetDistance;
    [SerializeField] private float triggerDistance = 3f;
    [SerializeField] private float aPoint = -0.5f;
    [SerializeField] private float bPoint = 0.5f;
    public GameObject bullet;
    private Vector2 randomDirection;
    [SerializeField] private float changeDirectionTime = 2f;
    private float changeDirectionTimer;

    public float maxHealth = 100f;
    private float currentHealth;

    public bool isAttacking = false;

    private GameObject target;
    private Coroutine attackCoroutine;
    [SerializeField] private float attackReset = 5f;

    void Start()
    {
        target = GameObject.FindWithTag(playerTag);

        if (target == null)
        {
            Debug.LogError(playerTag + " isimli taga sahip nesne bulunamadı.");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        changeDirectionTimer = changeDirectionTime;

        currentHealth = maxHealth;

        StartCoroutine(FireBulletAtRandomIntervals());
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        if (target != null && currentHealth > 0)
        {
            targetDistance = Vector2.Distance(transform.position, target.transform.position);

            if (targetDistance <= triggerDistance)
            {
                FollowTarget();
            }
            else
            {
                RandomMovement();
            }
        }
    }

    void Die()
    {
        currentHealth = 0;
        rb.velocity = Vector2.zero;

        Debug.Log(gameObject.name + " isimli Peri öldü.");

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && !isAttacking)
        {
            isAttacking = true;

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackPlayer(collision.gameObject));
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && isAttacking)
        {
            if (currentHealth > 0)
            {
            isAttacking = false;

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            }

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.Hurt(false);
            }
        }
    }

    IEnumerator AttackPlayer(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        while (isAttacking)
        {
            if (playerController != null)
            {
                animator.SetBool("Attack", true);

                playerController.Hurt(true);
                playerController.health -= damage;
                yield return new WaitForSeconds(attackReset);
            }
            else
            {
                yield break; // Player yoksa coroutine'den çık
            }
        }
    }

    void FollowTarget()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x, 0) * speed;

        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void RandomMovement()
    {
        changeDirectionTimer -= Time.deltaTime;

        if (changeDirectionTimer <= 0)
        {
            randomDirection = new Vector2(Random.Range(aPoint, bPoint), 0).normalized;
            changeDirectionTimer = changeDirectionTime;
        }

        rb.velocity = randomDirection * (speed / 2);

        if (randomDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (randomDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }


    IEnumerator FireBulletAtRandomIntervals()
    {
        while (currentHealth > 0)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3f));

            if (targetDistance <= triggerDistance)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;

                // Instantiate pozisyonunu ayarla
                Vector2 spawnPosition = transform.position;
                spawnPosition.x += spriteRenderer.flipX ? 1f : -1f;

                GameObject instantiatedLazer = Instantiate(bullet, spawnPosition, Quaternion.identity);
                Rigidbody2D lazerRb = instantiatedLazer.GetComponent<Rigidbody2D>();
                lazerRb.velocity = new Vector2(direction.x, 0) * speed * 3;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (target != null && targetDistance <= triggerDistance && currentHealth > 0)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
