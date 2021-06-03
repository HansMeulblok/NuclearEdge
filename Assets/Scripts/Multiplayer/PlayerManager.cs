using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private List<int> deadPlayers;
    private bool isRendered = false;
    private Dictionary<int, bool> playersLoaded;

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
        }
        else
        {
            StartCoroutine(CheckMasterLoaded());
        }

        StartCoroutine(ChangePlayersColor());
    }

    private void Update()
    {
        // Add player name to list of dead player for server
        if (MultiTargetCamera.createdPlayerList && isRendered && !playerSprite.isVisible)
        {
            if (!deadPlayers.Contains(photonView.ViewID))
            {
                deadPlayers.Add(photonView.ViewID);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
            }
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
