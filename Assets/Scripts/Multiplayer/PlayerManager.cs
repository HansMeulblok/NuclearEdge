using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject nameplate;
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    private List<string> deadPlayers;
    private Dictionary<string, string> playerColors;
    private Vector2 networkPosition;
    private float networkRotation;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        deadPlayers = new List<string>();

        Invoke("SetNames", 1f);
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
            KillPlayer(PhotonNetwork.NickName);
        }
    }

    public void KillPlayer(string name)
    {
        if (!deadPlayers.Contains(name)) { deadPlayers.Add(name); }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
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
            playerColors = propertiesThatChanged["playerColors"] as Dictionary<string, string>;

            foreach (Transform player in multiTargetCamera.targets)
            {
                string playerName = player.GetComponent<PhotonView>().Owner.NickName;
                Color colorTemp = new Color();
                ColorUtility.TryParseHtmlString(playerColors[playerName], out colorTemp);
                player.gameObject.GetComponent<SpriteRenderer>().color = colorTemp;
            }
        }
    }

    public void ChangePlayersColor()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<string, string> playerColors = new Dictionary<string, string>();
            Color[] playerColor = { Color.green, Color.red, Color.blue, Color.yellow };
            for (int i = 0; i < multiTargetCamera.targets.Count; i++)
            {
                string playerName = multiTargetCamera.targets[i].gameObject.GetComponent<PhotonView>().Owner.NickName;
                string color = $"#{ColorUtility.ToHtmlStringRGBA(playerColor[i])}";
                playerColors.Add(playerName, color);
            }
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerColors"))
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerColors", playerColors } });
            }
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

    public void SetNames()
    {
        nameplate.GetComponent<TextMeshPro>().text = photonView.Owner.NickName;
    }
}

