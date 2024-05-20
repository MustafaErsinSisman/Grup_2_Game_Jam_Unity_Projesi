using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackDelete : MonoBehaviour
{
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, 2f);
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
