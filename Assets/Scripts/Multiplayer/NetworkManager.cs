using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public MenuManager menuManager;

    [SerializeField]
    private const int MENU_INDEX = 0;

    private bool sceneLoaded = false;

    void Start()
    {
        DontDestroyOnLoad(this);

        // Server settings
        PhotonNetwork.GameVersion = "0.0.2";
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        // Making sure that player is not in a room when connecting for the first time or when reconnecting
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        

        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to server....");

            PhotonNetwork.ConnectUsingSettings();    // TODO: Add local play option
        }
        else
        {
            // Show lobby when returning from room
            menuManager.BackToLobbyScreen();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Player connected to server.");

        if (!PhotonNetwork.InLobby)
        {
            print("Joining lobby");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server. Reason: " + cause.ToString());

        // Trying to reconnect player
        StartCoroutine(ReconnectPlayer());
    }

    private IEnumerator ReconnectPlayer()
    {
        yield return new WaitForSeconds(5);

        print("Attempting to reconnect... ");
        ConnectToServer();

        yield break;
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
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "StartGame", true } });
        PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = false;
        int i = Random.Range(1, SceneManager.sceneCountInBuildSettings);
        PhotonNetwork.LoadLevel(i);
        SceneManager.sceneLoaded += OnSceneLoaded; // Checks if scene is loaded for host
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(MENU_INDEX))
        {
            PhotonNetwork.LoadLevel(0);
            Destroy(gameObject);
        }
        else
        {
            menuManager.BackToLobbyScreen();
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom) { PhotonNetwork.LeaveRoom(); print("Player left room."); }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["StartGame"] != null)
        {
            if ((bool)propertiesThatChanged["StartGame"] && !sceneLoaded)
            {
                //PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "StartGame", false } });

                SceneManager.sceneLoaded += OnSceneLoaded; // Checks if scene is loaded for client
                sceneLoaded = true;
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

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(MENU_INDEX) && !playerFound)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
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
