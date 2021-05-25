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

    private const int movingPlatformCode = 5;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        platformEditor = GetComponent<PlatformEditor>();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; };
        if (isFalling)
        {
            timer += Time.deltaTime;
            //transform.Translate(Vector2.down * (fallingSpeed * Time.deltaTime), Space.World);
            rb.MovePosition(transform.position - new Vector3(0, 10, 0) * fallingSpeed * Time.deltaTime);
        }

        if (timer >= maxTime)
        {
            timer = 0;
            TurnOffPlatform();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == movingPlatformCode)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];

            if (objectName == gameObject.name)
            {
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
        object[] content = new object[] { gameObject.name }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(movingPlatformCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
