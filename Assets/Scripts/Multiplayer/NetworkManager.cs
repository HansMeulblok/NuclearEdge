using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public UIManager uIManager;
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to server....");

            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.NickName = "Test";
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
            MaxPlayers = 2
        };

        if (uIManager.IsInputCorrect(uIManager.l_createRoomNameInput))
        {
            PhotonNetwork.CreateRoom(uIManager.l_createRoomNameInput.text, options, TypedLobby.Default);
        }
    }

    public void CreateRoomMultiplayer()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2
        };

        if (uIManager.IsInputCorrect(uIManager.mp_createRoomNameInput))
        {
            PhotonNetwork.CreateRoom(uIManager.mp_createRoomNameInput.text, options, TypedLobby.Default);
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
        if (uIManager.IsInputCorrect(uIManager.l_joinNameInput))
        {
            PhotonNetwork.JoinRoom(uIManager.l_joinNameInput.text);
        }
    }

    public void JoinRoomMultiplayer()
    {
        if (uIManager.IsInputCorrect(uIManager.mp_joinNameInput))
        {
            PhotonNetwork.JoinRoom(uIManager.mp_joinNameInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        print("Joined room successfull.");
        uIManager.CreateLobby(PhotonNetwork.CurrentRoom.Name);

        uIManager.startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Failed joining room. Reason: " + message);
    }
    #endregion

    #region Lobby
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom) { PhotonNetwork.LeaveRoom(true); print("Player left room."); }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        uIManager.startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    #endregion
}
