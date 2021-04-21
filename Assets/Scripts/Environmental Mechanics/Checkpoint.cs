using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameObject player;
    PlayerStatusEffects playerStatusEffects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

            if (playerStatusEffects.respawnPosition.x < transform.position.x)
            {
                playerStatusEffects.respawnPosition = transform.position;
            }
        }
    }
}
