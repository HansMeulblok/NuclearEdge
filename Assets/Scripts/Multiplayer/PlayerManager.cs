using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    private List<string> deadPlayers;

    void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        deadPlayers = new List<string>();

        Invoke("ChangePlayersColor", 1f);
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
