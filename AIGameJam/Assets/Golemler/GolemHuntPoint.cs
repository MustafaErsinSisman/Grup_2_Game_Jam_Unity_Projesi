using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemHuntPoint : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector2 direction = collision.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
                playerController.Bounce(direction);
            }
        }
    }
}
