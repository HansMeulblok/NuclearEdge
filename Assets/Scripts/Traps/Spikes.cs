using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    PlayerStatusEffects pse;
    Rigidbody2D rb;


    //When something enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
        //If it is the player
        if (other.CompareTag("Player"))
        {
            //Get the PlayerStatusEffects script from the player
            pse = other.GetComponent<PlayerStatusEffects>();
            //The player is dead
            pse.isDead = true;
        }
    }
}
