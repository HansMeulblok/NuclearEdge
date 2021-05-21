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
    public float maxTime;
    public float fallingSpeed;
    public float resetTime;

    private float timer;

    private bool steppedOn = false;
    private bool canFall = false;
    private bool isPlatformActive;

    private GameObject newPlatform;
    private const int fallingPlatformCode = 2;


    private void Update()
    {


        //check if the player is on top of the platform
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            PhotonView pv = hit.transform.gameObject.GetComponent<PhotonView>();

            if (hit.transform.tag == "Player" && !canFall && pv.IsMine)
            {
                activateFallingPlatform();
            }
        }

        if (!PhotonNetwork.IsMasterClient) { return; }

        //start timer if the platform is stepped on
        if (steppedOn)
        {
            newPlatform = ObjectPooler.Instance.SpawnFromPool("FallingPlatformMoving", transform.position, Quaternion.identity);
            int objectID = newPlatform.GetComponent<PhotonView>().GetInstanceID();

            timer += Time.deltaTime;
        }

        //if timer is maxed out start falling
        if (timer >= maxTime)
        {
            newPlatform.GetComponent<FallingPlatformMoving>().SetValues(canFall, fallingSpeed, maxTime);

            Invoke("ResetPlatform", maxTime);
            canFall = true;
            steppedOn = false;
            timer = 0;
        }
    }

    //reset the platform after a while
    private void ResetPlatform()
    {
        // transform.gameObject.SetActive(true);
        SwitchStaticPlatform(true);
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
    }


    private void activateFallingPlatform()
    {
        object[] content = new object[] { gameObject.name, false }; ;
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
            string receivedObj = (string)tempObject[0];
            bool isActive = (bool)tempObject[1];

            print("Object: " + receivedObj + ", setting " + isActive);

            if (receivedObj == gameObject.name)
            {
                print("Switching platform...");
                SwitchStaticPlatform(isActive);
                
                // Platform aanzetten
                if (PhotonNetwork.IsMasterClient)
                {
                    steppedOn = true;
                }
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
