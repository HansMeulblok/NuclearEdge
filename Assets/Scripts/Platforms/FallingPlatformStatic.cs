using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//[ExecuteInEditMode]
public class FallingPlatformStatic : MonoBehaviourPun, IOnEventCallback
{
    [Header("Falling variables")]
    public float turningOffTime = 0.5f;
    public float fallingDownDur = 2;
    public float fallingSpeed = 5;
    public float maxDelayTime = 5;

    private float startTime;
    private float timer;

    private bool steppedOn = false;
    private bool canFall = false;
    private bool isActivated = false;

    private SpriteRenderer spriteHolder;
    private BoxCollider2D platformCollider;
    private GameObject newPlatform;

    private const int staticPlatformCode = 4;

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    } 

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == staticPlatformCode)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];
            float serverTime = (float)tempObject[1];

            if (objectName == gameObject.name)
            {
                startTime = serverTime;
                steppedOn = true;
            }
        }
    }

    private void ActivateFallingPlatform()
    {
        object[] content = new object[] { gameObject.name, (float)PhotonNetwork.Time }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(staticPlatformCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void Start()
    {
        spriteHolder = GetComponentInChildren<SpriteRenderer>();
        platformCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Check if the player is on top of the platform
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f) && !isActivated)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            PhotonView pv = hit.transform.gameObject.GetComponent<PhotonView>();

            if (hit.transform.tag == "Player" && !canFall && pv.IsMine)
            {
                ActivateFallingPlatform();
                isActivated = true;
            }
        }

        // Start timer if the platform is stepped on
        if (steppedOn)
        {
            timer = (float)(PhotonNetwork.Time - startTime);

            // If receiving event took longer than delay time, skip falling
            if (timer >= maxDelayTime)
            {
                steppedOn = isActivated = false;
                timer = 0;
            }
            else
            {
                canFall = true;
            }
        }

        // If timer is maxed out start falling
        if (timer >= turningOffTime && timer < maxDelayTime)
        {
            newPlatform = ObjectPooler.Instance.SpawnFromPool("FallingPlatformMoving", transform.position, Quaternion.identity);
            newPlatform.GetComponent<FallingPlatformMoving>().SetValues(canFall, fallingSpeed, fallingDownDur);
            SwitchStaticPlatform(false);

            // Reset trigger of moving platform
            steppedOn = false;
            timer = 0;

            // Reset static platform when moving platform is done
            Invoke("ResetStaticPlatform", fallingDownDur);
        }
    }

    //reset the platform after a while
    private void ResetStaticPlatform()
    {
        SwitchStaticPlatform(true);
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
        isActivated = false;
    }

    public void SwitchStaticPlatform(bool isActive)
    {
        if (isActive)
        {
            spriteHolder.enabled = true;
            platformCollider.enabled = true;
        }
        else
        {
            spriteHolder.enabled = false;
            platformCollider.enabled = false;
        }
    }
}
