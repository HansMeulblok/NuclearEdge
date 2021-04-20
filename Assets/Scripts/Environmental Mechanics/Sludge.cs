using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sludge : MonoBehaviour

{
    [SerializeField]
    float slowModifier = 0.5f;
    [SerializeField]
    float duration = 2f;

    GameObject player;
    PlayerStatusEffects playerStatusEffects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

            playerStatusEffects.slowed = true;
            playerStatusEffects.inSludge = true;
            playerStatusEffects.slowModifier = slowModifier;
            playerStatusEffects.slowedTimer = duration;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player = collision.gameObject;
        playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

        playerStatusEffects.inSludge = false;
    }
}