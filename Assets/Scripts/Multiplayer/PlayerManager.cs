using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private SpriteRenderer playerSprite;

    private MultiTargetCamera multiTargetCamera;
    private Dictionary<int, bool> playersLoaded;
    private Dictionary<int, string> playerColors;

    private List<int> deadPlayers;
    private bool isRendered = false;

    void Awake()
    {
        if (!photonView.IsMine) { enabled = false; return; }

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        deadPlayers = new List<int>();
        playersLoaded = new Dictionary<int, bool>();

        if (PhotonNetwork.IsMasterClient)
        {
            if (!playersLoaded.ContainsKey(photonView.ViewID)) { playersLoaded.Add(photonView.ViewID, true); }
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PlayersLoaded", playersLoaded } });
            
            StartCoroutine(ChangePlayersColor());
        }
        else
        {
            StartCoroutine(CheckMasterLoaded());
        }
    }

    private void Update()
    {
        // Add player name to list of dead player for server
        if (MultiTargetCamera.createdPlayerList && isRendered && !playerSprite.isVisible)
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        if (!deadPlayers.Contains(photonView.OwnerActorNr))
        {
            deadPlayers.Add(photonView.OwnerActorNr);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // Disable player if found in list of players
        if (propertiesThatChanged["DeadPlayers"] != null)
        {
            deadPlayers = (propertiesThatChanged["DeadPlayers"] as int[]).ToList();
        };

        if (propertiesThatChanged["PlayersLoaded"] != null)
        {
            playersLoaded = (Dictionary<int, bool>)propertiesThatChanged["PlayersLoaded"];

            if (playersLoaded.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                MultiTargetCamera.allPlayersCreated = true;
            }
        };

        if (propertiesThatChanged["playerColors"] != null)
        {
            playerColors = propertiesThatChanged["playerColors"] as Dictionary<int, string>;

            // Sets name and color of players
            foreach (PhotonView player in multiTargetCamera.pvPlayers)
            {
                ColorUtility.TryParseHtmlString(playerColors[player.ViewID], out Color colorTemp);

                player.gameObject.GetComponent<SpriteRenderer>().color = colorTemp;
                player.gameObject.GetComponentInChildren<TextMeshPro>().text = player.Owner.NickName;
            }
        }
    }

    public IEnumerator CheckMasterLoaded()
    {
        yield return new WaitUntil(() => playersLoaded.Count >= 1);

        if (!playersLoaded.ContainsKey(photonView.ViewID))
        {
            playersLoaded.Add(photonView.ViewID, true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PlayersLoaded", playersLoaded } });
        }

        yield break;
    }

    public IEnumerator ChangePlayersColor()
    {
        yield return new WaitUntil(() => MultiTargetCamera.createdPlayerList == true);

        Dictionary<int, string> playerColors = new Dictionary<int, string>();
        Color[] playerColor = { Color.green, Color.red, Color.blue, Color.yellow };

        int i = 0;
        foreach (PhotonView player in multiTargetCamera.pvPlayers)
        {
            string color = $"#{ColorUtility.ToHtmlStringRGBA(playerColor[i])}";
            playerColors.Add(player.ViewID, color);
            i++;
        }

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerColors"))
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "playerColors", playerColors } });
        }

        yield break;
    }

    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!deadPlayers.Contains(otherPlayer.ActorNumber)) { deadPlayers.Add(otherPlayer.ActorNumber); }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
    }
}

