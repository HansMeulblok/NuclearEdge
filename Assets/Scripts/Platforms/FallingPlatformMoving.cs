using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class FallingPlatformMoving : MonoBehaviourPun, IOnEventCallback
{
    private bool isFalling = false;
    private float fallingSpeed;
    private float timer;
    private float maxTime;
    private Rigidbody2D rb;
    private PlatformEditor platformEditor;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformEditor = GetComponent<PlatformEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; };

        if (isFalling)
        {
            timer += Time.deltaTime;
        }

        if (timer >= maxTime)
        {
            timer = 0;
            TurnOffPlatform();
        }
    }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) { return; };

        if (isFalling)
        {
            rb.MovePosition((Vector2)transform.position + (Vector2.down * fallingSpeed * Time.fixedDeltaTime));
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.PLATFORM_MOVING)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];

            if (objectName == gameObject.name)
            {
                GetComponentInChildren<PlayerMovement2D>()?.UnParent();
                gameObject.SetActive(false);
            }
        }
    }

    public void SetValues(bool canFall, float newFallingSpeed, float newMaxTime)
    {
        fallingSpeed = newFallingSpeed;
        isFalling = canFall;
        maxTime = newMaxTime;
    }

    public void ScalePlatform(int newLength, int newHeight)
    {
        platformEditor.editing = true;
        platformEditor.platformLength = newLength;
        platformEditor.platformHeight = newHeight;
        platformEditor.editing = false;
    }

    void TurnOffPlatform()
    {
        if (!PhotonNetwork.InRoom) { return; }

        object[] content = new object[] { gameObject.name }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.PLATFORM_MOVING, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
