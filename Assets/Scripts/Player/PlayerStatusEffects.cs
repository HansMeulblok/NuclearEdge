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
    [Header("Player Globals")]
    public bool inSludge;
    public bool movementChanged;
    public bool isDead;
    public Vector2 respawnPosition;

    Rigidbody2D rb;

    PlayerMovement2D playerMovement;
    SpriteRenderer statusVisual;
    float originalMaxSpeed;
    float originalJumpStrength;
    bool originalcanWallJump;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<PlayerMovement2D>();
        statusVisual = GameObject.FindGameObjectWithTag("Status").GetComponent<SpriteRenderer>();

        originalMaxSpeed = playerMovement.maxSpeed;
        originalJumpStrength = playerMovement.jumpStrenght;
        originalcanWallJump = playerMovement.canWallJump;

        // For testing purposes only, this should be changed to the starting location of the level
        respawnPosition = transform.position;
    }

    private void Update()
    {
        // Temp debug code to kill the player
        if (Input.GetKeyDown(KeyCode.R))
        {
            isDead = true;
        }
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
                ResetSlowedStats();
            }
        }

        if (isDead)
        {
            ResetPlayer();   
        }
    }

    private void ResetPlayer()
    {
        isDead = false;

        // TODO: Reset all player debuffs
        ResetSlowedStats();
        rb.velocity = Vector3.zero;
        transform.position = respawnPosition;
    }

    private void ResetSlowedStats()
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
