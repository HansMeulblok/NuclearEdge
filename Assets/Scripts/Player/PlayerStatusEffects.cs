using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class PlayerStatusEffects : MonoBehaviourPunCallbacks
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
    public bool isStunned;
    public bool leveltesting = false;
    public Vector2 respawnPosition;

    [Header("Stun Modifiers")]
    public float stunMovementModifier;
    public float stunJumpModifier;
    public float blinkInterval;
    public float stunDuration;

    float stunTimer;

    Rigidbody2D rb;

    PlayerMovement2D playerMovement;
    SpriteRenderer statusVisual, playerSprite;
    float originalMaxSpeed;
    float originalJumpStrength;
    bool originalcanWallJump;
    bool canBlink = false;
    bool isInvincible = false;

    private const int slowCode = 8;
    private const int stunCode = 9;

    private void Start()
    {
        // Disable script if player is not the local player.
        //if (photonView != null && !photonView.IsMine) { enabled = false; }


        rb = gameObject.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<PlayerMovement2D>();
        statusVisual = GetComponentsInChildren<SpriteRenderer>()[1];
        playerSprite = GetComponent<SpriteRenderer>();

        originalMaxSpeed = playerMovement.maxSpeed;
        originalJumpStrength = playerMovement.jumpStrenght;
        originalcanWallJump = playerMovement.canWallJump;

        // For testing purposes only, this should be changed to the starting location of the level
        respawnPosition = transform.position;

       
    }

    private void Update()
    {
        // Temp debug code to kill the player
        if (Input.GetKeyDown(KeyCode.R) && leveltesting)
        {
            isDead = true;
        }

        if (isDead && leveltesting)
        {
            ResetPlayer();
        }
        else
        {
            //permanent death
            // Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Slow debuff
        if (slowed && photonView.IsMine)
        {
            // raise event
            RaiseEvent(photonView.ViewID, slowCode, true);
            statusVisual.enabled = true;

            if (!movementChanged)
            {
                playerMovement.maxSpeed *= slowMovementModifier;
                playerMovement.jumpStrenght *= slowJumpModifier;
                playerMovement.canWallJump = false;
            }

            movementChanged = true;

            if (slowedTimer > 0 && !inSludge) { slowedTimer -= Time.deltaTime; }
            else if (slowedTimer <= 0)
            {
                ResetStats();
            }
        }

        if (isStunned && photonView.IsMine)
        {
            // Blink sprite
            if (!isInvincible)
            {
                // Raise event
                RaiseEvent(photonView.ViewID, stunCode, true);
                //StartBlinking();
                isInvincible = true;
                playerMovement.KnockBack();
            }

            // Slow player

            if (!movementChanged)
            {
                playerMovement.maxSpeed *= stunMovementModifier;
                playerMovement.jumpStrenght *= stunJumpModifier;
                playerMovement.canWallJump = false;
            }

            movementChanged = true;

            stunTimer += Time.deltaTime;

            if (stunTimer >= stunDuration)
            {
                // Raise event
                RaiseEvent(photonView.ViewID, stunCode, false);
                //StopBlinking();
                stunTimer = 0;
                canBlink = false;
                isStunned = false;
                isInvincible = false;
                ResetStats();
            }
        }
    }

    private void StartBlinking()
    {
        InvokeRepeating("Blinking", 0, blinkInterval);
    }

    private void StopBlinking()
    {
        CancelInvoke("Blinking");
    }
    private void ResetPlayer()
    {
        isDead = false;

        // TODO: Reset all player debuffs
        ResetStats();
        rb.velocity = Vector3.zero;
        transform.position = respawnPosition;
    }

    private void ResetStats()
    {
        // Reset player visuals
        statusVisual.enabled = false;
        playerSprite.color = Color.green;

        // Reset player movement stats
        playerMovement.maxSpeed = originalMaxSpeed;
        playerMovement.jumpStrenght = originalJumpStrength;
        playerMovement.canWallJump = originalcanWallJump;

        // Reset status effect checks
        slowedTimer = 0;
        movementChanged = false;
        slowed = false;
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void RaiseEvent(int id, int networkCode, bool isActivated)
    {
        object[] content = new object[] { id };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent((byte)networkCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if(eventCode == slowCode)
        {

        }

        if(eventCode == stunCode)
        {
            //if(stunCode)
        }
    }

    private void Blinking()
    {
        canBlink = !canBlink;
        if (canBlink)
        {
            playerSprite.color = Color.green;
        }
        else
        {
            playerSprite.color = Color.clear;
        }
    }
}
