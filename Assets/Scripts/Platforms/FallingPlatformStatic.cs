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
    private float timer;
    private bool steppedOn = false;
    private bool canFall = false;
    private bool isPlatformActive;
    public float resetTime;

    private const int fallingPlatformCode = 2;


    private void Update()
    {

        //start timer if the platform is stepped on
        if (steppedOn)
        {
            timer += Time.deltaTime;
        }

        //if timer is maxed out start falling
        if(timer >= maxTime)
        {
            canFall = true;
            steppedOn = false;
            timer = 0;
            // rpc set this platform invis
            activateFallingPlatform();
            // get platform from pool 
            GameObject newPlatform = ObjectPooler.Instance.SpawnFromPool("FallingPlatform", transform.position, Quaternion.identity);

            // start translating pooled object down
            newPlatform.GetComponent<FallingPlatformMoving>().SetValues(canFall, fallingSpeed, maxTime);
            // after x amount of time or distance pooledobject setactive(false)
            // rpc set this platform active
            Invoke("ResetPlatform", maxTime);
        }



        //check if the player is on top of the platform
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            if (hit.transform.tag == "Player" && !canFall)
            {
                steppedOn = true;
            }
        }

    }

    //reset the platform after a while
    private void ResetPlatform()
    {
        // transform.gameObject.SetActive(true);
        SwitchStaticPlatform();
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;

    }


    private void activateFallingPlatform()
    {
        object[] content = new object[] { gameObject.name };
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

            if (receivedObj == gameObject.name)
            {
                SwitchStaticPlatform();
            }
        }
    }


    public void SwitchStaticPlatform()
    {
        isPlatformActive = !isPlatformActive;
        if(isPlatformActive)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
        }       
    }

}
