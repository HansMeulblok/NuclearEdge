using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;

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

        Invoke("ChangePlayersColor", 0.6f);
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
