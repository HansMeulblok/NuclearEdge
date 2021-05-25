using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

[ExecuteInEditMode]
public class FallingPlatformStatic : MonoBehaviourPun, IOnEventCallback
{
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    [Header("Falling variables")]
    public float maxTime = 2;
    public float fallingSpeed = 4;
    public float maxDelayTime = 5;

    private float startTime;
    private float timer;

    private bool steppedOn = false;
    private bool canFall = false;
    private bool isActivated = false;

    private GameObject newPlatform;
    private const int fallingPlatformCode = 2;

    private void Update()
    {
        // Check if the player is on top of the platform
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f) && !isActivated)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            PhotonView pv = hit.transform.gameObject.GetComponent<PhotonView>();

            if (hit.transform.tag == "Player" && !canFall && pv.IsMine)
            {
                activateFallingPlatform();
                isActivated = true;
            }
        }

        // Start timer if the platform is stepped on
        if (steppedOn)
        {
            canFall = true;
            timer = (float)(PhotonNetwork.Time - startTime);
            print(timer);
        }

        // If timer is maxed out start falling
        if (timer >= maxTime && timer < maxDelayTime)
        {
            newPlatform = ObjectPooler.Instance.SpawnFromPool("FallingPlatformMoving", transform.position, Quaternion.identity);
            newPlatform.GetComponent<FallingPlatformMoving>().SetValues(canFall, fallingSpeed, maxTime);

            // Reset trigger of moving platform
            steppedOn = false;
            timer = 0;

            // Reset static platform when moving platform is done
            Invoke("ResetStaticPlatform", maxTime);
        }
        // Delay too long, so skip
        else
        {
            steppedOn = isActivated = false;
            timer = 0;
        }
    }

    //reset the platform after a while
    private void ResetStaticPlatform()
    {
        SwitchStaticPlatform(true);
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
    }

    private void activateFallingPlatform()
    {
        object[] content = new object[] { gameObject.name, (float)PhotonNetwork.Time, false }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(fallingPlatformCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == fallingPlatformCode)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];
            float serverTime = (float)tempObject[1];
            bool isActive = (bool)tempObject[2];

            print("Object: " + objectName + ", setting " + isActive + ". Trying to acces " + gameObject.name);

            if (objectName == gameObject.name)
            {
                print("Switching platform...");
                SwitchStaticPlatform(isActive);
                startTime = serverTime;
                steppedOn = true;
                isActivated = isActive;
            }
        }
    }

    public void SwitchStaticPlatform(bool isActive)
    {
        if (isActive)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
