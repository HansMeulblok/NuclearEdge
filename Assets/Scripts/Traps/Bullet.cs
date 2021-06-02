using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IOnEventCallback
{
    private Vector2 moveDirection;
    private float moveSpeed;

    private const int bulletDestroyCode = 7;

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
        if (eventCode == bulletDestroyCode)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            int objectViewID = (int)tempObject[0];

            if (objectViewID == photonView.ViewID)
            {
                print("Destroying bullet with id [" + objectViewID + "]");
                Destroy();
            }
        }
    }

    private void DestoyBulletEvent(int objectViewID)
    {
        object[] content = new object[] { objectViewID }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(bulletDestroyCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                collision.gameObject.GetComponent<PlayerStatusEffects>().isStunned = true;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            DestoyBulletEvent(photonView.ViewID);
            print("Sending event to destroy bullet with id [" + photonView.ViewID + "]");
        }

        Destroy();
    }

    // Variables that are set in the shooting method in the Cannon.
    public void SetBulletProperties(Vector2 dir, float speed, float lifeSpan)
    {
        moveDirection = dir;
        moveSpeed = speed;

        //if (PhotonNetwork.IsMasterClient) { Invoke("DestoyBulletEvent", lifeSpan); }
    }

    private void Destroy()
    {
       gameObject.SetActive(false);
    }
}
