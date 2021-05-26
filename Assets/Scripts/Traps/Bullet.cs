using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IOnEventCallback
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private float bulletLifeSpan;

    private const int bulletDestroyCode = 7;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        if (PhotonNetwork.IsMasterClient) { Invoke("DestoyBulletEvent", bulletLifeSpan); }
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
                Destroy();
            }
        }
    }

    private void DestoyBulletEvent()
    {
        object[] content = new object[] { photonView.ViewID }; ;
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
            collision.gameObject.GetComponent<PlayerStatusEffects>().isStunned = true;
        }

        DestoyBulletEvent();
    }

    // Variables that are set in the shooting method in the Cannon.
    public void SetBulletValues(Vector2 dir, float speed, float lifeSpan)
    {
        moveDirection = dir;
        moveSpeed = speed;
        bulletLifeSpan = lifeSpan;
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }
}
