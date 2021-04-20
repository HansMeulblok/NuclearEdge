using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    // Player status effects
    [Header("Status effects")]
    public bool slowed;

    // Player status timers
    [Header("Status Timers")]
    public float slowedTimer;

    // Player status modifiers
    [Header("Status Timers")]
    public float slowModifier;

    // Globals
    public bool inSludge;
    public bool movementChanged;

    PlayerMovement2D playerMovement;
    float originalMaxSpeed;
    float originalJumpStrength;

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement2D>();
        originalMaxSpeed = playerMovement.maxSpeed;
        originalJumpStrength = playerMovement.jumpStrenght;
    }

    private void FixedUpdate()
    {
        // Slow debuff
        if (slowed)
        {
            if (!movementChanged) { 
                playerMovement.maxSpeed *= slowModifier;
                playerMovement.jumpStrenght *= slowModifier + .5f;
            }

            movementChanged = true;

            if (slowedTimer > 0 && !inSludge) { slowedTimer -= Time.deltaTime; }
            else if (slowedTimer <= 0)
            {
                // Reset player movement stats
                playerMovement.maxSpeed = originalMaxSpeed;
                playerMovement.jumpStrenght = originalJumpStrength;

                // Reset status effect checks
                slowedTimer = 0;
                movementChanged = false;
                slowed = false;
            }
        }
    }
}
