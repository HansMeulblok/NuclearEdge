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
    [Header("Status Modifiers")]
    public float slowMovementModifier;
    public float slowJumpModifier;

    // Globals
    [Header("Status Checks")]
    public bool inSludge;
    public bool movementChanged;

    PlayerMovement2D playerMovement;
    SpriteRenderer statusVisual;
    float originalMaxSpeed;
    float originalJumpStrength;
    bool originalcanWallJump;

    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement2D>();
        statusVisual = GameObject.FindGameObjectWithTag("Status").GetComponent<SpriteRenderer>();
        originalMaxSpeed = playerMovement.maxSpeed;
        originalJumpStrength = playerMovement.jumpStrenght;
        originalcanWallJump = playerMovement.canWallJump;
    }

    private void FixedUpdate()
    {
        // Slow debuff
        if (slowed)
        {
            statusVisual.enabled = true;

            if (!movementChanged) { 
                playerMovement.maxSpeed *= slowMovementModifier;
                playerMovement.jumpStrenght *= slowJumpModifier;
                playerMovement.canWallJump = false;
            }

            movementChanged = true;

            if (slowedTimer > 0 && !inSludge) { slowedTimer -= Time.deltaTime; }
            else if (slowedTimer <= 0)
            {
                // Reset player visuals
                statusVisual.enabled = false;

                // Reset player movement stats
                playerMovement.maxSpeed = originalMaxSpeed;
                playerMovement.jumpStrenght = originalJumpStrength;
                playerMovement.canWallJump = originalcanWallJump;

                // Reset status effect checks
                slowedTimer = 0;
                movementChanged = false;
                slowed = false;
            }
        }
    }
}
