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
    float eventTimeStamp;
    bool originalcanWallJump;
    bool canBlink = false;
    bool isInvincible = false;
    bool isSlowed = false;
    Color playerColor;

    private const int slowCode = 8;
    private const int stunCode = 9;

    private void Start()
    {
        // Disable script if player is not the local player.
        //if (photonView != null && !photonView.IsMine) { enabled = false; }

        playerColor = GetComponentsInChildren<SpriteRenderer>()[1].color;

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

        // Slow debuff
        if (slowed)
        {
            // raise event
            if (!isSlowed)
            {
                RaiseEvent(photonView.ViewID, slowCode, true);
                isSlowed = true;
            }
            statusVisual.enabled = true;

            if (!movementChanged)
            {
                playerMovement.maxSpeed *= slowMovementModifier;
                playerMovement.jumpStrenght *= slowJumpModifier;
                playerMovement.canWallJump = false;
            }

            movementChanged = true;

            if (slowedTimer > 0 && !inSludge)
            { 
                if(photonView.IsMine)
                {
                    slowedTimer -= Time.deltaTime;
                }
                else
                {
                    slowedTimer -= (float)PhotonNetwork.Time - eventTimeStamp;
                }
            }
            else if (slowedTimer <= 0 && isSlowed)
            {
                RaiseEvent(photonView.ViewID, slowCode, false);
                ResetStats();
            }
        }

        if (isStunned)
        {
            // Blink sprite
            if (!isInvincible)
            {
                // Raise event
                RaiseEvent(photonView.ViewID, stunCode, true);
                StartBlinking();
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
                StopBlinking();
                ResetStats();
                stunTimer = 0;
                canBlink = false;
                isStunned = false;
                isInvincible = false;
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
        playerSprite.color = playerColor;

        // Reset player movement stats
        playerMovement.maxSpeed = originalMaxSpeed;
        playerMovement.jumpStrenght = originalJumpStrength;
        playerMovement.canWallJump = originalcanWallJump;

        // Reset status effect checks
        slowedTimer = 0;
        movementChanged = false;
        slowed = false;
        isSlowed = false;
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
        // raise event and give photon id, network byte and a timestamp from when it was send
        object[] content = new object[] { id, isActivated, (float)PhotonNetwork.Time};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)networkCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if(eventCode == slowCode || eventCode == stunCode)
        {
            // continue if received the right codes otherwise return;
        }
        else { return; }

        object[] tempObjects = (object[])photonEvent.CustomData;
        int photonId = (int)tempObjects[0];
        bool activate = (bool)tempObjects[1];
        eventTimeStamp = (float)tempObjects[2];
        // grab objects from array

        float timeDif = (float)PhotonNetwork.Time - eventTimeStamp;
        // calculcate time difference between the time received and current server time 

        if(eventCode == slowCode && photonView.ViewID == photonId && timeDif <= (slowedTimer))
        {
            // enable or disable effect
            if (activate)
            {
                statusVisual.enabled = true;
            }
            else
            {
                ResetStats();
            }
        }

        if(eventCode == stunCode && photonView.ViewID == photonId && timeDif <= (stunTimer))
        {
            // enable or disable effect
            if (activate)
            {
                StartBlinking();
            }
            else
            {
                StopBlinking();
                ResetStats();
            }
        }
    }

    private void Blinking()
    {
        canBlink = !canBlink;
        if (canBlink)
        {
            playerSprite.color = playerColor;
        }
        else
        {
            playerSprite.color = Color.clear;
        }
    }
}
