using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    private List<string> deadPlayers;

    private Vector2 networkPosition;
    private float networkRotation;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        deadPlayers = new List<string>();

        Invoke("ChangePlayersColor", 0.6f);
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
            // Receives information of other players
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
        // Add player name to list of dead player for server
        if (isRendered && !playerSprite.isVisible && photonView.IsMine)
        {
            if (!deadPlayers.Contains(PhotonNetwork.NickName)) { deadPlayers.Add(PhotonNetwork.NickName); }
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Disable player if found in list of players
        if (propertiesThatChanged["DeadPlayers"] != null)
        {
            deadPlayers = (propertiesThatChanged["DeadPlayers"] as string[]).ToList();

            if (deadPlayers.Contains(photonView.Owner.NickName))
            {
                multiTargetCamera.targets.Remove(transform);
                gameObject.SetActive(false);
            }
        };
    }

    public void ChangePlayersColor()
    {
        int i = 0;
        Color[] playerColor = { Color.green, Color.red, Color.blue, Color.yellow };
        foreach (Transform player in multiTargetCamera.targets)
        {
            player.GetComponent<SpriteRenderer>().color = playerColor[i];
            i++;
        }
    }

    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }
}
