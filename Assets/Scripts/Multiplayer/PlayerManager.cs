using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    List<string> deadPlayers = new List<string>();

    // [SerializeField] private Transform playerParent;
    private Vector2 networkPosition;
    private float networkRotation;

    void Awake()
    {
        //if (!photonView.IsMine)
        //{
        //    Update layer to player two
        //    foreach (Transform child in playerParent)
        //    {
        //        child.gameObject.layer = LayerMask.NameToLayer("PlayerTwo");
        //    }

        //    Update camera to splitscreen
        //    Camera camera = playerParent.GetComponentInChildren<Camera>();
        //    camera.GetComponent<AudioListener>().enabled = false;
        //    camera.cullingMask = ~(1 << LayerMask.NameToLayer("PlayerOne"));
        //    camera.rect = new Rect(0, 0, 1, 0.5f);
        //}

        playerRB = GetComponent<Rigidbody2D>();

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Sends information of player to others
        if (stream.IsWriting)
        {
            stream.SendNext(playerRB.position);
            // stream.SendNext(playerRB.rotation);
            stream.SendNext(playerRB.velocity);

        }
        else
        {
            // Receives information of player to others
            networkPosition = (Vector2)stream.ReceiveNext();
            // networkRotation = (float)stream.ReceiveNext();
            playerRB.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += playerRB.velocity * lag;
        }
    }

    private void FixedUpdate()
    {
        // Updates position of others.
        if (!photonView.IsMine)
        {
            playerRB.position = Vector2.MoveTowards(playerRB.position, networkPosition, Time.fixedDeltaTime);

            // Uncomment this and networkRotation variable (above) to enable rotation sync
            // playerRB.MoveRotation(networkRotation + Time.fixedDeltaTime * 100.0f); 
        }
    }

    private void Update()
    {
        // Remove player from target list and disable player when out of camera FoV
        if (isRendered && !playerSprite.isVisible && photonView.IsMine)
        {
            multiTargetCamera.targets.Remove(transform);
            string playerId = PhotonNetwork.LocalPlayer.UserId;
            deadPlayers.Add(playerId);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers} });
            //gameObject.SetActive(false);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["DeadPlayers"] != null)
        {
            deadPlayers = (List<string>)propertiesThatChanged["DeadPlayers"];
            foreach (string id in deadPlayers)
            {
                //if(deadPlayers.Contains == Photo)
            }
        };
    }


    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }
}
