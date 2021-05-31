using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

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
    public bool isStunned;
    public Vector2 respawnPosition;

    [Header("Stun Modifiers")]
    public float stunMovementModifier;
    public float stunJumpModifier;
    public float blinkInterval;
    public float stunDuration;

    // If the current server time - the event time that was received is greater than maxSyncTime, some methods need not be activated.
    [Header("Sync")]
    public float maxSyncTime = 4;

    float stunTimer;

    PlayerMovement2D playerMovement;
    SpriteRenderer statusVisual, playerSprite;
    Color playerColor;
    float originalMaxSpeed;
    float originalJumpStrength;
    float eventTimeStamp;
    bool originalcanWallJump;
    bool canBlink = false;
    bool isInvincible = false;
    bool isSlowEventCalled = false;
    

    // Byte codes
    private const int slowCode = 8;
    private const int stunCode = 9;

    private void Start()
    {
        // Get colours a little later because of load time
        Invoke("GetColours", 2f);

        playerMovement = gameObject.GetComponent<PlayerMovement2D>();
        statusVisual = GetComponentsInChildren<SpriteRenderer>()[1];
        playerSprite = GetComponent<SpriteRenderer>();

        originalMaxSpeed = playerMovement.maxSpeed;
        originalJumpStrength = playerMovement.jumpStrenght;
        originalcanWallJump = playerMovement.canWallJump;
    }

    private void Update()
    {
        // Slow debuff
        if (slowed)
        {
            // Raise event
            if (!isSlowEventCalled)
            {
                RaiseEvent(photonView.ViewID, slowCode, true);
                isSlowEventCalled = true;
            }
            // Turn on effect locally
            statusVisual.enabled = true;

            // Apply slow modifiers
            if (!movementChanged)
            {
                playerMovement.maxSpeed *= slowMovementModifier;
                playerMovement.jumpStrenght *= slowJumpModifier;
                playerMovement.canWallJump = false;
            }
            movementChanged = true;

            // Check if your are the local player otherwise use the server time
            if (slowedTimer > 0 && !inSludge)
            {
                slowedTimer -= Time.deltaTime;
            }
            else if (slowedTimer <= 0 && isSlowEventCalled)
            {
                // Turn off effect for all players
                RaiseEvent(photonView.ViewID, slowCode, false);
                ResetStats();
            }
        }


        // Stun debuff
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


            // Apply slow modifiers

            if (!movementChanged)
            {
                playerMovement.maxSpeed *= stunMovementModifier;
                playerMovement.jumpStrenght *= stunJumpModifier;
                playerMovement.canWallJump = false;
            }

            movementChanged = true;

            // Start timer
            stunTimer += Time.deltaTime;

            if (stunTimer >= stunDuration)
            {
                // Raise event
                RaiseEvent(photonView.ViewID, stunCode, false);
                StopBlinking();
                ResetStats();
                stunTimer = 0;           
            }
        }
    }

    private void StartBlinking()
    {
        // Repeat blinking effect
        InvokeRepeating("Blinking", 0, blinkInterval);
    }

    private void StopBlinking()
    {
        // Stop blinking effect
        CancelInvoke("Blinking");
    }

    private void GetColours()
    {
        // Get current player colour
        playerColor = GetComponent<SpriteRenderer>().color;
    }


    private void ResetStats()
    {
        // Reset player visuals
        statusVisual.enabled = false;
        playerSprite.color = playerColor;
        StopBlinking();

        // Reset player movement stats
        playerSprite.color = playerColor;
        playerMovement.maxSpeed = originalMaxSpeed;
        playerMovement.jumpStrenght = originalJumpStrength;
        playerMovement.canWallJump = originalcanWallJump;

        // Reset status effect checks
        slowed = false;
        slowedTimer = 0;
        movementChanged = false;
        isSlowEventCalled = false;
        canBlink = false;
        isStunned = false;
        isInvincible = false;
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
        // Raise event and give photon id, network byte and a timestamp from when it was send
        object[] content = new object[] { id, isActivated, (float)PhotonNetwork.Time};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)networkCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        // Continue if received the right codes otherwise return;
        if (eventCode != slowCode && eventCode != stunCode)
        {
            return;
        }

        // Grab objects from array
        object[] tempObjects = (object[])photonEvent.CustomData;
        int photonId = (int)tempObjects[0];
        bool activate = (bool)tempObjects[1];
        eventTimeStamp = (float)tempObjects[2];

        float timeDif = (float)PhotonNetwork.Time - eventTimeStamp;

        // Calculcate time difference between the time received and current server time 

        if(eventCode == slowCode && photonView.ViewID == photonId && timeDif <= maxSyncTime)
        {
            // Enable or disable effect
            if (activate)
            {
                statusVisual.enabled = true;
            }
            else
            {
                ResetStats();
            }
        }

        if(eventCode == stunCode && photonView.ViewID == photonId && timeDif <= maxSyncTime)
        {
            // Enable or disable effect
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
        // Blink the player sprite between a tranparent colour and the original colour
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
