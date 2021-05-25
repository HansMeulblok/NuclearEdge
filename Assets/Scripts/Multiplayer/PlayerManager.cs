using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    private List<string> deadPlayers;
    private List<KeyValuePair<string, Color>> playerColors;

    private Vector2 networkPosition;
    private float networkRotation;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        deadPlayers = new List<string>();

        Invoke("ChangePlayersColor", 1f);
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
                if (multiTargetCamera.targets.Count != 0)
                {
                    multiTargetCamera.targets.Remove(transform);
                    gameObject.SetActive(false);
                }
            }

            if (multiTargetCamera.targets.Count == 1)
            {
                if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerWon"))
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerWon", multiTargetCamera.targets[0].gameObject.GetComponent<PhotonView>().Owner.NickName } });
                }
            }
        };

        if (propertiesThatChanged["playerColors"] != null)
        {
            playerColors = (propertiesThatChanged["playerColors"] as KeyValuePair<string, Color>[]).ToList();

            foreach (KeyValuePair<string, Color> playerColor in playerColors)
            {
                foreach (Transform player in multiTargetCamera.targets)
                {
                    if(player.gameObject.GetComponent<PhotonView>().Owner.NickName == playerColor.Key)
                    {
                        player.gameObject.GetComponent<SpriteRenderer>().color = playerColor.Value;
                    }
                }
            }
        }
    }

    public void ChangePlayersColor()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<KeyValuePair<string, Color>> playerColors = new List<KeyValuePair<string, Color>>();
            Color[] playerColor = { Color.green, Color.red, Color.blue, Color.yellow };
            for (int i = 0; i < multiTargetCamera.targets.Count; i++)
            {
                string playerName = multiTargetCamera.targets[i].gameObject.GetComponent<PhotonView>().Owner.NickName;
                Color color = playerColor[i];
                playerColors.Add(new KeyValuePair<string, Color>(playerName, color));
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerColors", playerColors.ToArray() } });
        }
    }

    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Transform player in multiTargetCamera.targets)
        {
            string playerName = player.gameObject.GetComponent<PhotonView>().Owner.NickName;
            if (otherPlayer.NickName == playerName)
            {
                if (!deadPlayers.Contains(playerName)) { deadPlayers.Add(playerName); }
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
                multiTargetCamera.targets.Remove(player);
                break;
            }
        }
    }
}
