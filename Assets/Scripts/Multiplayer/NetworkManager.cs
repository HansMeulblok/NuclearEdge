using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public MenuManager menuManager;

    void Start()
    {
        DontDestroyOnLoad(this);
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        // TODO: Add local play option
        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to server....");

            PhotonNetwork.GameVersion = "0.0.2";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Player connected to server.");
        if (!PhotonNetwork.InLobby) { PhotonNetwork.JoinLobby(); }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server. Reason: " + cause.ToString());
    }

    #region Create Room
    public void CreateRoomLocal()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4
        };

        if (menuManager.IsInputCorrect(menuManager.l_createRoomNameInput))
        {
            PhotonNetwork.CreateRoom(menuManager.l_createRoomNameInput.text, options, TypedLobby.Default);
        }
    }

    public void CreateRoomMultiplayer()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4
        };

        if (menuManager.IsInputCorrect(menuManager.mp_createRoomNameInput))
        {
            PhotonNetwork.CreateRoom(menuManager.mp_createRoomNameInput.text, options, TypedLobby.Default);
        }
    }

    public override void OnCreatedRoom()
    {
        print("Created room successfully.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed creating room. Reason: " + message);
    }
    #endregion

    #region Join Room
    public void JoinRoomLocal()
    {
        if (menuManager.IsInputCorrect(menuManager.l_joinNameInput))
        {
            PhotonNetwork.JoinRoom(menuManager.l_joinNameInput.text);
        }
    }

    public void JoinRoomMultiplayer()
    {
        if (menuManager.IsInputCorrect(menuManager.mp_joinNameInput))
        {
            PhotonNetwork.JoinRoom(menuManager.mp_joinNameInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        print("Joined room successfully.");
        menuManager.CreateLobby(PhotonNetwork.CurrentRoom.Name);
        menuManager.startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Failed joining room. Reason: " + message);
    }
    #endregion

    #region Game Logic
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "StartGame", true } });
        PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = false;
        int i = Random.Range(1, SceneManager.sceneCountInBuildSettings);
        PhotonNetwork.LoadLevel(i); 
        SceneManager.sceneLoaded += OnSceneLoaded; // Checks if scene is loaded for host
    }

    public override void OnLeftRoom()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom) { PhotonNetwork.LeaveRoom(true); print("Player left room."); }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["StartGame"] != null)
        {
            if ((bool)propertiesThatChanged["StartGame"])
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "StartGame", false } });

                SceneManager.sceneLoaded += OnSceneLoaded; // Checks if scene is loaded for client
            }
        };
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool playerFound = false;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerFound = true;
            }
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) && !playerFound)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (menuManager != null)
        {
            menuManager.startButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
    #endregion

}
