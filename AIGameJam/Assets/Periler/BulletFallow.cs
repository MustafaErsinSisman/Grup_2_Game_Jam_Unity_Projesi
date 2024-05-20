using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFallow : MonoBehaviour
{
    private GameObject target;
    public float damage = 5f;

    void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (target != null)
        {
            float speed = 5f; // Hareket hızını belirleyin
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            playerController.health -= damage;

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
