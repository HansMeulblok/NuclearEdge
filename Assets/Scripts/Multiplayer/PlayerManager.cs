using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject nameplate;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private List<int> deadPlayers;
    private bool isRendered = false;
    private Dictionary<int, bool> playersLoaded;

    private Dictionary<string, string> playerColors;

    void Awake()
    {
        Invoke("SetNames", 1f); // TODO: CHANGE THIS
        
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

        };

        if (propertiesThatChanged["playerColors"] != null)
        {
            playerColors = propertiesThatChanged["playerColors"] as Dictionary<string, string>;

            // CHANGED TO VIEWID
            foreach (Transform player in multiTargetCamera.targets)
            {
                string playerName = player.GetComponent<PhotonView>().Owner.NickName;
                Color colorTemp = new Color();
                ColorUtility.TryParseHtmlString(playerColors[playerName], out colorTemp);
                player.gameObject.GetComponent<SpriteRenderer>().color = colorTemp;
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

        if (PhotonNetwork.IsMasterClient)
        {
            // CHANGED TO VIEWID
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
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "playerColors", playerColors } });
            }
        }

    }

    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    foreach (Transform player in multiTargetCamera.targets)
    //    {
    //        int viewId = player.gameObject.GetComponent<PhotonView>().ViewID;
            
    //        if (otherPlayer.NickName == playerName)
    //        {
    //            if (!deadPlayers.Contains(playerName)) { deadPlayers.Add(playerName); }
    //            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers.ToArray() } });
    //            multiTargetCamera.targets.Remove(player);
    //            break;
    //        }
    //    }
    //}

    public void SetNames()
    {
        nameplate.GetComponent<TextMeshPro>().text = photonView.Owner.NickName;
    }
}

